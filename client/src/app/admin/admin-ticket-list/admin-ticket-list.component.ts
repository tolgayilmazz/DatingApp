import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TicketService } from '../../_services/ticket.service';
import { CommonModule } from '@angular/common';
import { NgIf, NgFor } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Router } from '@angular/router';


@Component({
  selector: 'app-admin-ticket-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './admin-ticket-list.component.html',
  styleUrl: './admin-ticket-list.component.css'
})
export class AdminTicketListComponent implements OnInit {

  filter: 'pending' | 'approved' | 'rejected' = 'pending';
  eventId!: number;
  tickets: any[] = [];
  clubId!: number;
  previewImage: string | null = null;

  constructor(private service: TicketService, private route: ActivatedRoute, private router: Router) {}


  ngOnInit(): void {
    this.eventId = Number(this.route.snapshot.paramMap.get('eventId'));
    this.clubId = Number(this.route.snapshot.queryParamMap.get('clubId')); // capture ?clubId=xxx from route
    this.loadTickets();
  }

  loadTickets(): void {
    this.service.getEventBuyers(this.eventId).subscribe({
      next: (data) => {
        this.tickets = data;
      },
      error: (err) => {
        console.error('Failed to fetch tickets:', err);
      }
    });
  }

  approveTickets(ticketId: number): void {
    this.service.approveTicket(ticketId).subscribe({
      next: () => {
        const ticket = this.tickets.find(t => t.ticketId === ticketId);
        if (ticket){ 
          ticket.approved = true;
          ticket.rejected = false;
        }
        alert('Ticket approved succesfully!');
      },
      error: (err) => {
        console.error('Failed to approve ticket:', err);
        alert('Failed to approve ticket!');
      }
    });
  }

  rejectTickets(ticketId: number): void {
    if (confirm('Are you sure you want to reject this ticket?')) {
      this.service.rejectTicket(ticketId).subscribe({
        next: () => {
          const ticket = this.tickets.find(t => t.ticketId === ticketId);
          if (ticket) {
            ticket.rejected = true;
            ticket.approved = false;
          }
          alert('Ticket rejected successfully!');
        },
        error: (err) => {
          console.error('Failed to reject ticket:', err);
          alert('Failed to reject ticket!');
        }
      });
    }
  }

  resetTicket(ticketId: number): void {
    if (confirm("Reset this ticket to pending status?")) {
      this.service.resetTicket(ticketId).subscribe({
        next: () => {
          const ticket = this.tickets.find(t => t.ticketId === ticketId);
          if (ticket) {
            ticket.approved = false;
            ticket.rejected = false;
          }
          alert('Ticket status reset to pending.');
        },
        error: err => {
          console.error('Reset failed:', err);
          alert('Failed to reset ticket status!');
        }
      });
    }
  }
  

  goBackToAdmin(): void {
    this.router.navigate(['/admin']);
  }

  showPreview(imageUrl: string): void {
    this.previewImage = imageUrl;
  }

  closePreview(): void {
    this.previewImage = null;
  }

  get filteredTickets() {
    return this.tickets.filter(ticket => {
      if (this.filter === 'pending') return !ticket.approved && !ticket.rejected;
      if (this.filter === 'approved') return ticket.approved;
      if (this.filter === 'rejected') return ticket.rejected;
      return false;
    });
  }

}
