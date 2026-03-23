import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AccountService } from '../../_services/account.service';
import { BreadcrumbService } from '../../_services/breadcrumb.service';
import { ThemeService } from '../../_services/theme.service';
import { BreadcrumbComponent } from '../breadcrumb/breadcrumb';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [CommonModule, RouterModule, BreadcrumbComponent],
  templateUrl: './layout.html',
  styleUrls: ['./layout.css'],
})
export class LayoutComponent {
  theme = inject(ThemeService);
  private router = inject(Router);
  private account = inject(AccountService);
  private breadcrumbService = inject(BreadcrumbService);
  sidebarOpen = signal(true);

  currentYear = new Date().getFullYear();
  pageTitle = this.breadcrumbService.pageTitle;

  updatePageTitle(title: string) {
    this.pageTitle.set(title);
  }

  toggleSidebar() {
    this.sidebarOpen.update((v) => !v);
  }
  logout() {
    this.account.logout();
    this.router.navigate(['/login']);
  }
  toggleTheme() {
    // this.theme.toggle();
    document.body.classList.toggle('dark-theme');
  }
}
