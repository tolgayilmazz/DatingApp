import { Component, inject } from '@angular/core';
import { EventService } from '../_services/event.service';
import { OnInit } from '@angular/core';
import { NgFor } from '@angular/common';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';


@Component({
  selector: 'app-events',
  standalone: true,
  imports: [ CommonModule, RouterModule],
  templateUrl: './events.component.html',
  styleUrl: './events.component.css'
})
export class EventsComponent implements OnInit{

  events: any[] = [];
  constructor(private service: EventService){}

  ngOnInit(): void {
    this.loadEvents();
  }

  loadEvents(): void{
    this.service.getEvents().subscribe({
      next: (data) => {
        this.events = data;
      },
      error: (err) => {
        console.error('Error loading events', err);
      }
    });
  }

}
