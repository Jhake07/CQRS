import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../_services/account.service';
export const loginGuard: CanActivateFn = (route, state) => {
  const account = inject(AccountService);
  const router = inject(Router);
  const user = account.getStoredUser();
  // ✔ If logged in, redirect to main page
  if (user) {
    return router.createUrlTree(['/batchserial']);
  }
  //  not logged in → allow login
  return true;
};
