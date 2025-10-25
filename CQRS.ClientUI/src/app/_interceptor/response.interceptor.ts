import {
  HttpInterceptorFn,
  HttpRequest,
  HttpHandler,
  HttpErrorResponse,
} from '@angular/common/http';
import { inject } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { catchError, throwError } from 'rxjs';

// Optional: move this to a shared utils file if needed
function formatValidationErrors(errors: Record<string, string[]>): string {
  return Object.entries(errors)
    .map(([field, messages]) => `${field}: ${messages.join(', ')}`)
    .join('\n');
}

export const responseInterceptor: HttpInterceptorFn = (req, next) => {
  const toastr = inject(ToastrService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const res = error?.error;

      const isStructured =
        res &&
        typeof res === 'object' &&
        'isSuccess' in res &&
        'message' in res;

      if (isStructured) {
        toastr.error(res.message, 'Submission Error');

        if (res.validationErrors) {
          const formatted = formatValidationErrors(res.validationErrors);
          toastr.warning(formatted, 'Validation Details');
        }
      } else {
        toastr.error(
          'An unexpected error occurred. Please reach out to system admin or try again later.',
          'Network/Server Error'
        );
      }

      // Tag error so components can skip duplicate handling
      (error as any)._handledByInterceptor = true;

      return throwError(() => error);
    })
  );
};
