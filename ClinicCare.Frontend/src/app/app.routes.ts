import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';
import { adminOnlyGuard } from './guards/admin-only.guard';
import { LandingPage } from './features/landing-page/landing-page';

export const routes: Routes = [
  {
    path: '',
    component: LandingPage,
    pathMatch: 'full'
  },
  {
    path: 'auth',
    loadChildren: () => import('./features/auth/auth.routes').then((m) => m.AUTH_ROUTES),
  },
  {
    path: 'admin',
    loadChildren: () =>
      import('./features/dashboard/dashboard.routes').then((m) => m.DASHBOARD_ROUTES),
    canActivate: [authGuard],
    canMatch: [adminOnlyGuard],
  },
  {
    path: 'employees',
    loadChildren: () =>
      import('./features/employees/employee.routes').then((m) => m.EMPLOYEES_ROUTES),
    canActivate: [authGuard],
  },
  {
    path: 'payments',
    loadChildren: () =>
      import('./features/payments/payments.routes').then((m) => m.PAYMENTS_ROUTES),
    canActivate: [authGuard],
  },
  {
    path: 'prescriptions',
    loadChildren: () =>
      import('./features/prescription/prescription.routes').then((m) => m.PRESCRIPTIONS_ROUTES),
    canActivate: [authGuard],
  },
  {
    path: 'appointments',
    loadChildren: () =>
      import('./features/appointment/appointment.routes').then((m) => m.APPOINTMENTS_ROUTES),
    canActivate: [authGuard],
  },
  {
    path: 'my-profile',
    loadChildren: () =>
      import('./features/myprofile/myprofile.routes').then((m) => m.MYPROFILE_ROUTES),
    canActivate: [authGuard],
  },
];
