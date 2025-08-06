import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { RegisterUserRequest } from '../_models/appuser/registeruserrequest';
import { AppuserService } from '../_services/appuser.service';
import { ToastmessageService } from '../_services/toastmessage.service';
import { FormMode } from '../_enums/form-mode.enum';
import { SharedFormsModule } from '../shared/shared-forms/shared-forms-module';
import { SharedTablesModule } from '../shared/shared-tables/shared-tables-module';
import { PaginatorService } from '../_services/paginator.service';
import { UserFormFactory } from '../_formfactories/user-form.factory';
import { FormAccessService } from '../_services/form-access.service';
import { FormUtilsService } from '../_services/form-utils.service';
import { ConfirmService } from '../_services/confirm.service';
import { finalize, of, switchMap, tap, timer } from 'rxjs';
import { CustomResultResponse } from '../_models/response/customresultresponse';
import { UpdateUserRequest } from '../_models/appuser/updateuserrequest';
import { ViewUserRequest } from '../_models/appuser/viewuserrequest';
import { ResetUserRequest } from '../_models/appuser/resetuserrequest';
import { UserTable } from "./user-table/user-table";

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [SharedFormsModule, SharedTablesModule, UserTable],
  templateUrl: './user.html',
  styleUrl: './user.css',
})
export class User implements OnInit {
  private appUserService = inject(AppuserService);
  private toast = inject(ToastmessageService);
  private confirmService = inject(ConfirmService);
  private paginator = inject(PaginatorService);
  private formFactory = inject(UserFormFactory);
  private formAccess = inject(FormAccessService);
  private formUtils = inject(FormUtilsService);

  FormMode = FormMode;

  userForm!: FormGroup;
  userList = signal<ViewUserRequest[]>([]);
  selectedUser: ViewUserRequest | null = null;

  formMode: FormMode = FormMode.None;
  isSaving = false;
  showValidationAlert = false;

  pageSize = signal<number>(5);
  currentPage = signal(1);
  sortColumn = signal('');
  sortDirection = signal<'' | 'asc' | 'desc'>('');
  searchBox = signal('');
  User: any;
  // Get current year
  currentYear = new Date().getFullYear();

  ngOnInit(): void {
    this.formMode = FormMode.New;
    this.initializeForm();
    this.loadUsers();
  }
  readonly editableFields: string[] = ['role', 'isActive'];

  private initializeForm(): void {
    this.userForm = this.formFactory.create(this.formMode);
    this.applyFieldAccess();
  }

  private loadUsers(): void {
    this.appUserService.getAll().subscribe({
      next: (data) => {
        this.userList.set(data);
      },
      error: (error) => {
        const msg =
          error.status === 404
            ? 'User not found. Please check the API.'
            : 'An unexpected error occured.';
        this.toast.error(msg, 'Load Error');
      },
    });
  }

  openConfirmation(): void {
    this.showValidationAlert = true;

    if (this.userForm.invalid) {
      this.formUtils.markAllTouched(this.userForm);
      this.showValidationAlert = true;
      return;
    }

    this.confirmService
      .confirm(
        'Confirm Save',
        'Are you sure you want to save this user?',
        'Yes, Save',
        'Cancel'
      )
      .pipe(
        tap((confirmed) => {
          if (confirmed) {
            this.isSaving = true;
            this.userForm.disable();
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
    if (this.userForm.invalid) {
      this.toast.warning(
        'Please double check the form before submitting.',
        'Validation Warning'
      );
      return;
    }

    this.isSaving = true;
    this.userForm.disable();

    const payload: UpdateUserRequest = {
      username: this.userForm.get('username')?.value,
      email: this.userForm.get('email')?.value,
      role: this.userForm.get('role')?.value,
      isActive: this.userForm.get('isActive')?.value,
    };

    const payload1: RegisterUserRequest = {
      firstName: this.userForm.get('firstName')?.value,
      lastName: this.userForm.get('lastName')?.value,
      email: this.userForm.get('email')?.value,
      username: this.userForm.get('username')?.value,
      password: this.userForm.get('password')?.value,
      confirmPassword: this.userForm.get('confirmPassword')?.value,
      role: this.userForm.get('role')?.value,
      isActive: this.userForm.get('isActive')?.value,
      token: '', // or default if needed
      id: 0, // or undefined if using auto-gen on backend
    };

    const request$ =
      this.formMode === FormMode.Edit && this.selectedUser
        ? this.appUserService.updateStatusRole(payload)
        : this.appUserService.save(payload1); // save() still uses full User object

    request$.subscribe({
      next: (response) => {
        if (response.isSuccess) {
          this.toast.success(
            response.message,
            this.formMode === FormMode.Edit
              ? 'Update Successful'
              : 'Save Successful'
          );

          if (this.formMode === this.FormMode.Edit) {
            const updatedList = this.userList().map((item) =>
              item.email === this.selectedUser?.email
                ? this.userForm.value
                : item
            );
            this.userList.set(updatedList);
          } else {
            this.userList.update((list) => [...list, this.userForm.value]);
          }
          this.resetForm();
        } else {
          const errors = Array.isArray(response.validationErrors)
            ? response.validationErrors
            : Object.values(response.validationErrors ?? {}).flat();

          if (errors.length > 0) {
            const combined = errors.join('\n');
            this.toast.warning(combined, response.message);
          } else {
            this.toast.error('An unexpected error occurred.', response.message);
          }
        }
      },
      error: (err) => {
        this.userForm.enable();
        const res = err?.error as CustomResultResponse;
        res?.message
          ? this.toast.showDetailedError(res)
          : this.toast.error('Unexpected error.', 'Error');
        this.isSaving = false;
      },
      complete: () => {
        this.isSaving = false;
        this.loadUsers();
        this.userForm.enable();
      },
    });
  }

  resetUserPassword(request: ResetUserRequest): void {
    this.confirmService
      .confirm(
        'Confirm Password Reset',
        `Are you sure you want to reset credentials for "${request.username}"?`,
        'Yes',
        'No'
      )
      .pipe(
        switchMap((confirmed) => {
          if (!confirmed) return of(null);
          this.isSaving = true;
          return this.appUserService.reset(request);
        }),
        finalize(() => {
          this.isSaving = false;
        })
      )
      .subscribe({
        next: (response) => {
          if (!response) return;

          if (response.isSuccess) {
            this.toast.success(response.message, 'Reset Successful');
            this.resetForm(); // Optional: reset selected user form
          } else {
            this.toast.error(response.message, 'Reset Failed');
            this.toast.showValidationWarnings(response.validationErrors);
          }

          this.loadUsers();
        },
        error: (err) => {
          const res = err?.error as CustomResultResponse;
          if (res?.message) {
            this.toast.showDetailedError(res);
          } else {
            this.toast.error('Unexpected error format.', 'Reset Failed');
          }
        },
      });
  }

  populateFormEdit(user: ViewUserRequest): void {
    this.formMode = FormMode.Edit;
    this.userForm.enable();
    this.applyFieldAccess();
    this.userForm.patchValue(user);
    this.selectedUser = user;
  }

  populateFormView(user: ViewUserRequest): void {
    this.formMode = FormMode.View;
    this.userForm.disable();
    this.userForm.patchValue(user);
    this.selectedUser = user;
  }

  resetForm(): void {
    this.formMode = FormMode.New;
    this.selectedUser = null;
    this.showValidationAlert = false;

    this.formUtils.resetWithDefaults(this.userForm, {
      id: null,
    });
  }

  paginatedUserList = computed(() => {
    const search = this.searchBox().toLowerCase().trim();
    const page = this.currentPage();
    const size = this.pageSize();

    let filtered = this.userList().filter((item) => {
      return (
        item.firstName?.toLowerCase().includes(search) ||
        item.lastName?.toLowerCase().includes(search) ||
        item.email?.toLowerCase().includes(search) ||
        item.username?.toLowerCase().includes(search)
      );
    });

    if (this.sortColumn() && this.sortDirection()) {
      filtered.sort((a, b) => {
        const valA = a[this.sortColumn() as keyof ViewUserRequest];
        const valB = b[this.sortColumn() as keyof ViewUserRequest];
        if (valA == null || valB == null) return 0;
        return this.sortDirection() === 'asc'
          ? valA > valB
            ? 1
            : -1
          : valA < valB
          ? 1
          : -1;
      });
    }

    return this.paginator.getPaginated(filtered, size, page);
  });

  handleSort(event: { column: string; direction: '' | 'asc' | 'desc' }): void {
    this.sortColumn.set(event.column); //updates the signal value
    this.sortDirection.set(event.direction); //updates the signal value
  }

  applyFieldAccess(): void {
    this.formAccess.applyAccess(
      this.userForm,
      this.formMode,
      this.editableFields
    );
  }

  capitalizeFirstLetter(value: string): string {
    return value
      ? value.charAt(0).toUpperCase() + value.slice(1).toLowerCase()
      : '';
  }

  updateUserName(): void {
    const firstName = this.userForm.get('firstName')?.value?.trim() || '';
    const lastName = this.userForm.get('lastName')?.value?.trim() || '';

    const firstInitial = firstName.charAt(0).toUpperCase();

    const titleCaseLastName = lastName
      .split(' ')
      .map(
        (word: string) =>
          word.charAt(0).toUpperCase() + word.slice(1).toLowerCase()
      )
      .join('');

    const generatedUsername = firstInitial + titleCaseLastName;
    const generatedPassword = `${generatedUsername}@${this.currentYear}`;

    this.userForm.get('username')?.setValue(generatedUsername);
    this.userForm.get('password')?.setValue(generatedPassword);
    this.userForm.get('confirmPassword')?.setValue(generatedPassword);
  }
}
