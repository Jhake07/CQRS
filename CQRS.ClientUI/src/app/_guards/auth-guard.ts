import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { inject } from '@angular/core';

export const authGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const router = inject(Router);

  const user = accountService.getStoredUser();

  if (user) {
    router.navigate(['/batchserial']);
    return false;
  }
  return true;
};
