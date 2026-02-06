import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { finalize } from 'rxjs';
import { TokenService } from '../shared/services/token.service';
import { LoaderService } from '../shared/services/loader.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {

  const tokenService = inject(TokenService);
  const loader = inject(LoaderService);

  if (!req.url.includes('/login') && !req.url.includes('/register')) {
    loader.show();
  }

  const token = tokenService.get();

  if (token) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  return next(req).pipe(
    finalize(() => {
      loader.hide();
    })
  );
};
