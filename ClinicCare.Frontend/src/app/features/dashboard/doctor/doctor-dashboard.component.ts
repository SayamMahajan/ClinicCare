import { Component, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
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

interface Appointment {
  id: string;
  firstName: string;
  lastName: string;
  date: string; // YYYY-MM-DD
  slot: string; // morning, earlynoon, latenoon, evening, night
}

@Component({
  selector: 'app-doctor-dashboard',
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
    MatDividerModule
  ],
  templateUrl: './doctor-dashboard.component.html',
})
export class DoctorDashboardComponent {
  search = '';
  slotType = '';
  section = 'today';

  logout() {
    // TODO: Implement actual logout logic (clear tokens, redirect, etc.)
    alert('Logged out!');
  }

  appointments = signal<Appointment[]>([
    { id: '1', firstName: 'Amit', lastName: 'Sharma', date: '2026-01-29', slot: 'morning' },
    { id: '2', firstName: 'Neha', lastName: 'Verma', date: '2026-01-29', slot: 'earlynoon' },
    { id: '3', firstName: 'Rahul', lastName: 'Singh', date: '2026-01-28', slot: 'latenoon' },
    { id: '4', firstName: 'Priya', lastName: 'Patel', date: '2026-01-27', slot: 'evening' },
    { id: '5', firstName: 'Suresh', lastName: 'Kumar', date: '2026-01-29', slot: 'night' }
  ]);

  get filteredAppointments() {
    let filtered = this.appointments().filter(a => {
      if (this.section === 'today') return a.date === '2026-01-29';
      if (this.section === 'upcoming') return a.date > '2026-01-29';
      if (this.section === 'past') return a.date < '2026-01-29';
      return true;
    });
    if (this.search.trim()) {
      const s = this.search.toLowerCase();
      filtered = filtered.filter(a =>
        a.firstName.toLowerCase().includes(s) ||
        a.lastName.toLowerCase().includes(s)
      );
    }
    if (this.slotType) {
      filtered = filtered.filter(a => a.slot === this.slotType);
    }
    return filtered;
  }

  slotTypes = ['morning', 'earlynoon', 'latenoon', 'evening', 'night'];

  constructor(private router: Router) {}

  goToAppointment(id: string) {
    this.router.navigate(['/appointment', id]);
  }
}
