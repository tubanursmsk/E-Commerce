import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './core/services/authService';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isLoggedIn()) {
    return true; // Giriş yapılmışsa sayfaya izin ver
  } else {
    // Giriş yapılmamışsa login'e yönlendir ve gidilmek istenen sayfayı kaydet
    return router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url }});
  }
};