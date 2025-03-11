import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HttpHeaders } from '@angular/common/http';
import { tap } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SuperAdminService {

  private baseUrl = 'http://localhost:5203/api';

  constructor(private http: HttpClient
              , private toastr: ToastrService) { }


  getAllUsers(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/users`);
  }

  getAdminsWithClubs(): Observable<any[]> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<any[]>(`${this.baseUrl}/admin/admins-with-clubs`, {headers});
  }

  getClubsWithAdmins(): Observable<any[]> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<any[]>(`${this.baseUrl}/admin/clubs-with-admins`, {headers});
  }

  updateUserRole(userId: number, newRole: string ): Observable<any> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.put(`${this.baseUrl}/account/update-role`, {userId, newRole}, {headers});
  }

  assignAdminToTheClubs(adminId: number, clubId: any): Observable<{success?: boolean; message?: string; error?: string}> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    

    const payload = {AdminId: adminId, ClubIds: [Number(clubId)]};
    return this.http.post<{ success?: boolean; message?: string; error?: string }>(`${this.baseUrl}/admin/add-admin-to-clubs`, payload, {headers})
      .pipe(tap (response => {console.log('apı response', response);
            if (response && response.message){
              this.toastr.success(response.message);
            }
          }),
          catchError(error => {
            console.error('Error:', error);

          if (error.status === 200 || error.status === 201){
            this.toastr.success('Admin assigned to the club successfully');
            return of(error);
          }

          this.toastr.error('Error while assigning admin to the club');
          return throwError(() => new Error(error.error || 'Error while assigning admin to the club'));
        })
      );
  }

  updateAdminClubs(adminId: number, clubs: any[]): Observable<any> {
    return this.http.put(`${this.baseUrl}/admin/update-admin-clubs`, {adminId, clubs});
  }

  

  getAllClubs(): Observable<any[]> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<any[]>(`${this.baseUrl}/club/get-clubs`, {headers});
  }

  addClub(clubData: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/club/add-club`, clubData);
  }

  getAdminsWithUsers(): Observable<any[]> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<any[]>(`${this.baseUrl}/admin/admins-with-users`, {headers});
  }

  deleteAdminClub(adminId: number): Observable<{success?: boolean; message?: string; error?: string}> {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.delete(`${this.baseUrl}/admin/delete-admin/${adminId}`, {headers}).pipe(tap((response: any) => {
      
      if(response && response.message){
        this.toastr.success(response.message);
      }
    }),
    catchError(error => {
      console.error('Error:', error);

      const errorMessage = error.error?.error || "Error while deleting admin-club relation";
      this.toastr.error(errorMessage);
      return throwError(() => new Error(errorMessage));
    }));


  }
  
}
