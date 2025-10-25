import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { FormGroup } from '@angular/forms';

import { of, timer } from 'rxjs';
import { switchMap, tap, finalize } from 'rxjs/operators';
import { BatchSerialFormFactory } from '../_formfactories/batch-serial-form.factory';
import { BatchSerialService } from '../_services/batchserial.service';
import { ConfirmService } from '../_services/confirm.service';
import { ToastmessageService } from '../_services/toastmessage.service';
import { FormAccessService } from '../_services/form-access.service';
import { FormUtilsService } from '../_services/form-utils.service';
import { PaginatorService } from '../_services/paginator.service';

import { BatchSerial } from '../_models/batchserial/batchserial.model';
import { CustomResultResponse } from '../_models/response/customresultresponse.model';
import { FormMode } from '../_enums/form-mode.enum';
import { BatchStatus } from '../_enums/batchstatus.enum';

import { SharedFormsModule } from '../shared/shared-forms/shared-forms-module';
import { SharedTablesModule } from '../shared/shared-tables/shared-tables-module';
import { signal, computed } from '@angular/core';
import { BatchTable } from './batch-table/batch-table';
import { Product } from '../_models/product/product.model';
import { ProductService } from '../_services/product.service';

@Component({
  selector: 'app-batchserial',
  standalone: true,
  imports: [SharedFormsModule, SharedTablesModule, BatchTable],
  templateUrl: './batchserial.html',
  styleUrl: './batchserial.css',
})
export class Batchserial implements OnInit {
  private batchService = inject(BatchSerialService);
  private toast = inject(ToastmessageService);
  private confirmService = inject(ConfirmService);
  private formFactory = inject(BatchSerialFormFactory);
  private formAccess = inject(FormAccessService);
  private formUtils = inject(FormUtilsService);
  private paginator = inject(PaginatorService);
  private cdRef = inject(ChangeDetectorRef);
  private productService = inject(ProductService);
  StatusType = BatchStatus;
  FormMode = FormMode;
  statusOptions: string[] = Object.values(BatchStatus);

  batchSerialForm!: FormGroup;
  batchSerialList = signal<BatchSerial[]>([]);
  selectedBatchSerial: BatchSerial | null = null;

  formMode: FormMode = FormMode.None;
  isSaving = false;
  showValidationAlert = false;

  pageSize = signal<number>(5);
  currentPage = signal(1);
  tableLoading = signal(false);
  sortColumn = signal('');
  sortDirection = signal<'' | 'asc' | 'desc'>('');
  searchBox = signal('');

  readonly productList = signal<Product[]>([]);

  readonly totalFilteredItems = computed(
    () => this.filteredBatchSErialList().length
  );
  readonly activeStatus = signal<(typeof this.statusTabs)[number]>('All');
  readonly statusTabs = [
    'All',
    'Open',
    'In Progress',
    'Cancelled',
    'Completed',
    'Close',
  ] as const;

  //#region Initialization Functions
  readonly editableFields: string[] = [
    'contractNo',
    'customer',
    'address',
    'docNo',
    'item_ModelCode',
  ];

  ngOnInit(): void {
    this.formMode = FormMode.New;
    this.initializeForm();
    this.loadBatchSerials();
    this.loadProducts();
  }

  private initializeForm(): void {
    this.batchSerialForm = this.formFactory.create(this.formMode);
    this.applyFieldAccess();
  }

  private loadBatchSerials(): void {
    this.tableLoading.set(true);
    this.batchService
      .getAll()
      .pipe(
        switchMap((data) =>
          timer(1000).pipe(
            switchMap(() => {
              this.batchSerialList.set(data);
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
              ? 'Batch serials not found. Please check the API.'
              : 'An unexpected error occurred.';
          this.toast.error(msg, 'Load Error');
        },
      });
  }

  private loadProducts(): void {
    this.productService.getAll().subscribe({
      next: (products) => this.productList.set(products),
      error: () => this.toast.error('Failed to load products', 'Load Error'),
    });
  }

  //#endregion

  //#region Submit/Update Functions
  openConfirmation(): void {
    this.showValidationAlert = false;

    if (this.batchSerialForm.invalid) {
      this.formUtils.markAllTouched(this.batchSerialForm);
      this.showValidationAlert = true;
      return;
    }

    this.confirmService
      .confirm(
        'Confirm Save',
        'Are you sure you want to save this batch serial entry?',
        'Yes, Save',
        'Cancel'
      )
      .pipe(
        tap((confirmed) => {
          if (confirmed) {
            this.isSaving = true;
            this.batchSerialForm.disable();
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
    if (this.batchSerialForm.invalid) {
      this.toast.warning(
        'Please double-check the form before submitting.',
        'Validation Warning'
      );
      return;
    }

    this.isSaving = true;
    this.batchSerialForm.disable();

    const payload = this.batchSerialForm.getRawValue();
    const request$ =
      this.formMode === FormMode.Edit && this.selectedBatchSerial
        ? this.batchService.update(this.selectedBatchSerial.id!, payload)
        : this.batchService.save(payload);

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
            const updatedList = this.batchSerialList().map((item) =>
              item.contractNo === this.selectedBatchSerial?.contractNo
                ? this.batchSerialForm.value
                : item
            );
            this.batchSerialList.set(updatedList);
          } else {
            // Add new item to signal
            this.batchSerialList.update((list) => [
              ...list,
              this.batchSerialForm.value,
            ]);
          }

          this.resetForm();
        } else {
          this.toast.error(response.message, 'Submission Error');
          this.toast.showValidationWarnings(response.validationErrors);
        }
      },
      error: (err) => {
        this.batchSerialForm.enable();
        const res = err?.error as CustomResultResponse;
        res?.message
          ? this.toast.showDetailedError(res)
          : this.toast.error('Unexpected error format.', 'Error');

        // Restore form state on error
        const originalJobOrder: BatchSerial = {
          ...this.selectedBatchSerial!,
          id: this.selectedBatchSerial!.id!,
        };

        setTimeout(() => {
          this.populateFormEdit(originalJobOrder);
        }, 0);

        this.isSaving = false;
      },
      complete: () => {
        this.isSaving = false;
        this.loadBatchSerials();
        this.batchSerialForm.enable();
      },
    });
  }
  //#endregion

  //#region Table Functions
  readonly filteredBatchSErialList = computed(() => {
    const status = this.activeStatus();
    const search = this.searchBox().toLowerCase();

    return this.batchSerialList()
      .filter((batch) => status === 'All' || batch.status === status)
      .filter(
        (batch) =>
          batch.contractNo?.toLowerCase().includes(search) ||
          batch.docNo?.toLowerCase().includes(search) ||
          batch.customer?.toLowerCase().includes(search) ||
          batch.address?.toLowerCase().includes(search) ||
          batch.item_ModelCode?.toLowerCase().includes(search) ||
          batch.status?.toLowerCase().includes(search) ||
          batch.batchQty?.toString().includes(search)
      );
  });

  readonly paginatedBatchList = computed(() =>
    this.filteredBatchSErialList().slice(
      (this.currentPage() - 1) * this.pageSize(),
      this.currentPage() * this.pageSize()
    )
  );

  readonly totalFilteredCount = computed(
    () => this.filteredBatchSErialList().length
  );

  readonly statusCounts = computed(() => {
    const list = this.batchSerialList();
    const counts: Record<string, number> = {
      All: list.length,
      Open: list.filter((batch) => batch.status === 'Open').length,
      'In Progress': list.filter((batch) => batch.status === 'In Progress')
        .length,
      Cancelled: list.filter((batch) => batch.status === 'Cancelled').length,
      Completed: list.filter((batch) => batch.status === 'Completed').length,
    };
    return counts;
  });

  readonly totalPages = computed(() => {
    const totalItems = this.filteredBatchSErialList().length;
    const pageSize = this.pageSize();
    return Math.max(1, Math.ceil(totalItems / pageSize));
  });

  handleSort(event: { column: string; direction: '' | 'asc' | 'desc' }): void {
    this.sortColumn.set(event.column); //updates the signal value
    this.sortDirection.set(event.direction); //updates the signal value
  }

  applyFieldAccess(): void {
    this.formAccess.applyAccess(
      this.batchSerialForm,
      this.formMode,
      this.editableFields
    );
  }

  populateFormEdit(batch: BatchSerial): void {
    this.formMode = FormMode.Edit;
    this.batchSerialForm.enable();
    this.applyFieldAccess();
    this.batchSerialForm.patchValue(batch);
    this.selectedBatchSerial = batch;
  }

  populateFormView(batch: BatchSerial): void {
    this.formMode = FormMode.View;
    this.batchSerialForm.disable();
    this.batchSerialForm.patchValue(batch);
    this.selectedBatchSerial = batch;
  }

  cancelBatchSerial(id: number): void {
    this.confirmService
      .confirm(
        'Confirm Cancellation',
        'Are you sure you want to cancel this batch contract?',
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

          return this.batchService.cancel(id);
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

          this.loadBatchSerials();
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
    this.selectedBatchSerial = null;
    this.showValidationAlert = false;

    this.formUtils.resetWithDefaults(this.batchSerialForm, {
      id: null,
      contractNo: '',
      orderQty: 0,
      deliverQty: 0,
      status: 'Open',
      batchQty: 0,
    });
  }
  //#endregion
}
