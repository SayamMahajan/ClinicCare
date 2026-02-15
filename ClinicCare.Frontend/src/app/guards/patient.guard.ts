import { CanActivateChildFn } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const patientGuard: CanActivateChildFn = (childRoute, state) => {
  const authService = inject(AuthService);

  if(authService.role == 'Patient'){
    return true;
  }
  else{
    return false;
  }
};
