import { Injectable } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { FormMode } from '../_enums/form-mode.enum';

@Injectable({ providedIn: 'root' })
export class FormAccessService {
  applyAccess(form: FormGroup, mode: FormMode, editableFields: string[]): void {
    const allFields = Object.keys(form.controls);

    allFields.forEach((field) => {
      const control = form.get(field);
      if (!control) return;

      if (mode === FormMode.New || editableFields.includes(field)) {
        control.enable();
      } else {
        control.disable();
      }
    });
  }
}
