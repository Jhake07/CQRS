import { Injectable } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FormMode } from '../_enums/form-mode.enum';

@Injectable({
  providedIn: 'root',
})
export class ProductFormFactory {
  constructor(private fb: FormBuilder) {}

  create(mode: FormMode): FormGroup {
    return this.fb.group({
      id: [null],
      modelCode: ['', Validators.required],
      description: ['', Validators.required],
      brand: ['', Validators.required],
      defaultJOQty: [0, [Validators.required, Validators.min(100)]],
      components: ['', Validators.required],
      accessories: [0, [Validators.required, Validators.min(3)]],
      stats: ['', Validators.required],
    });
  }
}
