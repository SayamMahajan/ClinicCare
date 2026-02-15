import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, catchError, of } from 'rxjs';
import {
  PaginatedResult,
  PrescriptionSearchParams,
} from '../shared/models/pagination.model';
import {
  PrescriptionCreateDto,
  PrescriptionResponseDto,
} from '../shared/models/prescription.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class PrescriptionService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/api/prescriptions`;

  getAll(
    params: PrescriptionSearchParams
  ): Observable<PaginatedResult<PrescriptionResponseDto>> {
    let httpParams = new HttpParams()
      .set('pageNumber', params.pageNumber.toString())
      .set('pageSize', params.pageSize.toString());

    if (params.searchTerm) httpParams = httpParams.set('searchTerm', params.searchTerm);
    if (params.patientId) httpParams = httpParams.set('patientId', params.patientId);
    if (params.doctorId) httpParams = httpParams.set('doctorId', params.doctorId);
    if (params.startDate) httpParams = httpParams.set('startDate', params.startDate);
    if (params.endDate) httpParams = httpParams.set('endDate', params.endDate);

    return this.http
      .get<PaginatedResult<PrescriptionResponseDto>>(this.baseUrl, { params: httpParams })
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

  getById(id: string): Observable<PrescriptionResponseDto | null> {
    return this.http
      .get<PrescriptionResponseDto>(`${this.baseUrl}/${id}`)
      .pipe(catchError(() => of(null)));
  }

  create(dto: PrescriptionCreateDto): Observable<void> {
    return this.http.post<void>(this.baseUrl, dto);
  }
}