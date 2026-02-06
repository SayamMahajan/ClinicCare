import { CanActivateChildFn } from '@angular/router';
import { AuthService } from '../shared/services/auth.service';
import { inject } from '@angular/core';

export const patientGuard: CanActivateChildFn = (childRoute, state) => {
  const authService = inject(AuthService);

  if(authService.role == 'Patient'){
    return true;
  }
  else{
    return false;
  }
};
