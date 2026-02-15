import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, catchError, of } from 'rxjs';
import { PaginatedResult, PaymentSearchParams } from '../shared/models/pagination.model';
import { environment } from '../../environments/environment';
import { PaymentResponseDto, PaymentCreateDto } from '../shared/models/payment.model';

@Injectable({ providedIn: 'root' })
export class PaymentService {
  private baseUrl = `${environment.apiUrl}/api/payments`;
  private http = inject(HttpClient);

  getAll(params: PaymentSearchParams): Observable<PaginatedResult<PaymentResponseDto>> {
    let httpParams = new HttpParams()
      .set('pageNumber', params.pageNumber.toString())
      .set('pageSize', params.pageSize.toString());

    if (params.searchTerm) httpParams = httpParams.set('searchTerm', params.searchTerm);
    if (params.patientId) httpParams = httpParams.set('patientId', params.patientId);
    if (params.doctorId) httpParams = httpParams.set('doctorId', params.doctorId);
    if (params.type) httpParams = httpParams.set('type', params.type);
    if (params.startDate) httpParams = httpParams.set('startDate', params.startDate);
    if (params.endDate) httpParams = httpParams.set('endDate', params.endDate);

    return this.http
      .get<PaginatedResult<PaymentResponseDto>>(this.baseUrl, { params: httpParams })
      .pipe(
        catchError(() =>
          of({
            items: [],
            pageNumber: params.pageNumber,
            pageSize: params.pageSize,
            totalPages: 0,
            hasPreviousPage: false,
            hasNextPage: false,
          })
        )
      );
  }

  getById(id: string): Observable<PaymentResponseDto | null> {
    return this.http
      .get<PaymentResponseDto>(`${this.baseUrl}/${id}`)
      .pipe(catchError(() => of(null)));
  }

  create(payment: PaymentCreateDto): Observable<void> {
    return this.http.post<void>(this.baseUrl, payment);
  }
}