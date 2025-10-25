import { Injectable } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FormMode } from '../_enums/form-mode.enum';

@Injectable({ providedIn: 'root' })
export class JobOrderFormFactory {
  constructor(private fb: FormBuilder) {}

  create(mode: FormMode): FormGroup {
    return this.fb.group({
      id: [null], // Auto-generated
      joNo: ['', Validators.required],
      batchSerial_ContractNo: ['', Validators.required],
      stats: ['Open'],
      orderType: ['', Validators.required],
      orderQty: [null, [Validators.required, Validators.min(100)]],
      startTime: [null],
      endTime: [null],
      line: ['', Validators.required],
      processOrder: [0],
      isNo: ['', Validators.required],
      // leftQty: [{ value: 0, disabled: true }],
      remainingQty: [0, Validators.required],
    });
  }
}
