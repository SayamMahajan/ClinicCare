import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, map, of } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PrescriptionCreateDto, PrescriptionResponseDto } from '../models/prescription.model';

@Injectable({
  providedIn: 'root',
})
export class PrescriptionService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/api/Prescriptions`;

  getAll(): Observable<PrescriptionResponseDto[]> {
    return this.http.get<any>(this.baseUrl).pipe(
      map((res) => (Array.isArray(res) ? res : (res?.data ?? []))),
      catchError(() => of([])),
    );
  }
  getById(id: string): Observable<PrescriptionResponseDto | null> {
    return this.http
      .get<PrescriptionResponseDto>(`${this.baseUrl}/${id}`)
      .pipe(catchError(() => of(null)));
  }

  createPrescription(dto: PrescriptionCreateDto): Observable<string> {
    return this.http.post<string>(this.baseUrl, dto);
  }
}
