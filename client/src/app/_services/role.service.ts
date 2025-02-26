import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  private http = inject(HttpClient);
  private baseUrl = 'http://localhost:5203/api/role/';
  private accountService = inject(AccountService);


  manageAdmins(): Observable<any>{
    const token = this.accountService.getAccessToken();
    console.log('Access token for Super Admin: ', token);
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });

    console.log('Authorization Header For Super Admin: ', headers.get('Authorization'));
    return this.http.get(`${this.baseUrl}superadmin/manage-admins` , {headers, responseType: 'text'});
  }

  getClubInfo(): Observable<any>{
    const token = this.accountService.getAccessToken();
    console.log('Access Token for Admin: ', token);

    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });

    console.log('Authorization Header for Admin: ' , headers.get('Authorization'));

    return this.http.get(`${this.baseUrl}admin/club-info`, {headers, responseType: 'text'});
  }
}


