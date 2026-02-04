import { Routes } from '@angular/router';
import { DashboardLayoutComponent } from '../../shared/components/dashboard-layout/dashboard-layout.component';

export const DASHBOARD_ROUTES: Routes = [
  {
    path: 'dashboard',
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
];
