import {
  Component,
  Input,
  Output,
  Signal,
  signal,
  computed,
  EventEmitter,
} from '@angular/core';

import { NgClass } from '@angular/common';

@Component({
  selector: 'app-paginator',
  standalone: true,
  templateUrl: './paginator.html',
  styleUrl: './paginator.css',
})
export class Paginator {
  @Input({ required: true }) currentPage!: Signal<number>;
  @Input({ required: true }) pageSize!: Signal<number>;
  @Input({ required: true }) totalItems!: Signal<number>;

  @Output() pageChange = new EventEmitter<number>();
  @Output() pageSizeChange = new EventEmitter<number>();

  readonly pageSizeOptions = [5, 10, 25, 50];

  readonly totalPages = computed(() =>
    Math.max(1, Math.ceil(this.totalItems() / this.pageSize()))
  );

  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages()) {
      this.pageChange.emit(page);
    }
  }

  changePageSize(size: number) {
    this.pageSizeChange.emit(size);
    this.pageChange.emit(1); // reset to first page
  }
}
