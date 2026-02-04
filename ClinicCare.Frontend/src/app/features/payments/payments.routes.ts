import { Routes } from '@angular/router';
import { DashboardLayoutComponent } from '../../shared/components/dashboard-layout/dashboard-layout.component';

export const PAYMENTS_ROUTES: Routes = [
  {
    path: '',
    component: DashboardLayoutComponent,
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./payments.component')
            .then(c => c.PaymentsComponent),  
      }
    ]
  },
];