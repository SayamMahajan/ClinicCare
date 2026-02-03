import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable, tap } from 'rxjs';
import { AuthResponse, LoginFormValue } from '../models/auth.model';

@Injectable({ providedIn: 'root' })
export class AuthService {

  private baseUrl = `${environment.apiUrl}/api`;

  private http = inject(HttpClient);

  loginPatient(data: LoginFormValue): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(
      `${this.baseUrl}/patients/login`,
      data
    ).pipe(
      tap(res => localStorage.setItem('token', res.token))
    );
  }

  loginEmployee(data: LoginFormValue): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(
      `${this.baseUrl}/employees/login`,
      data
    ).pipe(
      tap(res => localStorage.setItem('token', res.token))
    );
  }

  registerPatient(data: any) {
    return this.http.post(
      `${this.baseUrl}/patients/register`,
      data
    );
  }

  registerEmployee(data: any) {
    return this.http.post(
      `${this.baseUrl}/employees/register`,
      data
    );
  }

  logout() {
    localStorage.removeItem('token');
  }

  get token() {
    return localStorage.getItem('token');
  }

  private decodeToken(token: string): any {
    try {
      const payload = token.split('.')[1];
      const base64 = payload.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = decodeURIComponent(
        atob(base64)
          .split('')
          .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
          .join('')
      );
      return JSON.parse(jsonPayload);
    } catch (e) {
      return null;
    }
  }

  get userId(): string | null {
    const token = this.token;
    if (!token) return null;
    const decoded = this.decodeToken(token);
    return decoded?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || 
          decoded?.['sub'] || 
          decoded?.['nameid'] || null;
  }

  get role(): string | null {
    const token = this.token;
    if (!token) return null;
    const decoded = this.decodeToken(token);
    return decoded?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || 
          decoded?.['role'] || null;
  }

  get email(): string | null {
    const token = this.token;
    if (!token) return null;
    const decoded = this.decodeToken(token);
    return decoded?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || 
          decoded?.['email'] || null;
  }

  get decodedToken(): any {
    const token = this.token;
    return token ? this.decodeToken(token) : null;
  }

}
