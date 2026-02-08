import { Component, inject, signal } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
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

  private router = inject(Router);
  role = signal<Role>(this.authService.role as Role);

  isExpanded(item: any): boolean {
    if (item.children?.length) {
      return true;
    }
    return false
  }

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
      },
      {
        label: 'Prescriptions',
        route: '/prescriptions',
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
      },
      {
        label: 'Prescriptions',
        route: '/prescriptions',
      }
    ]
  };
}
