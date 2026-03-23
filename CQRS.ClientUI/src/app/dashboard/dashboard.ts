import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { LayoutComponent } from '../layout/layout/layout';
@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css'],
})
export class DashboardComponent implements OnInit {
  private layout = inject(LayoutComponent);
  ngOnInit(): void {
    this.layout.updatePageTitle('Dashboard');
  }
}
