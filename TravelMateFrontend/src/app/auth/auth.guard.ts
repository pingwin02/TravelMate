import { map, take } from 'rxjs/operators';
import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from './service/auth.service';

export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.isLoggedIn().pipe(
    take(1),
    map((isLoggedIn: boolean) => {
      if (!isLoggedIn) {
        localStorage.setItem('redirectUrl', router.url);
        router.navigate(['/login']);
        return false;
      }
      return true;
    })
  );
};
