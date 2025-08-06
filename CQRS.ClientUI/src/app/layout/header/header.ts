import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AccountService } from '../../_services/account.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './header.html',
  styleUrl: './header.css',
})
export class Header {
  constructor(private accountService: AccountService, private router: Router) {}

  onLogout(): void {
    if (this.accountService.currentUser()) {
      this.accountService.logout();
      this.router.navigate(['/login']);
    }
  }
}
