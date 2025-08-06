import { Injectable } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Injectable({ providedIn: 'root' })
export class FormUtilsService {
  markAllTouched(form: FormGroup): void {
    Object.values(form.controls).forEach((control) => {
      control.markAsTouched();
    });
  }

  resetWithDefaults(
    form: FormGroup,
    defaults: Partial<Record<string, any>>
  ): void {
    form.reset(defaults);
    form.enable();
  }
}
