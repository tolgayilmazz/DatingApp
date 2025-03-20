import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class EventService {

  constructor(private http: HttpClient) { }

  private baseUrl = 'http://localhost:5203/api/events';

  getEvents(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/get-events`);
  }

  deleteEvent(eventId: number): Observable<any>{
    return this.http.delete(`${this.baseUrl}/delete-events/${eventId}`);
  }

  createEvent(eventData: any): Observable<any>{
    return this.http.post(`${this.baseUrl}/create-events`, eventData);
  } 
}
