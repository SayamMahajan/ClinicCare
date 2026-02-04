import { Routes } from '@angular/router';
import { DashboardLayoutComponent } from '../../shared/components/dashboard-layout/dashboard-layout.component';

export const EMPLOYEES_ROUTES: Routes = [
  {
    path: '',
    component: DashboardLayoutComponent,
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./manage-employees/manage-employees.component')
            .then(c => c.ManageEmployeesComponent),  
      }
    ]
  },
];
