import { inject } from '@angular/core';
import { CanMatchFn } from '@angular/router';
import { AuthService } from '../shared/services/auth.service';

export const adminOnlyGuard: CanMatchFn = (route, segments) => {
  const authService = inject(AuthService);

  if(authService.role == 'Admin'){
    return true;
  }
  else{
    authService.navigateToUrl('/');
    return false;
  }
};
