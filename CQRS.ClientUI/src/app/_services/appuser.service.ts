import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environment/environment.dev';
import { Observable } from 'rxjs';
import { RegisterUserRequest } from '../_models/appuser/registeruserrequest.model';
import { CustomResultResponse } from '../_models/response/customresultresponse.model';
import { UpdateUserRequest } from '../_models/appuser/updateuserrequest.model';
import { ViewUserRequest } from '../_models/appuser/viewuserrequest.model';
import { ResetUserRequest } from '../_models/appuser/resetuserrequest.model';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root',
})
export class AppuserService {
  private readonly baseUrl = `${environment.apiUrl}userauth`;

  constructor(
    private http: HttpClient,
    private accountService: AccountService
  ) {}

  getAll(): Observable<ViewUserRequest[]> {
    return this.http.get<ViewUserRequest[]>(this.baseUrl);
  }

  save(payload: RegisterUserRequest): Observable<CustomResultResponse> {
    const createdBy = this.accountService.getLoggedInUserId();
    const enrichedPayload = { ...payload, createdBy };
    return this.http.post<CustomResultResponse>(
      `${this.baseUrl}/register`,
      enrichedPayload
    );
  }

  updateStatusRole(
    payload: UpdateUserRequest
  ): Observable<CustomResultResponse> {
    return this.http.put<CustomResultResponse>(
      `${this.baseUrl}/update-status-role`,
      payload
    );
  }

  reset(payload: ResetUserRequest): Observable<CustomResultResponse> {
    return this.http.put<CustomResultResponse>(
      `${this.baseUrl}/reset-password`,
      payload
    );
  }
}
