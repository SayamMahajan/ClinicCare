import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, catchError, of } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  EmployeeSearchParams,
  PaginatedResult,
} from '../shared/models/pagination.model';
import {
  EmployeeResponseDto,
  EmployeeUpdateDto,
  AdminDashboardResponse,
} from '../shared/models/employee.model';

@Injectable({
  providedIn: 'root',
})
export class EmployeeService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/api/employees`;

  getAll(params: EmployeeSearchParams): Observable<PaginatedResult<EmployeeResponseDto>> {
    let httpParams = new HttpParams()
      .set('pageNumber', params.pageNumber.toString())
      .set('pageSize', params.pageSize.toString())
      .set('role', params.role);

    if (params.searchTerm) httpParams = httpParams.set('searchTerm', params.searchTerm);
    if (params.gender) httpParams = httpParams.set('gender', params.gender);
    if (params.specializationId)
      httpParams = httpParams.set('specializationId', params.specializationId);

    return this.http
      .get<PaginatedResult<EmployeeResponseDto>>(this.baseUrl, { params: httpParams })
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

  getAdminDashboard(): Observable<AdminDashboardResponse | null> {
    return this.http
      .get<AdminDashboardResponse>(`${this.baseUrl}/admin-dashboard`)
      .pipe(
        catchError((error) => {
          console.error('Failed to load admin dashboard:', error);
          return of(null);
        })
      );
  }

  getById(id: string): Observable<EmployeeResponseDto | null> {
    return this.http
      .get<EmployeeResponseDto>(`${this.baseUrl}/${id}`)
      .pipe(catchError(() => of(null)));
  }

  update(id: string, employee: EmployeeUpdateDto): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, employee);
  }

  patch(id: string, employee: EmployeeUpdateDto): Observable<void> {
    return this.http.patch<void>(`${this.baseUrl}/${id}`, employee);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}