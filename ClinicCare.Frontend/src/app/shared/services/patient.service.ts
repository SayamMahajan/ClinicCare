import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { PatientProfile, PatientUpdate } from '../models/patient.model';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class PatientService {

  private http = inject(HttpClient);
  private authService = inject(AuthService);

  private baseUrl = `${environment.apiUrl}/api/patients`;

  getMyProfile(): Observable<PatientProfile> {
    const id = this.authService.userId!;
    return this.http.get<PatientProfile>(`${this.baseUrl}/${id}`);
  }

  updateMyProfile(data: PatientUpdate): Observable<void> {
    const id = this.authService.userId!;
    return this.http.put<void>(`${this.baseUrl}/${id}`, data);
  }
}
