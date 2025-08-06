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
          'An unexpected error occurred. Please try again later.',
          'Network/Server Error'
        );
      }

      // Re-throw original error to let component handle it (if needed)
      return throwError(() => error);
    })
  );
};
