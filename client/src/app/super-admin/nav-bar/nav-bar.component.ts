import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { SuperAdminComponent } from '../super-admin.component';
import { AdminsAndClubsComponent } from '../admins-and-clubs/admins-and-clubs.component';

import { TitleCasePipe } from '@angular/common';

@Component({
  selector: 'app-nav-bar',
  standalone: true,
  imports: [],
  templateUrl: './nav-bar.component.html',
  styleUrl: './nav-bar.component.css'
})
export class NavBarComponent {
  private router = inject(Router);

  routeToSuperAdmin(): void {
    console.log('abc')
    this.router.navigate(['/super-admin']);
  }
  routeToAdminClub(): void {
    console.log('def')
    this.router.navigate(['/admins-clubs']);
  }

}
