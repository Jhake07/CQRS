import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { CustomResultResponse } from '../_models/response/customresultresponse.model';

@Injectable({
  providedIn: 'root',
})
export class ToastmessageService {
  constructor(private toastr: ToastrService) {}

  success(message: string, title = 'Successful') {
    this.toastr.success(message, title);
  }

  info(message: string, title = 'Information') {
    this.toastr.info(message, title);
  }

  warning(message: string, title = 'Warning') {
    this.toastr.warning(message, title);
  }

  error(message: string, title = 'Error') {
    this.toastr.error(message, title);
  }

  showValidationWarnings(
    errors?: Record<string, string[]>,
    customTitle = 'Validation Warning'
  ) {
    if (!errors) return;
    Object.entries(errors).forEach(([field, messages]) => {
      messages.forEach((msg) => {
        this.toastr.warning(msg, `Validation: ${field}`);
      });
    });
  }

  showDetailedError(
    response: CustomResultResponse,
    customTitle = 'Submission Error'
  ) {
    let errorToast = response.message
      ? ` ${response.message}`
      : ' An error occurred.';

    if (response.validationErrors) {
      const validationMessages = Object.entries(response.validationErrors)
        .flatMap(([_, msgs]) => msgs.map((msg) => `• ${msg}`))
        .join('\n');

      errorToast += `\n\nErrors:\n${validationMessages}`;
    }

    this.toastr.show(
      errorToast.replace(/\n/g, '<br/>'),
      customTitle,
      {
        enableHtml: true,
        disableTimeOut: false,
        timeOut: 9000,
        extendedTimeOut: 3000,
      },
      'toast-error' // or 'toast-warning', 'toast-info', etc.
    );
  }
}
