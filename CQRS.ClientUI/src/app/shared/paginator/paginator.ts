import { Component, Input, Output, EventEmitter } from '@angular/core';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-paginator',
  standalone: true,
  imports: [NgClass],
  templateUrl: './paginator.html',
  styleUrl: './paginator.css',
})
export class Paginator {
  @Input() currentPage = 1;
  @Input() totalPages = 1;
  @Input() pageSize = 5;

  @Output() pageSizeChange = new EventEmitter<number>();
  @Output() pageChange = new EventEmitter<number>();

  get pages(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }

  goTo(page: number): void {
    if (page >= 1 && page <= this.totalPages && page !== this.currentPage) {
      this.pageChange.emit(page);
    }
  }

  changePageSize(event: Event): void {
    const select = event.target as HTMLSelectElement;
    const newSize = Number(select.value);
    if (!isNaN(newSize)) {
      this.pageSizeChange.emit(newSize);
      this.pageChange.emit(1); // reset to page 1 on size change
    }
  }
}
