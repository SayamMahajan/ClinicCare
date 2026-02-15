import { Routes } from '@angular/router';
import { DashboardLayoutComponent } from '../../shared/components/dashboard-layout/dashboard-layout.component';

export const PRESCRIPTIONS_ROUTES: Routes = [
  {
      path: '',
      component: DashboardLayoutComponent,
      children: [
        {
          path: '',
          loadComponent: () =>
            import('./prescription.component')
              .then(c => c.PrescriptionListComponent),  
        }
      ]
    },
];