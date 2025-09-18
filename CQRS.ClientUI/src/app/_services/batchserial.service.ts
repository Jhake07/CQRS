import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BatchSerial } from '../_models/batchserial/batchserial.model';
import { environment } from '../environment/environment.dev';
import { Observable } from 'rxjs';
import { CustomResultResponse } from '../_models/response/customresultresponse.model';

@Injectable({ providedIn: 'root' })
export class BatchSerialService {
  private readonly baseUrl = `${environment.apiUrl}batchserial`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<BatchSerial[]> {
    return this.http.get<BatchSerial[]>(this.baseUrl);
  }

  save(payload: BatchSerial): Observable<CustomResultResponse> {
    return this.http.post<CustomResultResponse>(this.baseUrl, payload);
  }

  update(id: number, payload: BatchSerial): Observable<CustomResultResponse> {
    console.log(id);
    return this.http.put<CustomResultResponse>(
      `${this.baseUrl}/${id}`,
      payload
    );
  }

  cancel(id: number): Observable<CustomResultResponse> {
    return this.http.delete<CustomResultResponse>(`${this.baseUrl}/${id}`);
  }
}
