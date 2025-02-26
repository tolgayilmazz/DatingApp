import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

export const adminGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const router = inject(Router);
  const toastr = inject(ToastrService);

  if (accountService.isAdmin || accountService.isSuperAdmin){
    return true;
  }
  else{
    toastr.error("Access Denied!");
    router.navigate(["/"]);
    return false;
  }
};
