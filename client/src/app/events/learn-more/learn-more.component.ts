import { Component } from '@angular/core';
import { OnInit } from '@angular/core';
import { EventService } from '../../_services/event.service';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { NgIf } from '@angular/common';
import { Router } from '@angular/router';
import { TicketService } from '../../_services/ticket.service';

@Component({
  selector: 'app-learn-more',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './learn-more.component.html',
  styleUrl: './learn-more.component.css'
})
export class LearnMoreComponent implements OnInit{
  eventDetails: any;
  receiptPhoto: string = '';
  uploadError: string = '';
  isSubmitting: boolean = false;


  constructor(private service: EventService, private route: ActivatedRoute, private router: Router, private ticketService: TicketService ){}

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const eventId = +params.get('id')!;
      this.loadEvent(eventId);
    });
  }

  loadEvent(eventId: number){
    this.service.getAnEvent(eventId).subscribe({
      next: (data) => this.eventDetails = data,
      error: (err) => console.error('Console not found: ', err)
    });
  }

  goBackToEvents(){
    this.router.navigate(['/events']);
  }

  onFileSelected(event: any): void {
    const file: File = event.target.files[0];
    this.uploadError = '';
  
    if (!file) return;
  
    if (file.type !== 'image/jpeg') {
      this.uploadError = 'Only JPEG files are allowed.';
      return;
    }
  
    if (file.size > 5 * 1024 * 1024) {
      this.uploadError = 'File size should be under 5MB.';
      return;
    }
  
    const reader = new FileReader();
    reader.onload = () => {
      this.receiptPhoto = reader.result as string;
    };
    reader.readAsDataURL(file);
  }


  submitTicket(): void {
    if(!this.receiptPhoto){
      this.uploadError = 'Please upload a receipt photo!';
      return;
    }

    const eventId = Number(this.route.snapshot.paramMap.get('id'));
    const ticketPayload = {
      eventId,
      receiptPhoto: this.receiptPhoto
    };

    this.isSubmitting = true;

    this.ticketService.createTicket(ticketPayload).subscribe({
      next: () => {
        alert('Ticket request submitted!');
        this.receiptPhoto = '';
        this.isSubmitting = false;
      },

      error: (err) => {
        console.error(err);
        alert('Failed to submit ticket.');
        this.isSubmitting = false;
      }
    });
  }
  

}
