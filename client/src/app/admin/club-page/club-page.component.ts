import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { NgIf } from '@angular/common';
import { NgFor } from '@angular/common';
import { EventService } from '../../_services/event.service';
import { NgModel } from '@angular/forms';
import { FormsModule } from '@angular/forms';
interface Club{
  clubId: number;
  clubName: string;
  logoUrl: string;
  description?: string;
}


@Component({
  selector: 'app-club-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
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
  clubs: Club[] = [];

  private baseUrl = 'http://localhost:5203/api';


  constructor(private route: ActivatedRoute, private http: HttpClient, private eventService: EventService){}

  ngOnInit(): void{
    this.route.paramMap.subscribe(params => {
      this.clubId = Number(params.get('id'));

      this.http.get<Club[]>(`${this.baseUrl}/admin/my-clubs`).subscribe(
        (data) => {
          this.clubs = data;
  
          this.clubDetails = this.clubs.find(club => club.clubId === this.clubId);
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



  
}
