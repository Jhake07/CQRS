import { Pipe, PipeTransform } from '@angular/core';
import { BatchStatus } from '../_enums/batchstatus.enum';
import { GenericStatus } from '../_enums/genericstatus.enum';

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
      case GenericStatus.Inactive:
        return 'bg-secondary';
      case GenericStatus.Active:
        return 'bg-info text-dark';
      default:
        return 'bg-secondary';
    }
  }
}
