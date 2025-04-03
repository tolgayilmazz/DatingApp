import { Component } from '@angular/core';
import { OnInit } from '@angular/core';
import { TicketService } from '../_services/ticket.service';
import { CommonModule } from '@angular/common';
import { NgFor } from '@angular/common';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-my-tickets',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './my-tickets.component.html',
  styleUrl: './my-tickets.component.css'
})
export class MyTicketsComponent implements OnInit {

  myTickets: any[] = [];

  constructor(private service: TicketService) {}

  ngOnInit(): void {
    this.service.getMyTickets().subscribe({
      next: (tickets) => {
        this.myTickets = tickets;
      },
      error: (err) => {
        console.error('Failed to fetch tickets', err);
      }
    });
  }
}
