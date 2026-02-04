import { Component, inject, Input, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MaterialModule } from '../../ui/material.module';
import { AuthService } from '../../services/auth.service';

type Role = 'Admin' | 'Doctor' | 'Patient';

@Component({
  selector: 'app-sidenav',
  standalone: true,
  imports: [CommonModule, RouterModule, MaterialModule],
  templateUrl: './app-sidenav.component.html'
})
export class AppSidenavComponent {
  private authService = inject(AuthService);

  role = signal<Role>(this.authService.role as Role || 'Patient');

  readonly menu: Record<Role, any[]> = {
    Admin: [
      {
        label: 'Dashboard',
        link: '/admin/dashboard'
      },
      {
        label: 'Employees',
        children: [
          { label: 'Add Employee', link: '/auth/register/employee' },
          { label: 'Manage Employees', link: '/employees' }
        ]
      },
      {
        label: 'Payments',
        link: '/payments'
      }
    ],

    Doctor: [
      {
        label: 'Appointments',
        icon: 'event',
        children: [
          { label: 'Requests', link: '/appointments/requests' },
          { label: 'Scheduled', link: '/appointments/scheduled' },
          { label: 'History', link: '/appointments/history' }
        ]
      },
      {
        label: 'Payments',
        link: '/payments'
      }
    ],

    Patient: [
      {
        label: 'Appointments',
        children: [
          { label: 'Book Appointment', link: '/appointments/book' },
          { label: 'Scheduled', link: '/appointments/scheduled' },
          { label: 'Requests', link: '/appointments/requests' },
          { label: 'History', link: '/appointments/history' }
        ]
      },
      {
        label: 'Payments',
        link: '/payments'
      }
    ]
  };
}
