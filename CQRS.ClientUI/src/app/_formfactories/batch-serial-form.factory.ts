import { Injectable } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FormMode } from '../_enums/form-mode.enum';

@Injectable({ providedIn: 'root' })
export class BatchSerialFormFactory {
  constructor(private fb: FormBuilder) {}

  create(mode: FormMode): FormGroup {
    return this.fb.group({
      id: [null],
      contractNo: ['', Validators.required],
      customer: ['', Validators.required],
      address: ['', Validators.required],
      docNo: ['', Validators.required],
      batchQty: [null, [Validators.required, Validators.min(500)]],
      orderQty: [0],
      deliverQty: [0],
      status: ['Open'],
      serialPrefix: ['', Validators.required],
      startSNo: ['', [Validators.required, Validators.pattern(/^\d+$/)]],
      endSNo: ['', [Validators.required, Validators.pattern(/^\d+$/)]],
      item_ModelCode: ['', Validators.required],
    });
  }
}
