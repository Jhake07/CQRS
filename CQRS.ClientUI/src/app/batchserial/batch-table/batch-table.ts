import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule, NgClass } from '@angular/common';
import { BatchSerial } from '../../_models/batchserial/batchserial';
import { StatusClassPipe } from '../../_pipes/status-class-pipe';
import { NoResults } from '../../shared/no-results/no-results';
import { SortableHeaderDirective } from '../../_directives/sortable-header.directive';

@Component({
  selector: 'app-batch-table',
  standalone: true,
  imports: [
    CommonModule,
    NoResults,
    StatusClassPipe,
    NgClass,
    SortableHeaderDirective,
  ],
  templateUrl: './batch-table.html',
  styleUrl: './batch-table.css',
})
export class BatchTable {
  @Input() batchList: BatchSerial[] = [];

  @Output() edit = new EventEmitter<BatchSerial>();
  @Output() view = new EventEmitter<BatchSerial>();
  @Output() cancel = new EventEmitter<number>();

  @Input() sortColumn: string = '';
  @Input() sortDirection: 'asc' | 'desc' | '' = '';
  @Output() sort = new EventEmitter<{
    column: string;
    direction: '' | 'asc' | 'desc';
  }>();
}
