import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import {
  AppointmentSearchParams,
  PaginatedResult,
} from '../shared/models/pagination.model';
import {
  AppointmentResponseDto,
  AppointmentCreateDto,
  AppointmentUpdateDto,
} from '../shared/models/appointment.model';

@Injectable({
  providedIn: 'root',
})
export class AppointmentService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/api/appointments`;

  getAll(
    params: AppointmentSearchParams
  ): Observable<PaginatedResult<AppointmentResponseDto>> {
    let httpParams = new HttpParams()
      .set('pageNumber', params.pageNumber.toString())
      .set('pageSize', params.pageSize.toString());

    if (params.searchTerm) httpParams = httpParams.set('searchTerm', params.searchTerm);
    if (params.status) httpParams = httpParams.set('status', params.status);
    if (params.prescriptionId)
      httpParams = httpParams.set('prescriptionId', params.prescriptionId);
    if (params.startDate) httpParams = httpParams.set('startDate', params.startDate);
    if (params.endDate) httpParams = httpParams.set('endDate', params.endDate);

    return this.http
      .get<PaginatedResult<AppointmentResponseDto>>(this.baseUrl, { params: httpParams })
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

  getById(id: string): Observable<AppointmentResponseDto | null> {
    return this.http
      .get<AppointmentResponseDto>(`${this.baseUrl}/${id}`)
      .pipe(catchError(() => of(null)));
  }

  create(appointment: AppointmentCreateDto): Observable<void> {
    return this.http.post<void>(this.baseUrl, appointment);
  }

  update(id: string, appointment: AppointmentUpdateDto): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, appointment);
  }

  patch(id: string, appointment: AppointmentUpdateDto): Observable<void> {
    return this.http.patch<void>(`${this.baseUrl}/${id}`, appointment);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}