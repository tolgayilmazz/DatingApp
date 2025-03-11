import { Component, OnInit } from '@angular/core';
import { RoleService } from '../_services/role.service';
import { SuperAdminService } from '../_services/super-admin.service';
import { ToastrService } from 'ngx-toastr';
import { NgFor } from '@angular/common';
import { CommonModule } from '@angular/common';
import { NavBarComponent } from "./nav-bar/nav-bar.component";
import { NgModule } from '@angular/core';
import { NgModel } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { User } from '../_models/user';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-super-admin',
  standalone: true,
  imports: [NgFor, CommonModule, NavBarComponent, FormsModule],
  templateUrl: './super-admin.component.html',
  styleUrl: './super-admin.component.css'
})
export class SuperAdminComponent implements OnInit{

  users: any[] = [];
  adminsWithClubs: any[] = [];
  clubsWithAdmins: any[] = [];

  constructor(
      private roleService: RoleService, 
      private toastr: ToastrService, 
      private superAdminService: SuperAdminService,
      private accountService: AccountService,
      private cdr: ChangeDetectorRef){}

  

  ngOnInit(): void {

    this.checkAccess();
    this.loadUsers();
    this.accountService.refreshToken().subscribe({
      next: (newUser: User|null) => {
        if(newUser?.token){
          localStorage.setItem('token', newUser.token);
        }
        
        setTimeout(() => {
          this.loadAdminsWithClubs();
          this.loadClubsWithAdmins();
          
        }
        , 1000);
      },
      error: () => {
        this.toastr.error("Session expired. Please log in again.");
        this.accountService.logout();
      }
    });
  }

  checkAccess(): void{
    this.roleService.manageAdmins().subscribe({
      next: (response) => {
        console.log('access grented to superadmin', response);
        this.toastr.success('Access granted to the superadmin');
      },
      error: (error) => {
        console.log('Access Failed', error);
        this.toastr.error('You do not have permission to access this resource!');
      }

    });
  }

  loadUsers(): void{
    this.superAdminService.getAllUsers().subscribe((data) => {
      console.log('bdjvh');
      this.users = data;
      this.cdr.detectChanges();
    });
  }

  loadClubsWithAdmins(): void{
    this.superAdminService.getClubsWithAdmins().subscribe((data) => {
      this.clubsWithAdmins = data;
    });
  }

  loadAdminsWithClubs(): void{
    this.superAdminService.getAdminsWithClubs().subscribe((data) => {
      this.adminsWithClubs = data;
    });
  }

  changeRole(userId: number, newRole: string): void{
    this.superAdminService.updateUserRole(userId, newRole).subscribe({
      next: (response) => {
        this.toastr.success('User role updated successfully');


        if(response?.newAccessToken){
          localStorage.setItem('token', response.newAccessToken);
          this.toastr.info('session updated');
        }
        
        console.log('loading users');
        this.loadUsers();
        
        

      },
      error: (error) => {
        if(error.status == 401){
          this.toastr.error('You do not have permission to access this resource');
          localStorage.removeItem('token');
          window.location.href = '/login';
        }
        else{
          this.toastr.error('Failed to update user role');
          console.error('Failed to update user role', error);
        }
      }
    });
  }

  assignAdmintoClub(adminId: number, clubId: number): void{
    this.superAdminService.assignAdminToTheClubs(adminId, clubId).subscribe(() => {
      this.loadAdminsWithClubs();
      this.loadClubsWithAdmins
      this.toastr.success('Admin assigned to the club successfully');
    });
  }

  updateAdminClubs(adminId: number, clubs: any[]): void{
    this.superAdminService.updateAdminClubs(adminId, clubs).subscribe(() => {
      this.loadAdminsWithClubs();
      this.loadClubsWithAdmins();
      this.toastr.success('Admin clubs updated successfully');
    });
  }

  




}
