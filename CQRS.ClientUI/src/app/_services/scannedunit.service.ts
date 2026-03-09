import { Injectable, inject } from '@angular/core';
import { ApiHandlerService } from './api-handler.service';
@Injectable({
  providedIn: 'root',
})
export class ScannedunitService {
  private api = inject(ApiHandlerService);
  create(mainSerial: string) {
    return this.api.post('scannedunits', { mainSerial });
  }
  updateComponents(mainSerial: string, data: any) {
    return this.api.patch(`scannedunits/${mainSerial}/components`, data);
  }
  getDetails<T>(mainSerial: string) {
    return this.api.get<T>(`scannedunits/${mainSerial}`);
  }
}
