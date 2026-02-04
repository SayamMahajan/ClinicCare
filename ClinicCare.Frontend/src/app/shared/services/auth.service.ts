import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable, tap } from 'rxjs';
import { AuthResponse, LoginFormValue } from '../models/auth.model';
import { TokenService } from './token.service';
import { DecodedToken } from '../models/token.model';
import { jwtDecode } from 'jwt-decode';

@Injectable({ providedIn: 'root' })
export class AuthService {

  private http = inject(HttpClient);
  private tokenService = inject(TokenService);

  private baseUrl = `${environment.apiUrl}/api`;

  loginPatient(data: LoginFormValue): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.baseUrl}/patients/login`, data)
      .pipe(tap(res => this.tokenService.set(res.token)));
  }

  loginEmployee(data: LoginFormValue): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.baseUrl}/employees/login`, data)
      .pipe(tap(res => this.tokenService.set(res.token)));
  }

  registerPatient(data: any) {
    return this.http.post(`${this.baseUrl}/patients/register`, data);
  }

  registerEmployee(data: any) {
    return this.http.post(`${this.baseUrl}/employees/register`, data);
  }

  logout() {
    this.tokenService.clear();
  }

  get token(): string | null {
    return this.tokenService.get();
  }

  get decodedToken(): DecodedToken | null {
    const token = this.token;
    if (!token) return null;

    const decoded = jwtDecode<any>(token);
    return decoded;
  }


  get userId(): string | null {
    return (
      this.decodedToken?.[
        'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'
      ] ?? null
    );
  }

  get email(): string | null {
    return (
      this.decodedToken?.[
        'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'
      ] ?? null
    );
  }

  get role(): string | null {
    return (
      this.decodedToken?.[
        'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
      ] ?? null
    );
  }


  get isLoggedIn(): boolean {
    return !!this.token;
  }
}
