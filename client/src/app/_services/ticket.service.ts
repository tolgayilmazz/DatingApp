import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';



@Injectable({
  providedIn: 'root'
})
export class TicketService {
  private baseUrl = 'http://localhost:5203/api/ticket';

  constructor(private http: HttpClient) { }

  private getAuthHeaders(): HttpHeaders {
    const user = JSON.parse(localStorage.getItem('user')!);
    const token = user?.token;
    return new HttpHeaders().set('Authorization', `Bearer ${token}`);
  }

  createTicket(ticketData: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/create`, ticketData, 
      {headers: this.getAuthHeaders()}
    );
  }

  getMyTickets(): Observable<any> {
    return this.http.get(`${this.baseUrl}/my-tickets`, {
      headers: this.getAuthHeaders()
    });
  }

  getEventBuyers(eventId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/event-buyers/${eventId}`, {
      headers: this.getAuthHeaders()
    });
  }

  approveTicket(ticketId: number): Observable<any> {
    return this.http.put(`${this.baseUrl}/approve/${ticketId}`, {}, {
      headers: this.getAuthHeaders()
    });
  }

  rejectTicket(ticketId:number): Observable<any> {
    return this.http.put(`${this.baseUrl}/reject/${ticketId}`, {}, {
      headers: this.getAuthHeaders()
    });
  }

  resetTicket(ticketId:number): Observable<any> {
    return this.http.put(`${this.baseUrl}/reset/${ticketId}`, {}, {
      headers: this.getAuthHeaders()
    });
  }

  deleteTicket(ticketId:number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/tickets/delete/${ticketId}`, {
      headers: this.getAuthHeaders()
    });
  }
}
