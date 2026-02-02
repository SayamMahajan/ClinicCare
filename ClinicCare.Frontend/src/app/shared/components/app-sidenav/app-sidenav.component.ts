import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MaterialModule } from '../../ui/material.module';

type Role = 'Admin' | 'Doctor' | 'Patient';

@Component({
  selector: 'app-sidenav',
  standalone: true,
  imports: [CommonModule, RouterModule, MaterialModule],
  templateUrl: './app-sidenav.component.html'
})
export class AppSidenavComponent {
  @Input({ required: true }) role!: Role;

  readonly menu: Record<Role, any[]> = {
    Admin: [
      {
        label: 'Dashboard',
        link: '/admin/dashboard'
      },
      {
        label: 'Employees',
        children: [
          { label: 'Add Employee', link: '/employees/add' },
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
        label: 'Dashboard',
        link: '/doctor/dashboard'
      },
      {
        label: 'Appointments',
        icon: 'event',
        children: [
          { label: 'Requests', link: '/appointments/requests' },
          { label: 'Scheduled', link: '/appointments/scheduled' },
          { label: 'Slot-wise', link: '/appointments/slots' },
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
        label: 'Dashboard',
        link: '/patient/dashboard'
      },
      {
        label: 'Appointments',
        children: [
          { label: 'Book Appointment', link: '/appointments/book' },
          { label: 'Active', link: '/appointments/active' },
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
