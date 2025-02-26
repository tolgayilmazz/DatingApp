import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

export const superAdminGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const router = inject(Router);
  const toastrservice = inject(ToastrService);

  if(accountService.isSuperAdmin){
    return true;
  }
  else{
    toastrservice.error('Access Denied!!');
    router.navigate(['/']);
    return false;
  }
};
