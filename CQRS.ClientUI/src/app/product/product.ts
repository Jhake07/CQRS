import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { ProductService } from '../_services/product.service';
import { ToastmessageService } from '../_services/toastmessage.service';
import { ConfirmService } from '../_services/confirm.service';
import { FormGroup } from '@angular/forms';
import { FormMode } from '../_enums/form-mode.enum';
import { ProductFormFactory } from '../_formfactories/product-form.factory';
import { SharedFormsModule } from '../shared/shared-forms/shared-forms-module';
import { FormUtilsService } from '../_services/form-utils.service';
import { generate, Observable, of, timer } from 'rxjs';
import { finalize, switchMap, tap } from 'rxjs/operators';
import { Product } from '../_models/product/product.model';
import { CustomResultResponse } from '../_models/response/customresultresponse.model';
import { PaginatorService } from '../_services/paginator.service';
import { ProductTable } from './product-table/product-table';
import { FormAccessService } from '../_services/form-access.service';
import { Brand } from '../_enums/brand.enum';
import { GenericStatus } from '../_enums/genericstatus.enum';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-product',
  standalone: true,
  imports: [SharedFormsModule, ProductTable, CommonModule],
  templateUrl: './product.html',
  styleUrl: './product.css',
})
export class ProductComponent implements OnInit {
  private productService = inject(ProductService);
  private toastr = inject(ToastmessageService);
  private confirmService = inject(ConfirmService);
  private formFactory = inject(ProductFormFactory);
  private formUtils = inject(FormUtilsService);
  private paginator = inject(PaginatorService);
  private formAccess = inject(FormAccessService);

  // Forms Declaration
  productForm!: FormGroup;
  productList = signal<Product[]>([]);
  selectedProduct: Product | null = null;
  FormMode = FormMode; // exposes enum to html template
  formMode: FormMode = FormMode.None; // tracks current mode

  showValidationAlert = false;
  isSaving = false;

  brandOptions = Object.values(Brand);
  statusOptions = Object.values(GenericStatus);

  componentOptions = signal(['Motherboard', 'PCBI', 'Antenna']);
  selectedComponents = signal<string[]>([]);
  componentSummary = computed(() => this.selectedComponents().join(';'));

  // Table Declaration
  pageSize = signal<number>(5);
  currentPage = signal(1);
  sortColumn = signal('');
  sortDirection = signal<'' | 'asc' | 'desc'>('');
  searchBox = signal('');
  tableLoading = signal(false);

  readonly totalFilteredItems = computed(
    () => this.filteredProductList().length
  );
  readonly filteredProductList = computed(() => {
    const status = this.activeStatus();
    const search = this.searchBox().toLowerCase();

    return this.productList()
      .filter((product) => status === 'All' || product.stats === status)
      .filter(
        (product) =>
          product.modelCode?.toLowerCase().includes(search) ||
          product.description?.toLowerCase().includes(search)
      );
  });

  readonly statusTabs = [
    'All',
    'Open',
    'In Progress',
    'Cancelled',
    'Completed',
  ] as const;
  readonly activeStatus = signal<(typeof this.statusTabs)[number]>('All');

  //#region Initialization Functions
  readonly editableFields: string[] = [
    'modelCode',
    'description',
    'brand',
    'defaultJOQty',
    'components',
    'accessories',
    'stats',
  ];

  ngOnInit(): void {
    this.formMode = FormMode.New;
    this.initializeForm();
    this.loadProducts();
  }

  private initializeForm(): void {
    this.productForm = this.formFactory.create(this.formMode);
  }

  private loadProducts(): void {
    this.tableLoading.set(true);
    // Minimum 3-second buffer before hiding spinner
    this.productService
      .getAll()
      .pipe(
        switchMap((data) =>
          timer(1000).pipe(
            // delay for 2 seconds
            switchMap(() => {
              this.productList.set(data);
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
              ? 'No Products found. Please check with System Administrator.'
              : 'An expected error occurred.';
          this.toastr.error(msg, 'Loading Error.');
        },
      });
  }

  toggleComponent(component: string, isChecked: boolean): void {
    const current = this.selectedComponents();
    const updated = isChecked
      ? [...current, component]
      : current.filter((c) => c !== component);

    this.selectedComponents.set(updated);
    this.productForm.get('components')?.setValue(updated.join(';'));
  }
  //#endregion

  //#region Submit/Update Functions
  openConfirmation(): void {
    this.showValidationAlert = false;

    if (this.productForm.invalid) {
      this.formUtils.markAllTouched(this.productForm);
      this.showValidationAlert = true;
      return;
    }

    this.confirmService
      .confirm(
        'Confirmation',
        'Are you sure to save this product entry?',
        'Yes',
        'Cancel'
      )
      .pipe(
        tap((confirmed) => {
          if (confirmed) {
            this.isSaving = true;
            this.productForm.disable();
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
    if (this.productForm.invalid) {
      this.toastr.warning(
        'Please double-check the form before submitting',
        'Invalid form'
      );
      return;
    }

    this.isSaving = true;
    this.productForm.disable();

    const payload = this.productForm.getRawValue();
    const request$ = this.getSaveOrUpdateRequest(payload);

    request$.subscribe({
      next: (response) => this.handleGenericResponse(response, 'save'),
      error: (err) => this.handleError(err),
      complete: () => this.finalizeSubmission(),
    });
  }

  private getSaveOrUpdateRequest(
    payload: Product
  ): Observable<CustomResultResponse> {
    return this.formMode === FormMode.Edit && this.selectedProduct
      ? this.productService.updateProduct(this.selectedProduct.id!, payload)
      : this.productService.saveProduct(payload);
  }

  private updateSignalList(): void {
    const newItem = this.productForm.value;

    if (this.formMode === FormMode.Edit && this.selectedProduct) {
      const updatedList = this.productList().map((item) =>
        item.modelCode === this.selectedProduct?.modelCode ? newItem : item
      );
      this.productList.set(updatedList);
    } else {
      this.productList.update((list) => [...list, newItem]);
    }
  }

  private handleError(err: unknown): void {
    this.productForm.enable();
    const res = (err as any)?.error as CustomResultResponse;

    const alreadyHandled = (err as any)?._handledByInterceptor;

    if (!alreadyHandled && res?.message) {
      this.toastr.showDetailedError(res);
    }

    this.isSaving = false;
  }

  private finalizeSubmission(): void {
    this.isSaving = false;
    this.tableLoading.set(true);
    this.loadProducts();
    this.productForm.enable();
  }
  //#endregion

  //#region Table Functions
  readonly paginatedProductList = computed(() =>
    this.filteredProductList().slice(
      (this.currentPage() - 1) * this.pageSize(),
      this.currentPage() * this.pageSize()
    )
  );

  readonly totalFilteredCount = computed(
    () => this.filteredProductList().length
  );

  readonly statusCounts = computed(() => {
    const list = this.productList();
    const counts: Record<string, number> = {
      All: list.length,
      Open: list.filter((product) => product.stats === 'Open').length,
      'In Progress': list.filter((product) => product.stats === 'In Progress')
        .length,
      Cancelled: list.filter((product) => product.stats === 'Cancelled').length,
      Completed: list.filter((product) => product.stats === 'Completed').length,
    };
    return counts;
  });

  readonly totalPages = computed(() => {
    const totalItems = this.filteredProductList().length;
    const pageSize = this.pageSize();
    return Math.max(1, Math.ceil(totalItems / pageSize));
  });

  handleSort(event: { column: string; direction: '' | 'asc' | 'desc' }): void {
    const key = event.column as keyof Product;

    this.sortColumn.set(key);
    this.sortDirection.set(event.direction);

    const sorted = [...this.productList()].sort((a, b) => {
      const valA = a[key];
      const valB = b[key];

      const strA = valA?.toString().toLowerCase() ?? '';
      const strB = valB?.toString().toLowerCase() ?? '';

      return event.direction === 'asc'
        ? strA.localeCompare(strB)
        : event.direction === 'desc'
        ? strB.localeCompare(strA)
        : 0;
    });

    this.productList.set(sorted);
  }

  applyFieldAccess(): void {
    this.formAccess.applyAccess(
      this.productForm,
      this.formMode,
      this.editableFields
    );
  }

  populateFormEdit(product: Product): void {
    this.formMode = FormMode.Edit;
    this.productForm.enable();
    this.applyFieldAccess();
    this.productForm.patchValue(product);
    this.selectedProduct = product;

    //check the components checkbox if they're present in the component textbox
    const rawComponents = product.components ?? '';
    const parsed = rawComponents
      .split(';')
      .map((c) => c.trim())
      .filter((c) => c);
    this.selectedComponents.set(parsed);
  }

  populateFormView(product: Product): void {
    this.formMode = FormMode.View;
    this.productForm.disable();
    this.productForm.patchValue(product);
    this.selectedProduct = product;

    //check the components checkbox if they're present in the component textbox
    const rawComponents = product.components ?? '';
    const parsed = rawComponents
      .split(';')
      .map((c) => c.trim())
      .filter((c) => c);
    this.selectedComponents.set(parsed);
  }

  cancelProduct(id: number): void {
    this.isSaving = true;

    this.confirmService
      .confirm(
        'Confirm Cancellation',
        'Are you sure to cancel this product?',
        'Yes',
        'No'
      )
      .pipe(
        switchMap((confirmed) => {
          if (!confirmed) {
            this.finalizeCancellation();
            return of(null);
          }
          return this.productService.cancel(id);
        })
      )
      .subscribe({
        next: (response) => this.handleGenericResponse(response, 'cancel'),
        error: (err) => this.handleError(err),
        complete: () => this.finalizeSubmission(),
      });
  }

  private finalizeCancellation(): void {
    this.isSaving = false;
  }
  //#endregion

  //#region Generic Function
  resetForm(): void {
    this.formMode = FormMode.New;
    this.selectedProduct = null;
    this.showValidationAlert = false;

    this.formUtils.resetWithDefaults(this.productForm, {
      id: null,
      modelCode: '',
      description: '',
      brand: '',
      defaultJOQty: 0,
      components: '',
      accessories: 0,
      stats: '',
    });
    this.selectedComponents.set([]);
  }

  private handleGenericResponse(
    response: CustomResultResponse | null,
    context: 'save' | 'cancel'
  ): void {
    if (!response) return;

    const isSuccess = response.isSuccess;
    const message = response.message;
    const validationErrors = response.validationErrors;

    if (isSuccess) {
      const title =
        context === 'save'
          ? this.formMode === FormMode.Edit
            ? 'Update Successful'
            : 'Save Successful'
          : 'Cancelled';

      this.toastr.success(message, title);
      this.resetForm();

      if (context === 'save') {
        this.updateSignalList();
      }
    } else {
      const errorTitle =
        context === 'save' ? 'Submission Error' : 'Cancellation Failed';

      this.toastr.error(message, errorTitle);
      this.toastr.showValidationWarnings(validationErrors);
    }

    if (context === 'cancel') {
      this.loadProducts();
    }
  }
  //#endregion
}
