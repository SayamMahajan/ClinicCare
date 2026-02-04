import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, catchError, map, of } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Employee {
  id: number;
  firstName: string;
  lastName: string;
  role: 'Admin' | 'Doctor';
  email: string;
  dateOfJoining: string;
}

export interface DoctorResponseDto {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: 'Admin' | 'Doctor';
  dateOfJoining: string;

  specializationId?: string;
  fee?: number;
  phone?: string;
}

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/api/employees`;

  getEmployees(role?: 'Admin' | 'Doctor'): Observable<Employee[]> {
    let params = new HttpParams();
    if (role) {
      params = params.set('role', role);
    }
    
    return this.http.get<any>(this.baseUrl, { params }).pipe(
      map((response: any) => Array.isArray(response) ? response : response.data || []),
      catchError(() => of([]))
    );
  }

  getDoctors(specializationId?: string) {
  let params = new HttpParams();

  if (specializationId) {
    params = params.set('specializationId', specializationId);
  }

  return this.http
    .get<DoctorResponseDto[]>(`${this.baseUrl}/doctor-details`, { params })
    .pipe(catchError(() => of([])));
}


  getById(id: string): Observable<Employee> {
    return this.http.get<Employee>(`${this.baseUrl}/${id}`);
  }

  update(id: string, employee: Partial<Employee>): Observable<Employee> {
    return this.http.put<Employee>(`${this.baseUrl}/${id}`, employee);
  }

  patch(id: string, employee: Partial<Employee>): Observable<Employee> {
    return this.http.patch<Employee>(`${this.baseUrl}/${id}`, employee);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
