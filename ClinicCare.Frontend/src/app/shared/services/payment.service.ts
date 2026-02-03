import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { Payment, Role } from '../models/payment.model';

@Injectable({ providedIn: 'root' })
export class PaymentService {

  private baseUrl = `${environment.apiUrl}/api/payments`;

  constructor(private http: HttpClient) {}

  getPayments(role: Role): Observable<Payment[]> {
    const params = new HttpParams().set('role', role);
    return this.http.get<Payment[]>(this.baseUrl, { params });
  }
}