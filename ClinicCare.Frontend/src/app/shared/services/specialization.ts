import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { catchError, map, Observable, of, throwError } from 'rxjs';
import { SpecializationCreate, SpecializationResponse } from '../models/specialization.model';

@Injectable({
  providedIn: 'root',
})
export class SpecializationService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/api/specializations`;

  getAll(): Observable<SpecializationResponse[]> {
    return this.http.get<SpecializationResponse[]>(this.baseUrl).pipe(catchError(() => of([])));
  }

  getById(id: string): Observable<SpecializationResponse> {
    return this.http.get<SpecializationResponse>(`${this.baseUrl}/${id}`);
  }

  create(specialization: SpecializationCreate): Observable<string> {
    return this.http.post<{ id: string }>(this.baseUrl, specialization).pipe(
      map((response) => response.id),
      catchError(() => throwError(() => new Error('Failed to create specialization'))),
    );
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
