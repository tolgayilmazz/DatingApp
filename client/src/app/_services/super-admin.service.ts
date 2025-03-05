import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HttpHeaders } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class SuperAdminService {

  private baseUrl = 'http://localhost:5203/api';

  constructor(private http: HttpClient) { }

  getAllAdminsWithUsers(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/admin/admins-with-users`);
  }

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

  assignAdminToTheClubs(adminId: number, clubId: number): Observable<any> {
    return this.http.post(`${this.baseUrl}/admin/add-admin-to-club`, {adminId, clubId});
  }

  updateAdminClubs(adminId: number, clubs: any[]): Observable<any> {
    return this.http.put(`${this.baseUrl}/admin/update-admin-clubs`, {adminId, clubs});
  }

  deleteAdmin(adminId: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/admin/delete-admin/${adminId}`);
  }
}
