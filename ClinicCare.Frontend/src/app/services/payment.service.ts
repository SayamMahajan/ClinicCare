import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { Payment } from '../models/payment.model';

@Injectable({ providedIn: 'root' })
export class PaymentService {
  private baseUrl = `${environment.apiUrl}/api/payments`;
  private http = inject(HttpClient);

  getPayments(): Observable<Payment[]> {
    return this.http.get<Payment[]>(this.baseUrl);
  }

  getPaymentById(id: string): Observable<Payment> {
    return this.http.get<Payment>(`${this.baseUrl}/${id}`);
  }

  createPayment(payload: {
    recipientId: string;
    senderId: string;
    amount: number;
  }): Observable<void> {
    return this.http.post<void>(this.baseUrl, payload);
  }
}
