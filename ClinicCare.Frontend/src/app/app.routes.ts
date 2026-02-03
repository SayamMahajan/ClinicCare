import { Routes } from '@angular/router';
import { MyProfileComponent } from './features/myprofile/myprofile.component';

export const routes: Routes = [
  { path: 'auth', loadChildren: () => import('./features/auth/auth.routes').then(m => m.AUTH_ROUTES)},
  { path: 'dashboard', loadChildren: () => import('./features/dashboard/dashboard.routes').then(m => m.DASHBOARD_ROUTES)},
  { path: 'employees', loadChildren: () => import('./features/employees/employee.routes').then(m => m.EMPLOYEES_ROUTES)},
  { path: 'payments', loadChildren: () => import('./features/payments/payments.routes').then(m => m.PAYMENTS_ROUTES)},
  { path: 'appointments', loadChildren: () => import('./features/appointment/appointment.routes').then(m => m.APPOINTMENTS_ROUTES)},
  { path: 'my-profile', component: MyProfileComponent}
];
