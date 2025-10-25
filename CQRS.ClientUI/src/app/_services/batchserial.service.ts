import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BatchSerial } from '../_models/batchserial/batchserial.model';
import { environment } from '../environment/environment.dev';
import { Observable } from 'rxjs';
import { CustomResultResponse } from '../_models/response/customresultresponse.model';
import { AccountService } from './account.service';

@Injectable({ providedIn: 'root' })
export class BatchSerialService {
  private readonly baseUrl = `${environment.apiUrl}batchserial`;

  constructor(
    private http: HttpClient,
    private accountService: AccountService
  ) {}

  getAll(): Observable<BatchSerial[]> {
    return this.http.get<BatchSerial[]>(this.baseUrl);
  }

  getByContractNo(contractNo: string): Observable<BatchSerial> {
    return this.http.get<BatchSerial>(`${this.baseUrl}/contract/${contractNo}`);
  }

  getAvailable(): Observable<BatchSerial[]> {
    return this.http.get<BatchSerial[]>(`${this.baseUrl}/available`);
  }

  save(payload: BatchSerial): Observable<CustomResultResponse> {
    const createdBy = this.accountService.getLoggedInUserId();
    const enrichedPayload = { ...payload, createdBy };
    //console.log(enrichedPayload);
    return this.http.post<CustomResultResponse>(this.baseUrl, enrichedPayload);
  }

  update(id: number, payload: BatchSerial): Observable<CustomResultResponse> {
    const createdBy = this.accountService.getLoggedInUserId();
    const enrichedPayload = { ...payload, createdBy };
    //console.log(enrichedPayload);
    return this.http.put<CustomResultResponse>(
      `${this.baseUrl}/${id}`,
      enrichedPayload
    );
  }

  cancel(id: number): Observable<CustomResultResponse> {
    return this.http.delete<CustomResultResponse>(`${this.baseUrl}/${id}`);
  }
}
