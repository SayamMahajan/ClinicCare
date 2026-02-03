import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { AppointmentResponseDto, AppointmentStatus, TimeSlot } from '../models/appointment.model';

@Injectable({
  providedIn: 'root'
})
export class AppointmentService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/api/appointments`;

  getAppointments(status?: AppointmentStatus): Observable<AppointmentResponseDto[]> {
    let params = new HttpParams();
    if (status) {
      params = params.set('status', status);
    }
    return this.http.get<any>(this.baseUrl, { params }).pipe(
      map((response: any) => Array.isArray(response) ? response : response.data || []),
      catchError(() => of([]))
    );
  }

  getById(id: string): Observable<AppointmentResponseDto> {
    return this.http.get<AppointmentResponseDto>(`${this.baseUrl}/${id}`);
  }

  create(appointment: any): Observable<string> {
    return this.http.post<{ id: string }>(this.baseUrl, appointment).pipe(
      map(response => response.id),
      catchError(() => of(''))
    );
  }

  update(id: string, appointment: any): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, appointment);
  }
  
  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
