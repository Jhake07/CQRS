import {
  Component,
  ElementRef,
  HostListener,
  inject,
  signal,
  effect,
  viewChild,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { AccountService } from '../../_services/account.service';
@Component({
  selector: 'app-session-warning-dialog',
  imports: [CommonModule],
  templateUrl: './session-warning-dialog.html',
  styleUrl: './session-warning-dialog.css',
  standalone: true,
})
export class SessionWarningDialog {
  private account = inject(AccountService);
  modalBox = viewChild<ElementRef>('modalBox');
  show = this.account.sessionWarning$; // controls visibility
  isVisible = signal(false); // controls fade animation
  constructor() {
    effect(() => {
      if (this.show()) {
        this.isVisible.set(true);
        document.body.classList.add('modal-open');
        setTimeout(() => this.focusFirstElement(), 25);
      } else {
        this.isVisible.set(false);
        document.body.classList.remove('modal-open');
      }
    });
  }
  // ESC closes modal (extends session)
  @HostListener('window:keydown.escape')
  onEsc() {
    if (this.show()) this.extendSession();
  }
  extendSession() {
    this.account.extendSession();
  }
  logout() {
    this.account.logout();
  }
  // Focus trap
  @HostListener('document:focusin', ['$event'])
  trapFocus(event: FocusEvent) {
    if (!this.show()) return;
    const modal = this.modalBox()?.nativeElement;
    if (!modal) return;
    if (!modal.contains(event.target)) {
      this.focusFirstElement();
    }
  }
  private focusFirstElement() {
    const modal = this.modalBox()?.nativeElement;
    if (!modal) return;
    const focusable = modal.querySelector(
      'button, [href], [tabindex]:not([tabindex="-1"])',
    ) as HTMLElement;
    focusable?.focus();
  }
}
