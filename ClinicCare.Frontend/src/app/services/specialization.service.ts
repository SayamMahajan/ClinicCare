import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, catchError, of } from 'rxjs';
import { environment } from '../../environments/environment';
import { PaginatedResult, PaginationParams } from '../shared/models/pagination.model';
import {
  SpecializationResponseDto,
  SpecializationCreateDto,
} from '../shared/models/specialization.model';

@Injectable({
  providedIn: 'root',
})
export class SpecializationService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/api/specializations`;

  getAll(
    params: PaginationParams
  ): Observable<PaginatedResult<SpecializationResponseDto>> {
    const httpParams = new HttpParams()
      .set('pageNumber', params.pageNumber.toString())
      .set('pageSize', params.pageSize.toString());

    return this.http
      .get<PaginatedResult<SpecializationResponseDto>>(this.baseUrl, {
        params: httpParams,
      })
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

  getById(id: string): Observable<SpecializationResponseDto | null> {
    return this.http
      .get<SpecializationResponseDto>(`${this.baseUrl}/${id}`)
      .pipe(catchError(() => of(null)));
  }

  create(dto: SpecializationCreateDto): Observable<void> {
    return this.http.post<void>(this.baseUrl, dto);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}