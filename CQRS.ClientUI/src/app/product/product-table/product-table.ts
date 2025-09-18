import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Product } from '../../_models/product/product.model';
import { CommonModule, NgClass } from '@angular/common';
import { NoResults } from '../../shared/no-results/no-results';
import { StatusClassPipe } from '../../_pipes/status-class-pipe';
import { SortableHeaderDirective } from '../../_directives/sortable-header.directive';

@Component({
  selector: 'app-product-table',
  imports: [
    CommonModule,
    NoResults,
    StatusClassPipe,
    NgClass,
    SortableHeaderDirective,
  ],
  templateUrl: './product-table.html',
  styleUrl: './product-table.css',
})
export class ProductTable {
  @Input() productList: Product[] = [];
  @Input() sortColumn: string = '';
  @Input() sortDirection: 'asc' | 'desc' | '' = '';

  @Output() sort = new EventEmitter<{
    column: string;
    direction: '' | 'asc' | 'desc';
  }>();

  //Get the data for view/edit/cancel function
  @Output() edit = new EventEmitter<Product>();
  @Output() view = new EventEmitter<Product>();
  @Output() cancel = new EventEmitter<number>();
}
