import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal, WritableSignal } from '@angular/core';
import { environment } from '../environment/environment.dev';
import { AuthResponse } from '../_models/response/authresponse.model';
import { Observable, tap, catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { ToastmessageService } from './toastmessage.service';
import { jwtDecode } from 'jwt-decode';

@Injectable({ providedIn: 'root' })
export class AccountService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;
  private router = inject(Router);
  private toast = inject(ToastmessageService);
  private currentUserSignal: WritableSignal<AuthResponse | null> = signal(null);
  public readonly currentUser = this.currentUserSignal.asReadonly();

  private logoutTimer: any = null;
  private sessionWarningSignal = signal(false);
  public readonly sessionWarning$ = this.sessionWarningSignal.asReadonly();

  constructor() {
    const user = this.getStoredUser();
    if (user) {
      this.setCurrentUser(user, false); // restore user, don’t reset expiry
    }
  }

  setCurrentUser(
    user: AuthResponse,
    applyNewExpiry = true,
    remember = false
  ): void {
    const now = Date.now();

    const expiresAt = applyNewExpiry
      ? now + 1000 * 60 * 10 // fixed 10-minute expiry
      : user.expiresAt ?? now + 1000 * 60 * 10;

    const userWithExpiry = { ...user, expiresAt };
    localStorage.setItem('user', JSON.stringify(userWithExpiry));
    //localStorage.setItem('token', user.token);
    this.currentUserSignal.set(user);
    this.scheduleAutoLogout(expiresAt - now);
  }

  private scheduleAutoLogout(timeoutMs: number): void {
    if (this.logoutTimer) clearTimeout(this.logoutTimer);

    // Show warning 30s before actual logout
    const warningOffset = 1000 * 30; // 30 seconds
    const warningTime = timeoutMs - warningOffset;

    if (warningTime > 0) {
      setTimeout(() => {
        console.warn('Session is about to expire');
        this.sessionWarningSignal.set(true); // Show modal
      }, warningTime);
    }

    // 🔐 Auto logout
    this.logoutTimer = setTimeout(() => {
      this.logout();
      this.router.navigate(['/login']);
    }, timeoutMs);
  }

  getStoredUser(): AuthResponse | null {
    const raw = localStorage.getItem('user');
    if (!raw) return null;

    try {
      const parsed: AuthResponse = JSON.parse(raw);
      const now = Date.now();

      if (typeof parsed.expiresAt === 'number' && parsed.expiresAt > now) {
        return parsed;
      }

      localStorage.removeItem('user');
      return null;
    } catch {
      localStorage.removeItem('user');
      return null;
    }
  }

  extendSession(): void {
    const currentUser = this.currentUserSignal();
    if (!currentUser) return;

    this.sessionWarningSignal.set(false); // hide modal

    // Reapply current session with fresh expiry
    this.setCurrentUser(currentUser, true, true); // treat like remembered
  }

  login(payload: {
    email: string;
    password: string;
    rememberMe: boolean;
  }): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.baseUrl}userauth/login`, payload)
      .pipe(
        tap((response) => {
          if (response.isSuccess && response.token?.trim().length > 0) {
            this.setCurrentUser(response, true, payload.rememberMe);
          } else {
            this.toast.error('Login failed:', response.message);
            //console.warn('Login failed:', response.message);
          }
        }),
        catchError((error) => {
          console.error('Login error:', error);
          return throwError(() => error);
        })
      );
  }

  logout(): void {
    localStorage.removeItem('user');
    this.currentUserSignal.set(null);
    this.sessionWarningSignal.set(false); // Hide session modal
    this.toast.info('Account has been logged out');
    if (this.logoutTimer) {
      clearTimeout(this.logoutTimer);
      this.logoutTimer = null;
    }
  }

  getLoggedInUserId(): string {
    const raw = localStorage.getItem('user');
    if (!raw) return '';

    try {
      const parsed: AuthResponse = JSON.parse(raw);
      const decoded: any = jwtDecode(parsed.token);      
      return decoded?.id?.toString() ?? ''; // or decoded.user_id depending on your token claim
    } catch {
      return '';
    }
  }
}
