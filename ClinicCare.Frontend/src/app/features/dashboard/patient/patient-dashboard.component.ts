import { Component } from '@angular/core';
import { CommonModule, TitleCasePipe } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatOptionModule } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatTableModule } from '@angular/material/table';
import { CdkTableModule } from '@angular/cdk/table';

interface Appointment {
  id: string;
  doctorName: string;
  date: string;
  slot: string;
  status?: string;
}

@Component({
  selector: 'app-patient-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    MatSidenavModule,
    MatListModule,
    MatToolbarModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatOptionModule,
    MatIconModule,
    MatButtonModule,
    MatDividerModule,
    TitleCasePipe,
    MatTableModule,
    CdkTableModule
  ],
  templateUrl: './patient-dashboard.component.html'
})
export class PatientDashboardComponent {
  section: 'profile' | 'upcoming' | 'request' | 'past' = 'profile';
  search = '';
  slotType = '';
  slotTypes = ['morning', 'earlynoon', 'latenoon', 'evening', 'night'];

  appointments: Appointment[] = [
    { id: '1', doctorName: 'Dr. Amit Sharma', date: '2026-01-29', slot: 'morning', status: 'approved' },
    { id: '2', doctorName: 'Dr. Neha Verma', date: '2026-01-29', slot: 'earlynoon', status: 'cancelled' },
    { id: '3', doctorName: 'Dr. Rahul Singh', date: '2026-01-28', slot: 'latenoon', status: 'completed' },
    { id: '4', doctorName: 'Dr. Priya Patel', date: '2026-01-27', slot: 'evening', status: 'cancelled' },
    { id: '5', doctorName: 'Dr. Suresh Kumar', date: '2026-01-29', slot: 'night', status: 'requested' }
  ];

  constructor(private router: Router) {}

  get filteredAppointments() {
    const today = '2026-01-29'; // In real app, use new Date() and format as needed
    let filtered = this.appointments.filter(a => {
      if (this.section === 'upcoming') {
        // Only future appointments (date > today)
        return (a.status === 'approved' || a.status === 'cancelled') && a.date >= today;
      }
      if (this.section === 'request') {
        return a.status === 'requested';
      }
      if (this.section === 'past') {
        // Only past appointments (date < today)
        return (a.status === 'completed' || a.status === 'cancelled') && a.date < today;
      }
      return false;
    });
    if (this.search.trim()) {
      const s = this.search.toLowerCase();
      filtered = filtered.filter(a =>
        a.doctorName.toLowerCase().includes(s)
      );
    }
    if (this.slotType) {
      filtered = filtered.filter(a => a.slot === this.slotType);
    }
    return filtered;
  }

  goToAppointment(id: string) {
    this.router.navigate(['/appointment', id]);
  }

  requestNewAppointment() {
    // TODO: Implement request new appointment logic
    alert('Request new appointment clicked!');
  }

  logout() {
    // TODO: Implement actual logout logic (clear tokens, redirect, etc.)
    alert('Signed out!');
  }
}
