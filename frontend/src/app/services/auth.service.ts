import { Injectable, PLATFORM_ID, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { isPlatformBrowser } from '@angular/common';
import { tap } from 'rxjs/operators';
import { Observable } from 'rxjs';

const API = 'http://localhost:5241/api/auth';
const ROLE_CLAIM = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
const NAME_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private platformId = inject(PLATFORM_ID);

  login(email: string, password: string): Observable<{ token: string; message: string }> {
    return this.http
      .post<{ token: string; message: string }>(`${API}/login`, { email, password })
      .pipe(tap(res => this.store('token', res.token)));
  }

  register(email: string, password: string): Observable<string> {
    return this.http.post(`${API}/register`, { email, password }, { responseType: 'text' });
  }

  logout(): void {
    this.remove('token');
  }

  getToken(): string | null {
    return isPlatformBrowser(this.platformId) ? localStorage.getItem('token') : null;
  }

  isLoggedIn(): boolean {
    const token = this.getToken();
    if (!token) return false;
    try {
      const { exp } = this.decode(token);
      return exp * 1000 > Date.now();
    } catch {
      return false;
    }
  }

  getEmail(): string {
    return this.claim(NAME_CLAIM) ?? '';
  }

  getRole(): string {
    const r = this.claim(ROLE_CLAIM);
    return Array.isArray(r) ? r[0] : (r ?? '');
  }

  getPermissions(): string[] {
    const p = this.claim('permission');
    if (!p) return [];
    return Array.isArray(p) ? p : [p];
  }

  hasPermission(permission: string): boolean {
    return this.getPermissions().includes(permission);
  }

  isAdmin(): boolean {
    return this.getRole() === 'Admin';
  }

  private claim(key: string): any {
    const token = this.getToken();
    if (!token) return null;
    try {
      return this.decode(token)[key] ?? null;
    } catch {
      return null;
    }
  }

  private decode(token: string): any {
    const b64 = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
    return JSON.parse(atob(b64));
  }

  private store(key: string, value: string): void {
    if (isPlatformBrowser(this.platformId)) localStorage.setItem(key, value);
  }

  private remove(key: string): void {
    if (isPlatformBrowser(this.platformId)) localStorage.removeItem(key);
  }
}
