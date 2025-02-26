import { HttpInterceptorFn, HttpRequest, HttpHandlerFn, HttpErrorResponse, HttpEvent } from '@angular/common/http';
import { NavigationExtras, Router } from '@angular/router';
import { inject, Inject, model } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';
import { catchError, throwError, switchMap, of, Observable, EMPTY } from 'rxjs';
import { User } from '../_models/user';

export const errorInterceptor: HttpInterceptorFn = (req: HttpRequest<any>, next: HttpHandlerFn): Observable<HttpEvent<any>> => {

  const router = inject(Router);
  const toastr = inject(ToastrService);
  const accountService = inject(AccountService);

  let isRefreshing = false;

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if(error){
        switch(error.status) {
          case 400:
            
            if(error.error.errors){
              const modalStateErrors = [];
              for (const key in error.error.errors){
                if(error.error.errors[key]){
                  modalStateErrors.push(error.error.errors[key]);
                }
              }
              throw modalStateErrors.flat();
            }
            else{
              toastr.error(error.error, error.status.toString());
            }
            break;
          case 401:
            if(!isRefreshing){
              isRefreshing = true;

              return accountService.refreshToken().pipe(
                switchMap((newUser: User | null) => {
                  isRefreshing = false;
                  if(newUser?.token) {
                    const clonedRequest = req.clone({
                      setHeaders: {
                        Authorization: `Bearer ${newUser.token}` 
                      }
                    });
                    return next(clonedRequest);
                  }
                  else{
                    toastr.error('Session expired!!! Please log in again');
                    accountService.logout();
                    router.navigateByUrl('/login');
                    return EMPTY;
                  }
                }),
                catchError(refreshError => {
                  isRefreshing = false;
                  toastr.error('Session expired. Please log in again.')
                  accountService.logout();
                  router.navigateByUrl('/login');
                  return throwError(() => refreshError);
                })

              );

            }
            else{
              toastr.error('Unauthorised', error.status.toString());
              return EMPTY;
            }
            
            break;

          case 404:
            router.navigateByUrl('/not-found');
            break;
          case 500:
            const navigationExtras: NavigationExtras = {state: {error: error.error}};
            router.navigateByUrl('/server-error', navigationExtras);
            break;
          default:
            toastr.error('Something unexpected went wrong');
            break;
        }
      }
      return throwError(() => error);
    })
  )
};
