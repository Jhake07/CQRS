import {
  HttpInterceptorFn,
  HttpRequest,
  HttpHandler,
  HttpErrorResponse,
} from '@angular/common/http';
import { inject } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { catchError, throwError } from 'rxjs';

export const responseInterceptor: HttpInterceptorFn = (req, next) => {
  const toastr = inject(ToastrService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const isStructured =
        error.error &&
        typeof error.error === 'object' &&
        'isSuccess' in error.error &&
        'message' in error.error;

      if (!isStructured) {
        toastr.error(
          'An unexpected error occurred. Please reach out to system admin or try again later.',
          'Network/Server Error'
        );
      }

      // Tag error to indicate interceptor has handled it (for component-level filtering)
      return throwError(() => {
        (error as any)._handledByInterceptor = !isStructured;
        return error;
      });
    })
  );
};
