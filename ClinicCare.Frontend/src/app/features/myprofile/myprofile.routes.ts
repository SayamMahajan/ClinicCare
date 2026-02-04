import { Routes } from '@angular/router';

export const MYPROFILE_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./myprofile.component')
        .then(c => c.MyProfileComponent),  
  },
];