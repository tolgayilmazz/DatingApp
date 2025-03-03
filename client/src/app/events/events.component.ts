import { Component } from '@angular/core';
import { HotEventsComponent } from './hot-events/hot-events.component';

@Component({
  selector: 'app-events',
  standalone: true,
  imports: [HotEventsComponent],
  templateUrl: './events.component.html',
  styleUrl: './events.component.css'
})
export class EventsComponent {
  events = [
    { name: 'Concert A', date: '2025-03-10', location: 'Venue 1', price: 50, likes: 80, image: 'concert_a.jpg' },
    { name: 'Seminar B', date: '2025-04-15', location: 'Venue 2', price: 20, likes: 120, image: 'seminar_b.jpg' }
  ];
  hotEvents = this.events.filter(event => event.likes > 100);

  buyTicket(event: any) {
    console.log('Buying ticket for', event.name);
  }

  likeEvent(event: any) {
    event.likes++;
    this.hotEvents = this.events.filter(e => e.likes > 100);
  }

}
