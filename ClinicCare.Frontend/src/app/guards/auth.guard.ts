import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';

//route - activated route snapshot
//state - router state snapshot
export const authGuard: CanActivateFn = (route, state) => {
  const auth = inject(AuthService);

  if(auth.isLoggedIn()) {
    return true;
  }
  else{
    auth.navigateToUrl('/');
    return false;
  }
};
