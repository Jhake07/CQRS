import { Injectable } from '@angular/core';
import { environment } from '../environment/environment.dev';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product } from '../_models/product/product.model';
import { CustomResultResponse } from '../_models/response/customresultresponse.model';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private readonly baseUrl = `${environment.apiUrl}product`;

  constructor(
    private http: HttpClient,
    private accountService: AccountService
  ) {}

  getAll(): Observable<Product[]> {
    return this.http.get<Product[]>(this.baseUrl);
  }

  getByCode(code: string): Observable<Product> {
    return this.http.get<Product>(`${this.baseUrl}/${code}`);
  }

  getById(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.baseUrl}/${id}`);
  }

  saveProduct(payload: Product): Observable<CustomResultResponse> {
    const createdBy = this.accountService.getLoggedInUserId();
    const enrichedPayload = { ...payload, createdBy };
    return this.http.post<CustomResultResponse>(this.baseUrl, enrichedPayload);
  }

  updateProduct(
    id: number,
    payload: Product
  ): Observable<CustomResultResponse> {
    console.log(id);
    console.log(payload);
    return this.http.put<CustomResultResponse>(
      `${this.baseUrl}/${id}`,
      payload
    );
  }

  cancel(id: number): Observable<CustomResultResponse> {
    return this.http.delete<CustomResultResponse>(`${this.baseUrl}/${id}`);
  }
}
