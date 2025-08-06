import { inject, Injectable } from '@angular/core';
import { BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { ConfirmDialog } from '../modals/confirm-dialog/confirm-dialog';
import { Observable, map, take } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ConfirmService {
  private modalService = inject(BsModalService);

  confirm(
    title = 'Confirmation',
    message = 'Are you sure?',
    btnOkText = 'Ok',
    btnCancelText = 'Cancel'
  ): Observable<boolean> {
    const config: ModalOptions = {
      initialState: {
        title,
        message,
        btnOkText,
        btnCancelText,
      },
    };

    const bsRef = this.modalService.show(ConfirmDialog, config);

    return bsRef.onHidden!.pipe(
      take(1),
      map(() => {
        const content = bsRef.content as { result?: boolean } | undefined;
        return content?.result ?? false;
      })
    );
  }
}
