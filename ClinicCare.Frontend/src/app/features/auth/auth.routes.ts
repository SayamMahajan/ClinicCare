import { Routes } from '@angular/router';

export const AUTH_ROUTES: Routes = [
  {
    path: 'login/:type',
    loadComponent: () =>
      import('./login/login-page/login-page.component')
        .then(c => c.LoginPageComponent)
  },
  {
    path: 'register/',
    loadComponent: () =>
      import('./register/register-page/register-page.component')
        .then(c => c.RegisterPageComponent)
  },
  {
    path: '',
    redirectTo: 'login/patient',
    pathMatch: 'full'
  }
];
