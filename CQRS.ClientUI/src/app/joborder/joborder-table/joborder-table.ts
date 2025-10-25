import { CommonModule, NgClass } from '@angular/common';
import {
  Component,
  Input,
  Output,
  EventEmitter,
  OnChanges,
} from '@angular/core';
import { NoResults } from '../../shared/no-results/no-results';
import { StatusClassPipe } from '../../_pipes/status-class-pipe';
import { SortableHeaderDirective } from '../../_directives/sortable-header.directive';
import { IjobOrder } from '../../_models/joborder/joborder.model';

@Component({
  selector: 'app-joborder-table',
  standalone: true,
  imports: [
    CommonModule,
    NoResults,
    StatusClassPipe,
    NgClass,
    SortableHeaderDirective,
  ],
  templateUrl: './joborder-table.html',
  styleUrl: './joborder-table.css',
})
export class JoborderTable {
  @Input() joborderList: IjobOrder[] = [];

  @Output() edit = new EventEmitter<IjobOrder>();
  @Output() view = new EventEmitter<IjobOrder>();
  @Output() cancel = new EventEmitter<number>();

  @Input() sortColumn: string = '';
  @Input() sortDirection: 'asc' | 'desc' | '' = '';
  @Output() sort = new EventEmitter<{
    column: string;
    direction: '' | 'asc' | 'desc';
  }>();
}
