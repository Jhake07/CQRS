import {
  ChangeDetectorRef,
  Component,
  computed,
  inject,
  OnInit,
  signal,
} from '@angular/core';
import { SharedFormsModule } from '../shared/shared-forms/shared-forms-module';
import { SharedTablesModule } from '../shared/shared-tables/shared-tables-module';
import { JoborderService } from '../_services/joborder.service';
import { ToastmessageService } from '../_services/toastmessage.service';
import { ConfirmService } from '../_services/confirm.service';
import { FormUtilsService } from '../_services/form-utils.service';
import { PaginatorService } from '../_services/paginator.service';
import { JobOrderStatus } from '../_enums/joborderstatus.enum';
import { FormMode } from '../_enums/form-mode.enum';
import { FormGroup } from '@angular/forms';
import { CustomResultResponse } from '../_models/response/customresultresponse.model';
import { IjobOrder } from '../_models/joborder/joborder.model';
import { FormAccessService } from '../_services/form-access.service';
import { switchMap, of, finalize, timer, tap } from 'rxjs';
import { JoborderTable } from './joborder-table/joborder-table';
import { JobOrderFormFactory } from '../_formfactories/job-order-form.factory';
import { BatchSerialService } from '../_services/batchserial.service';
import { BatchSerial } from '../_models/batchserial/batchserial.model';
import { JobOrderLines } from '../_enums/joborderlines.enum';
import { JobOrderType } from '../_enums/jobordertype.enum';

@Component({
  selector: 'app-joborder',
  standalone: true,
  imports: [SharedFormsModule, SharedTablesModule, JoborderTable],
  templateUrl: './joborder.html',
  styleUrl: './joborder.css',
})
export class Joborder implements OnInit {
  private batchSerialService = inject(BatchSerialService);
  private joborderService = inject(JoborderService);
  private toast = inject(ToastmessageService);
  private confirmService = inject(ConfirmService);
  private formFactory = inject(JobOrderFormFactory);
  private formAccess = inject(FormAccessService);
  private formUtils = inject(FormUtilsService);
  private paginator = inject(PaginatorService);
  private cdRef = inject(ChangeDetectorRef);

  StatusType = JobOrderStatus;
  statusOptions: string[] = Object.values(JobOrderStatus);

  jobOrderForm!: FormGroup;
  jobOrderList = signal<IjobOrder[]>([]);
  selectedJobOrder: IjobOrder | null = null;

  FormMode = FormMode;
  formMode: FormMode = FormMode.None;
  isSaving = false;
  showValidationAlert = false;

  pageSize = signal<number>(5);
  currentPage = signal(1);
  tableLoading = signal(false);
  sortColumn = signal('');
  sortDirection = signal<'' | 'asc' | 'desc'>('');
  searchBox = signal('');

  readonly batchContractList = signal<BatchSerial[]>([]);
  readonly editableFields: string[] = ['orderQty'];
  
  readonly totalFilteredItems = computed(
    () => this.filteredJobOrderList().length
  );
  private hasLoadedBatchSerials = false;

  readonly statusTabs = [
    'All',
    'Open',
    'In Progress',
    'Cancelled',
    'Completed',
  ] as const;
  readonly activeStatus = signal<(typeof this.statusTabs)[number]>('All');

  //#region Initialization Functions
  ngOnInit(): void {
    this.formMode = FormMode.None;
    this.initializeForm();
    this.loadJobOrders();
    this.loadBatchSerials();
  }

  lineOptions() {
    return [
      { label: '1', value: JobOrderLines.Line1 },
      { label: '2', value: JobOrderLines.Line2 },
      { label: 'QA', value: JobOrderLines.QA },
    ];
  }

  orderTypeOptions() {
    return [
      { label: 'Production', value: JobOrderType.Production },
      { label: 'QA', value: JobOrderType.QA },
    ];
  }

  private initializeForm(): void {
    this.jobOrderForm = this.formFactory.create(this.formMode);
  }

  private loadJobOrders(): void {
    this.tableLoading.set(true);
    this.joborderService
      .getAll()
      .pipe(
        switchMap((data) =>
          timer(1000).pipe(
            switchMap(() => {
              this.jobOrderList.set(data);
              return [];
            })
          )
        ),
        finalize(() => this.tableLoading.set(false))
      )
      .subscribe({
        error: (error) => {
          const msg =
            error.status === 404
              ? 'Job orders not found. Please check the API.'
              : 'An unexpected error occurred.';
          this.toast.error(msg, 'Load Error');
        },
      });
  }

  loadBatchSerials(): void {
    if (this.hasLoadedBatchSerials) return;

    this.batchSerialService.getAvailable().subscribe({
      next: (batch) => {
        this.batchContractList.set(batch);
        this.hasLoadedBatchSerials = true;
      },
      error: () => this.toast.error('Failed to load Job Order', 'Load Error'),
      complete: () => {
        this.jobOrderForm
          .get('batchSerial_ContractNo')
          ?.valueChanges.subscribe((contractNo) => {
            if (contractNo) {
              this.batchSerialService
                .getByContractNo(contractNo)
                .subscribe((batch) => {
                  //console.log(batch);
                  this.jobOrderForm.patchValue({ batch });
                  this.jobOrderForm.patchValue({
                    remainingQty: batch.remainingQty,
                  });
                  this.jobOrderForm.get('remainingQty')?.disable(); // restore readonly state
                });
            }
          });
      },
    });
  }

  //#endregion

  //#region Submit/Update Functions
  openConfirmation(): void {
    this.showValidationAlert = false;

    if (this.jobOrderForm.invalid) {
      this.formUtils.markAllTouched(this.jobOrderForm);
      this.showValidationAlert = true;
      return;
    }

    this.confirmService
      .confirm(
        'Confirm Save',
        'Are you sure you want to save this job order entry?',
        'Yes, Save',
        'Cancel'
      )
      .pipe(
        tap((confirmed) => {
          if (confirmed) {
            this.isSaving = true;
            this.jobOrderForm.disable();
          }
        }),
        switchMap((confirmed) => (confirmed ? timer(2000) : of(null)))
      )
      .subscribe((tick) => {
        if (tick !== null) {
          this.onSubmit();
        } else {
          this.isSaving = false;
        }
      });
  }

  onSubmit(): void {
    if (this.jobOrderForm.invalid) {
      this.toast.warning(
        'Please double-check the form before submitting.',
        'Validation Warning'
      );
      return;
    }

    this.isSaving = true;
    this.jobOrderForm.disable();

    const payload = this.jobOrderForm.getRawValue();
    const request$ =
      this.formMode === FormMode.Edit && this.selectedJobOrder
        ? this.joborderService.update(this.selectedJobOrder.id!, payload)
        : this.joborderService.save(payload);

    request$.subscribe({
      next: (response) => {
        if (response.isSuccess) {
          this.toast.success(
            response.message,
            this.formMode === FormMode.Edit
              ? 'Update Successful'
              : 'Save Successful'
          );

          if (this.formMode === FormMode.Edit) {
            // Update signal value immutably
            const updatedList = this.jobOrderList().map((item) =>
              item.batchSerial_ContractNo ===
              this.selectedJobOrder?.batchSerial_ContractNo
                ? this.jobOrderForm.value
                : item
            );
            this.jobOrderList.set(updatedList);
            this.resetForm();
          } else {
            // Add new item to signal
            this.jobOrderList.update((list) => [
              ...list,
              this.jobOrderForm.value,
            ]);
          }
        } else {
          this.toast.error(response.message, 'Submission Error');
          this.toast.showValidationWarnings(response.validationErrors);
        }
      },
      error: (err) => {
        this.jobOrderForm.enable();
        const res = err?.error as CustomResultResponse;
        res?.message
          ? this.toast.showDetailedError(res)
          : this.toast.error('Unexpected error format.', 'Error');

        // Restore form state on error
        const originalJobOrder: IjobOrder = {
          ...this.selectedJobOrder!,
          id: this.selectedJobOrder!.id!,
        };

        setTimeout(() => {
          this.populateFormEdit(originalJobOrder);
        }, 0);

        this.isSaving = false;
      },
      complete: () => {
        this.isSaving = false;
        this.loadJobOrders();
        this.jobOrderForm.enable();
      },
    });
  }

  //#endregion

  //#region Table Functions
  // readonly paginatedJobOrderList = computed(() => {
  //   const status = this.activeStatus();
  //   const search = this.searchBox().toLowerCase();

  //   return this.jobOrderList()
  //     .filter((jo) => (status === 'All' ? true : jo.stats === status))
  //     .filter(
  //       (jo) =>
  //         jo.joNo?.toLowerCase().includes(search) ||
  //         jo.isNo?.toLowerCase().includes(search) ||
  //         jo.batchSerial_ContractNo?.toLowerCase().includes(search)
  //     )
  //     .slice(
  //       (this.currentPage() - 1) * this.pageSize(),
  //       this.currentPage() * this.pageSize()
  //     );
  // });

  readonly filteredJobOrderList = computed(() => {
    const status = this.activeStatus();
    const search = this.searchBox().toLowerCase();

    return this.jobOrderList()
      .filter((jo) => status === 'All' || jo.stats === status)
      .filter(
        (jo) =>
          jo.joNo?.toLowerCase().includes(search) ||
          jo.isNo?.toLowerCase().includes(search) ||
          jo.batchSerial_ContractNo?.toLowerCase().includes(search)
      );
  });

  readonly paginatedJobOrderList = computed(() =>
    this.filteredJobOrderList().slice(
      (this.currentPage() - 1) * this.pageSize(),
      this.currentPage() * this.pageSize()
    )
  );

  readonly totalFilteredCount = computed(
    () => this.filteredJobOrderList().length
  );

  readonly statusCounts = computed(() => {
    const list = this.jobOrderList();
    const counts: Record<string, number> = {
      All: list.length,
      Open: list.filter((jo) => jo.stats === 'Open').length,
      'In Progress': list.filter((jo) => jo.stats === 'In Progress').length,
      Cancelled: list.filter((jo) => jo.stats === 'Cancelled').length,
      Completed: list.filter((jo) => jo.stats === 'Completed').length,
    };
    return counts;
  });

  readonly totalPages = computed(() => {
    const totalItems = this.filteredJobOrderList().length;
    const pageSize = this.pageSize();
    return Math.max(1, Math.ceil(totalItems / pageSize));
  });

  handleSort(event: { column: string; direction: '' | 'asc' | 'desc' }): void {
    this.sortColumn.set(event.column); //updates the signal value
    this.sortDirection.set(event.direction); //updates the signal value
  }

  applyFieldAccess(): void {
    this.formAccess.applyAccess(
      this.jobOrderForm,
      this.formMode,
      this.editableFields
    );
  }

  populateFormEdit(joborder: IjobOrder): void {
    console.log('yesyes');
    this.jobOrderForm.enable();
    this.formMode = FormMode.Edit;
    this.applyFieldAccess();

    // Patch the full job order
    this.jobOrderForm.patchValue(joborder);
    this.selectedJobOrder = joborder;

    // Load remainingQty for the selected batch
    setTimeout(() => {
      this.getBatchRemainingQty(joborder.batchSerial_ContractNo);
    }, 0);
  }

  populateFormView(joborder: IjobOrder): void {
    this.formMode = FormMode.View;
    this.jobOrderForm.enable();

    // Patch the full job order
    this.jobOrderForm.patchValue(joborder);
    this.selectedJobOrder = joborder;

    // Load remainingQty for the selected batch
    setTimeout(() => {
      this.getBatchRemainingQty(joborder.batchSerial_ContractNo);
      this.jobOrderForm.disable();
    }, 0);
  }

  private getBatchRemainingQty(contractNo: string): void {
    this.batchSerialService.getByContractNo(contractNo).subscribe({
      next: (batch) => {
        // console.log('Fetched batch:', batch);
        this.jobOrderForm.patchValue({ remainingQty: batch.remainingQty });
        this.jobOrderForm.get('remainingQty')?.disable();
      },
      error: () => {
        this.toast.error('Failed to load remaining quantity', 'Load Error');
      },
    });
  }

  cancelJobOrder(id: number): void {
    this.confirmService
      .confirm(
        'Confirm Cancellation',
        'Are you sure you want to cancel this Job Order?',
        'Yes',
        'No'
      )
      .pipe(
        switchMap((confirmed) => {
          if (!confirmed) return of(null);

          setTimeout(() => {
            this.isSaving = true;
            this.cdRef.detectChanges();
          });

          return this.joborderService.cancel(id);
        }),
        finalize(() => {
          setTimeout(() => {
            this.isSaving = false;
            this.cdRef.detectChanges();
          });
        })
      )
      .subscribe({
        next: (response) => {
          if (!response) return;

          if (response.isSuccess) {
            this.toast.success(response.message, 'Cancelled');
            this.resetForm();
          } else {
            this.toast.error(response.message, 'Cancellation Failed');
            this.toast.showValidationWarnings(response.validationErrors);
          }

          this.loadJobOrders();
        },
        error: (err) => {
          const res = err?.error as CustomResultResponse;
          res?.message
            ? this.toast.showDetailedError(res)
            : this.toast.error('Unexpected error format.', 'Error');
        },
      });
  }
  //#endregion

  //#region Generic Funtions
  allowOnlyDigits(event: KeyboardEvent): void {
    const key = event.key;
    if (!/^\d$/.test(key)) {
      event.preventDefault();
    }
  }

  resetForm(): void {
    this.formMode = FormMode.New;
    this.selectedJobOrder = null;
    this.showValidationAlert = false;

    this.formUtils.resetWithDefaults(this.jobOrderForm, {
      batchSerial_ContractNo: '',
      id: null,
      orderQty: 0,
      stats: 'Open',
      joNo: '',
      isNo: '',
      orderType: '',
      line: '',
      processOrder: 0,
    });
  }
  //#endregion
}
