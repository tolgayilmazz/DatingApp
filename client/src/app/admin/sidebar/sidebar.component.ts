import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { OnInit } from '@angular/core';
import { NgFor } from '@angular/common';
import { CommonModule } from '@angular/common';
import { RouterLinkActive } from '@angular/router';
import { HttpHeaders } from '@angular/common/http';

interface Club{
  clubId: number;
  clubName: string;
  logoUrl: string;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css'
})
export class SidebarComponent implements OnInit{
  clubs: Club[] = [];

  private baseUrl = 'http://localhost:5203/api';

  constructor(private http: HttpClient, private router: Router){}

  ngOnInit(): void{

    const user = JSON.parse(localStorage.getItem('user')!);
    const token = user?.token;
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    this.http.get<Club[]>(`${this.baseUrl}/admin/my-clubs`, {headers}).subscribe(
      (data) => {
        this.clubs = data;
      },
      (error) => {
        console.error('error fetching clubs.', error);
      }
    );
  }

  navigateToClub(clubId: number){
    this.router.navigate([`/admin/club`, clubId])
  }
}
