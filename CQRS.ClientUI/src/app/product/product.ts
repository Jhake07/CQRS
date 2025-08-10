import { Component, inject, OnInit, signal } from '@angular/core';
import { ProductService } from '../_services/product.service';
import { ToastmessageService } from '../_services/toastmessage.service';
import { ConfirmService } from '../_services/confirm.service';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { FormMode } from '../_enums/form-mode.enum';
import { ProductFormFactory } from '../_formfactories/product-form.factory';
import { SharedFormsModule } from '../shared/shared-forms/shared-forms-module';

@Component({
  selector: 'app-product',
  standalone: true,
  imports: [SharedFormsModule],
  templateUrl: './product.html',
  styleUrl: './product.css',
})
export class Product implements OnInit {
  private productService = inject(ProductService);
  private toastr = inject(ToastmessageService);
  private confirmService = inject(ConfirmService);
  private formFactory = inject(ProductFormFactory);
  productForm!: FormGroup;
  productList = signal<Product[]>([]);

  FormMode = FormMode; // exposes enum to html template
  formMode: FormMode = FormMode.None; // tracks current mode

  showValidationAlert = false;
  //for table functionality

  // editable fields
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

  private loadProducts(): void {}
}
