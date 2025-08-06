import { Injectable } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FormMode } from '../_enums/form-mode.enum';

@Injectable({ providedIn: 'root' })
export class UserFormFactory {
  constructor(private fb: FormBuilder) {}

  create(mode: FormMode): FormGroup {
    return this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      username: ['', Validators.required],
      role: ['', Validators.required],
      password: [''],
      confirmPassword: [''],
      useDefaultPassword: [{ value: true, disabled: true }],
      isActive: [null, Validators.required],
      token: [''], // Optional: might be readonly or handled in the background
    });
  }
}
