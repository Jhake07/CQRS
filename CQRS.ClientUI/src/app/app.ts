import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { RouterOutlet, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AccountService } from './_services/account.service';
import { Header } from './layout/header/header';
import { Footer } from './layout/footer/footer';
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, Header, Footer],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App implements OnInit {
  protected readonly title = signal('CQRS.ClientUi');
  private router = inject(Router);
  public accountService = inject(AccountService); // public for template access

  isUserLogged = computed(() => this.accountService.currentUser() !== null);
  showSessionWarning = computed(() => this.accountService.sessionWarning$()); //  watch for modal

  ngOnInit(): void {
    const user = this.accountService.getStoredUser();

    if (user) {
      this.accountService.setCurrentUser(user);
      this.router.navigate(['/batchserial']);
    } else {
      this.router.navigate(['/login']);
    }
  }

  onExtendSession(): void {
    this.accountService.extendSession(); // 🕒 user chooses to stay logged in
  }

  onForceLogout(): void {
    this.accountService.logout();
    this.router.navigate(['/login']);
  }
}
