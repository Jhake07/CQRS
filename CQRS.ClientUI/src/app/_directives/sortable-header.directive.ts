import {
  Directive,
  EventEmitter,
  HostListener,
  Input,
  Output,
} from '@angular/core';

@Directive({
  selector: '[appSortableHeader]',
  standalone: true,
})
export class SortableHeaderDirective {
  @Input() column: string = '';
  @Input() direction: 'asc' | 'desc' | '' = '';

  @Output() sort = new EventEmitter<{
    column: string;
    direction: '' | 'asc' | 'desc';
  }>();

  @HostListener('click')
  rotate(): void {
    const rotateMap: Record<string, '' | 'asc' | 'desc'> = {
      '': 'asc',
      asc: 'desc',
      desc: '',
    };
    this.direction = rotateMap[this.direction];
    this.sort.emit({ column: this.column, direction: this.direction });
  }
}
