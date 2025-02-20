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

}
