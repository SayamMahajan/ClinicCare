import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable, tap } from 'rxjs';
import { AuthResponse, LoginFormValue } from '../../shared/models/auth.models';

@Injectable({ providedIn: 'root' })
export class AuthService {

  private baseUrl = `${environment.apiUrl}/api`;

  constructor(private http: HttpClient) {}

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
}
