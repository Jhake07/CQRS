import { Injectable, inject, signal, effect, DestroyRef } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { ToastmessageService } from './toastmessage.service';
import {
  fromEvent,
  merge,
  interval,
  map,
  tap,
  throttleTime,
  switchMap,
  startWith,
  filter,
  takeUntil,
  Subject,
} from 'rxjs';
import { jwtDecode } from 'jwt-decode';
import { AuthResponse } from '../_models/response/authresponse.model';
import { environment } from '../environment/environment.dev';
@Injectable({ providedIn: 'root' })
export class AccountService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private toast = inject(ToastmessageService);
  private destroyRef = inject(DestroyRef);
  private baseUrl = environment.apiUrl;
  private destroy$ = new Subject<void>();
  private currentUserSignal = signal<AuthResponse | null>(null);
  public readonly currentUser = this.currentUserSignal.asReadonly();
  private sessionWarningSignal = signal(false);
  public readonly sessionWarning$ = this.sessionWarningSignal.asReadonly();
  // private readonly IDLE_TIMEOUT_MS = 10 * 60 * 1000; // 10 min
  // private readonly WARNING_MS = 30 * 1000; // warning at 30s left
  //for testing, set shorter timeouts
  private readonly IDLE_TIMEOUT_MS = 2 * 60 * 1000; // 2 minutes
  private readonly WARNING_MS = 15 * 1000; // warn at last 15s
  constructor() {
    const stored = this.getStoredUser();
    if (stored) this.setCurrentUser(stored, false);
    this.initializeIdleTracking();
    this.destroyRef.onDestroy(() => this.destroy$.next());
  }
  private initializeIdleTracking(): void {
    const activity$ = merge(
      fromEvent(window, 'click'),
      // fromEvent(window, 'mousemove'),
      fromEvent(window, 'keydown'),
    ).pipe(
      throttleTime(1000),
      tap(() => this.resetSessionExpiry()),
    );
    activity$.pipe(takeUntil(this.destroy$)).subscribe();
    interval(1000)
      .pipe(
        startWith(0),
        map(() => this.currentUserSignal()?.expiresAt ?? 0),
        filter((expiresAt) => expiresAt > 0),
        tap((expiresAt) => {
          const now = Date.now();
          const remaining = expiresAt - now;
          if (remaining <= 0) {
            this.logout();
            this.router.navigate(['/login']);
          } else if (remaining <= this.WARNING_MS) {
            this.sessionWarningSignal.set(true);
          } else {
            this.sessionWarningSignal.set(false);
          }
        }),
        takeUntil(this.destroy$),
      )
      .subscribe();
  }
  private resetSessionExpiry(): void {
    const user = this.currentUserSignal();
    if (!user) return;
    const now = Date.now();
    const expiresAt = now + this.IDLE_TIMEOUT_MS;
    const updated = { ...user, expiresAt };
    localStorage.setItem('user', JSON.stringify(updated));
    this.currentUserSignal.set(updated);
  }
  setCurrentUser(
    user: AuthResponse,
    applyNewExpiry = true,
    remember = false,
  ): void {
    const now = Date.now();
    const expiresAt = applyNewExpiry
      ? now + this.IDLE_TIMEOUT_MS
      : (user.expiresAt ?? now + this.IDLE_TIMEOUT_MS);
    const updated = { ...user, expiresAt };
    localStorage.setItem('user', JSON.stringify(updated));
    this.currentUserSignal.set(updated);
  }
  extendSession(): void {
    const currentUser = this.currentUserSignal();
    if (!currentUser) return;
    this.sessionWarningSignal.set(false);
    this.setCurrentUser(currentUser, true);
  }
  login(payload: { email: string; password: string; rememberMe: boolean }) {
    return this.http
      .post<AuthResponse>(`${this.baseUrl}userauth/login`, payload)
      .pipe(
        tap((response) => {
          if (response.isSuccess && response.token?.trim().length > 0) {
            this.setCurrentUser(response, true, payload.rememberMe);
          } else {
            this.toast.error('Login failed:', response.message);
          }
        }),
      );
  }
  logout(): void {
    localStorage.removeItem('user');
    this.currentUserSignal.set(null);
    this.sessionWarningSignal.set(false);
    document.body.classList.remove('modal-open');
    this.toast.info('Account has been logged out');
    this.router.navigate(['/login']);
  }
  getStoredUser(): AuthResponse | null {
    const raw = localStorage.getItem('user');
    if (!raw) return null;
    try {
      const user = JSON.parse(raw) as AuthResponse;
      if (user.expiresAt && user.expiresAt > Date.now()) {
        return user;
      }
      localStorage.removeItem('user');
      return null;
    } catch {
      localStorage.removeItem('user');
      return null;
    }
  }
  getLoggedInUserId(): string {
    const raw = localStorage.getItem('user');
    if (!raw) return '';
    try {
      const parsed: AuthResponse = JSON.parse(raw);
      const decoded: any = jwtDecode(parsed.token);
      return decoded?.id?.toString() ?? '';
    } catch {
      return '';
    }
  }
}
