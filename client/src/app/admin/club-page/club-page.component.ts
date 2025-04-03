import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { CommonModule, JsonPipe } from '@angular/common';
import { NgIf } from '@angular/common';
import { NgFor } from '@angular/common';
import { EventService } from '../../_services/event.service';
import { NgModel } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { HttpHeaders } from '@angular/common/http';
import { User } from '../../_models/user';
import { Router } from '@angular/router';
import { RouterModule } from '@angular/router';
interface Club{
  clubId: number;
  clubName: string;
  logoUrl: string;
  description?: string;
}


@Component({
  selector: 'app-club-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule ],
  templateUrl: './club-page.component.html',
  styleUrl: './club-page.component.css'
})
export class ClubPageComponent implements OnInit{
  eventData = {
    photo: '',
    eventName: '',
    eventDescription: '',
    eventDate: '',
    clubIds: [] as number[]
  };
  clubId: number | null = null;
  clubDetails: Club | undefined;
  selectedEventId: number | null = null;
  isUpdateMode: boolean = false;
  clubs: Club[] = [];
  events: any[] = [];
  

  private baseUrl = 'http://localhost:5203/api';


  constructor(private route: ActivatedRoute, private http: HttpClient, private eventService: EventService, private router: Router){}

  ngOnInit(): void{
    

    
    this.route.paramMap.subscribe(params => {
      this.clubId = Number(params.get('id'));
      const user = JSON.parse(localStorage.getItem('user')!);
      const token = user?.token;
      const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

      this.http.get<Club[]>(`${this.baseUrl}/admin/my-clubs`, {headers}).subscribe(
        (data) => {
          this.clubs = data;
  
          this.clubDetails = this.clubs.find(club => club.clubId === this.clubId);

          if (this.clubId !== null) {
            this.loadClubEvents(this.clubId);
          }
        },
        (error) => {
          console.error('Error fetching the clubs.', error);
        }
      );
    });

    
  }

  CreateEvent(){
    if(!this.eventData.eventName || !this.eventData.eventDescription || !this.eventData.eventDate){
      alert('Please fill all the fields! ');
      return;
    }

    if (!this.clubId){
      alert('No club Ids');
      return;
    }

    this.eventData.clubIds = [this.clubId];

    const formattedDate = new Date(this.eventData.eventDate).toISOString();
    const eventPayload = {...this.eventData, eventDate: formattedDate};

    this.eventService.createEvent(this.eventData).subscribe({
      next: () => {
        alert('Event created succesfully!');
        this.ResetForm();
        if (this.clubId !== null) {
          this.loadClubEvents(this.clubId);
        }
      },
      error: (errorr) => {
        console.error('Error creating event:', errorr);
        alert('Failed to create event');
      }

    })
  }

  ResetForm() {
    this.eventData = {
      photo: '',
      eventName: '',
      eventDescription: '',
      eventDate: '',
      clubIds: [this.clubId!] 
    };
  }

  loadClubEvents(clubId: number) {
    this.eventService.getClubEvents(clubId).subscribe({
      next: (data) => {
        this.events = data;
      },
      error: (errorr) => {
        console.error('Error fetching club events!', errorr);
      }
    });
  }

  deleteEvent(eventId: number): void {
    if (confirm('Are you sure you want to delete this event?')) {
      this.eventService.deleteEvent(eventId).subscribe({
        next: () => {
          if (this.clubId !== null) {
            this.loadClubEvents(this.clubId);
          }
        },
        error: (err) => {
          console.error('Failed to delete event', err);
        }
      });
    }
  }

  editEvent(event: any): void{
    this.selectedEventId = event.eventId;
    this.isUpdateMode = true;

    this.eventData = {
      photo: event.photo,
      eventName: event.eventName,
      eventDescription: event.eventDescription,
      eventDate: event.eventDate,
      clubIds: [this.clubId!]
    };
  }

  updateEvent(): void {
    if(!this.selectedEventId) return;

    const formattedDate = new Date(this.eventData.eventDate).toISOString();
    const updatedPayload = { ...this.eventData, eventDate: formattedDate };

    this.eventService.updateEvents(this.selectedEventId, updatedPayload).subscribe({
      next: () => {
        alert("Event Updated Succesfully!");
        this.loadClubEvents(this.clubId!);
        this.ResetForm();
        this.selectedEventId = null;
        this.isUpdateMode = false;
      },
      error: (err) => {
        console.error('Update failed:', err);
        alert('Failed to update event!');
      }
    });
  }

  onImageSelected(event: any, fileInput: HTMLInputElement): void {
    const file: File = event.target.files[0];
  
    if (!file) return;
  
    
    if (file.type !== 'image/jpeg') {
      alert('Only JPEG files are allowed.');
      fileInput.value = '';
      return;
    }
  
    
    const maxSizeInMB = 5;
    if (file.size > maxSizeInMB * 1024 * 1024) {
      alert('Image size should not exceed 5MB.');
      return;
    }
  
    
    const reader = new FileReader();
    reader.onload = () => {
      this.eventData.photo = reader.result as string;
    };
    reader.readAsDataURL(file);
  }
  
  goToTicketList(eventId: number) {
    this.router.navigate([`/admin/event/${eventId}/tickets`]);
  }
  



  
}
