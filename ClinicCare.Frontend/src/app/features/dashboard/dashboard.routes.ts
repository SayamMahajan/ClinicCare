import { Routes } from '@angular/router';

export const DASHBOARD_ROUTES: Routes = [
  {
    path: 'admin',
    loadComponent: () =>
      import('./layout/dashboard-layout.component')
        .then(c => c.DashboardLayoutComponent),
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
    path: 'doctor/dashboard',
    loadComponent: () =>
      import('./layout/dashboard-layout.component')
        .then(c => c.DashboardLayoutComponent),
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
    path: 'patient/dashboard',
    loadComponent: () =>
      import('./layout/dashboard-layout.component')
        .then(c => c.DashboardLayoutComponent),
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
