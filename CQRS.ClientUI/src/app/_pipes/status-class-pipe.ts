import { Pipe, PipeTransform } from '@angular/core';
import { BatchStatus } from '../_enums/batchstatus.enum';

@Pipe({
  name: 'statusClass',
  standalone: true,
})
export class StatusClassPipe implements PipeTransform {
  transform(status: string): string {
    switch (status) {
      case BatchStatus.Open:
        return 'bg-info text-dark';
      case BatchStatus.InProgress:
        return 'bg-warning text-dark';
      case BatchStatus.Completed:
        return 'bg-success';
      case BatchStatus.Cancelled:
        return 'bg-danger';
      default:
        return 'bg-secondary';
    }
  }
}
