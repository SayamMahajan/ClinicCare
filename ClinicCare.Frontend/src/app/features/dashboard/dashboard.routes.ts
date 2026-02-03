import { Routes } from '@angular/router';
import { DashboardLayoutComponent } from '../../shared/components/dashboard-layout/dashboard-layout.component';

export const DASHBOARD_ROUTES: Routes = [
  {
    path: 'admin',
    data: { role: 'Admin' },
    component: DashboardLayoutComponent,
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./admin-dashboard/admin-dashboard.component')
            .then(c => c.AdminDashboardComponent)
      }
    ]
  },
  {
    path: 'doctor',
    data: { role: 'Doctor' },
    component: DashboardLayoutComponent,
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./doctor-dashboard/doctor-dashboard.component')
            .then(c => c.DoctorDashboardComponent)
      }
    ]
  },
  {
    path: 'patient',
    data: { role: 'Patient' },
    component: DashboardLayoutComponent,
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./patient-dashboard/patient-dashboard.component')
            .then(c => c.PatientDashboardComponent)
      }
    ]
  }
];
