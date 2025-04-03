import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class EventService {

  constructor(private http: HttpClient) { }

  private baseUrl = 'http://localhost:5203/api/events';

  getEvents(): Observable<any[]> {
    const user = JSON.parse(localStorage.getItem('user')!);
    const token = user?.token;
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<any[]>(`${this.baseUrl}/get-events`, {headers});
  }

  deleteEvent(eventId: number): Observable<any>{
    const user = JSON.parse(localStorage.getItem('user')!);
    const token = user?.token;
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.delete(`${this.baseUrl}/delete-events/${eventId}`,{headers});
  }

  createEvent(eventData: any): Observable<any>{
    const user = JSON.parse(localStorage.getItem('user')!);
    const token = user?.token;
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.post(`${this.baseUrl}/create-events`, eventData, {headers});
  }
  
  getClubEvents(clubId: number): Observable<any[]>{
    const user = JSON.parse(localStorage.getItem('user')!);
    const token = user?.token;
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.get<any[]>(`${this.baseUrl}/get-club-events/${clubId}`, {headers});
  }

  updateEvents(eventId: number, updatedEvent: any): Observable<any>{
    const user = JSON.parse(localStorage.getItem('user')!);
    const token = user?.token;
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.put(`${this.baseUrl}/update-event/${eventId}`, updatedEvent, { headers });
  }

  getAnEvent(eventId: number): Observable<any>{
    const user = JSON.parse(localStorage.getItem('user')!);
    const token = user?.token;
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.get(`${this.baseUrl}/get-an-event/${eventId}`, {headers});
  }
}
