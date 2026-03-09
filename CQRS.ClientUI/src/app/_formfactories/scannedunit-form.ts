import { inject, Injectable } from '@angular/core';
import { FormMode } from '../_enums/form-mode.enum';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Injectable({
  providedIn: 'root',
})
export class ScannedunitFormFactory {
  private fb = inject(FormBuilder);

  // Factory method for Station 3: Update Components
  createUpdateComponents(mode: FormMode): FormGroup {
    return this.fb.group({
      mainSerial: ['', [Validators.required, Validators.minLength(5)]],
      motherboardSerial: ['', [Validators.required]],
      pcbiSerial: ['', [Validators.required]],
      powerSupplySerial: ['', [Validators.required]],
    });
  }

  createUpdateTagForm(): FormGroup {
    return this.fb.group({
      mainSerial: ['', [Validators.required, Validators.minLength(5)]],
      tagNumber: ['', [Validators.required]],
    });
  }
}
