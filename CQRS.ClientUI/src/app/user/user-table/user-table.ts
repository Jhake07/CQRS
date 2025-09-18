import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule, NgClass } from '@angular/common';
import { StatusClassPipe } from '../../_pipes/status-class-pipe';
import { NoResults } from '../../shared/no-results/no-results';
import { SortableHeaderDirective } from '../../_directives/sortable-header.directive';
import { ViewUserRequest } from '../../_models/appuser/viewuserrequest.model';
import { UpdateUserRequest } from '../../_models/appuser/updateuserrequest.model';
import { ResetUserRequest } from '../../_models/appuser/resetuserrequest.model';

@Component({
  selector: 'app-user-table',
  standalone: true,
  imports: [
    CommonModule,
    NoResults,
    StatusClassPipe,
    NgClass,
    SortableHeaderDirective,
  ],
  templateUrl: './user-table.html',
  styleUrl: './user-table.css',
})
export class UserTable {
  @Input() userList: ViewUserRequest[] = [];

  @Output() edit = new EventEmitter<ViewUserRequest>();
  @Output() view = new EventEmitter<ViewUserRequest>();
  @Output() reset = new EventEmitter<ResetUserRequest>();

  @Input() sortColumn: string = '';
  @Input() sortDirection: 'asc' | 'desc' | '' = '';
  @Output() sort = new EventEmitter<{
    column: string;
    direction: '' | 'asc' | 'desc';
  }>();
}
