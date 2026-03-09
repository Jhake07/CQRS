import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, tap, throwError } from 'rxjs';
import {
  CustomResultResponse,
  CustomResultResponseWithData,
} from '../_models/response/customresultresponse.model';
import { environment } from '../environment/environment.dev';
import { ToastmessageService } from './toastmessage.service';
@Injectable({ providedIn: 'root' })
export class ApiHandlerService {
  private http = inject(HttpClient);
  private toast = inject(ToastmessageService);
  private baseUrl = environment.apiUrl;
  // -----------------------
  // POST (Command)
  // -----------------------
  post(url: string, body: any) {
    return this.http
      .post<CustomResultResponse>(`${this.baseUrl}${url}`, body)
      .pipe(
        tap((res) => this.handleResponse(res)),
        catchError((err) => this.handleHttpError(err)),
      );
  }
  // -----------------------
  // PATCH (Command)
  // -----------------------
  patch(url: string, body: any) {
    return this.http
      .patch<CustomResultResponse>(`${this.baseUrl}${url}`, body)
      .pipe(
        tap((res) => this.handleResponse(res)),
        catchError((err) => this.handleHttpError(err)),
      );
  }
  // -----------------------
  // GET (Query) — return typed data
  // -----------------------
  get<T>(url: string) {
    return this.http
      .get<CustomResultResponseWithData<T>>(`${this.baseUrl}${url}`)
      .pipe(
        tap((res) => this.handleResponse(res)),
        catchError((err) => this.handleHttpError(err)),
      );
  }
  // -----------------------
  // Centralized Response Handling
  // -----------------------
  private handleResponse(res: CustomResultResponse) {
    if (res.isSuccess) {
      this.toast.success(res.message ?? 'Success');
    } else {
      if (res.validationErrors) {
        console.log('Validation Errors1:', res.validationErrors);
        this.toast.showValidationWarnings(res.validationErrors);
      } else {
        console.log('Validation Errors2:', res.validationErrors);
        this.toast.error(res.message ?? 'An error occurred.');
      }
    }
  }
  // -----------------------
  // Centralized HTTP Error Handling
  // -----------------------
  private handleHttpError(error: any) {
    const apiError = error?.error || {};
    const res: CustomResultResponse = {
      isSuccess: false,
      message: apiError.error || apiError.message || 'Unexpected server error.',
      validationErrors: apiError.validationErrors,
      id: undefined, // ✔ FIX: Angular doesn't accept null
    };
    this.toast.showDetailedError(res);
    return throwError(() => error);
  }
}
