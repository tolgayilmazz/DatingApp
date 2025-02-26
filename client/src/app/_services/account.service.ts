import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { User } from '../_models/user';
import { catchError, map, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  private http = inject(HttpClient);
  baseUrl = 'http://localhost:5203/api/';
  currentUser = signal<User | null>(null);

  constructor() {
    const storedUser = localStorage.getItem('user');
    if(storedUser){
      this.currentUser.set(JSON.parse(storedUser));
    }
  }

  login(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map(user => {
        if (user) {
          localStorage.setItem('user',JSON.stringify(user));
          this.currentUser.set(user);
        }
        return user;
      })
    )
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUser.set(null);
  }

  register(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map(user => {
        if (user) {
          localStorage.setItem('user',JSON.stringify(user));
          this.currentUser.set(user);
        }
        return user;
      })
    )
  }

  refreshToken(){
    const refreshToken = this.getRefreshToken();
    if(!refreshToken){
      return of(null);
    }
    return this.http.post<User>(this.baseUrl + 'account/refresh-token', {refreshToken}).pipe(
      map((newUser : User) => {
        if(newUser) {
          this.setUser(newUser);
          return newUser;
        }
        return null;
      }),
      catchError((error) => {
        console.error('Refresh token failed.' , error);
        return of(null);
      })
    )
  }

  getAccessToken(): string| null{
    const user = this.currentUser();
    return user?.token || null;
  }

  getRefreshToken(): string| null{
    const user = this.currentUser();
    return user?.refreshToken || null;
  }

  private setUser(user: User){
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUser.set(user);
  }

  get isSuperAdmin(): boolean{
    return this.currentUser()?.role === 'SuperAdmin';
  }

  get isAdmin(): boolean{
    return this.currentUser()?.role === 'Admin';
  }

  
}
