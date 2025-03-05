import { Component, inject } from '@angular/core';
import { SuperAdminComponent } from '../super-admin.component';
import { NavBarComponent } from "../nav-bar/nav-bar.component";
import { RoleService } from '../../_services/role.service';
import { ToastrService } from 'ngx-toastr';
import { OnInit } from '@angular/core';
import { SuperAdminService } from '../../_services/super-admin.service';
import { ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgModel } from '@angular/forms'; 
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';


@Component({
  selector: 'app-admins-and-clubs',
  standalone: true,
  imports: [NavBarComponent, CommonModule, FormsModule],
  templateUrl: './admins-and-clubs.component.html',
  styleUrl: './admins-and-clubs.component.css'
})
export class AdminsAndClubsComponent implements OnInit {

  users: any[] = [];
  adminsWithClubs: any[] = [];
  clubsWithAdmins: any[] = [];
  selectedAdminId: number | null = null;
  selectedClubId: number | null = null;


  private roleService = inject(RoleService);
  private toastr = inject(ToastrService);
  private superAdminService = inject(SuperAdminService);
  private cdr = inject(ChangeDetectorRef);

  ngOnInit(): void {
    this.checkAccess();
    this.loadAdminsWithClubs();
    this.loadClubsWithAdmins();
    this.loadUsers();
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

  assignAdmintoClub(adminId: number, clubId: number): void{
    if (this.selectedAdminId && this.selectedClubId){
    this.superAdminService.assignAdminToTheClubs(this.selectedAdminId, this.selectedClubId).subscribe(() => {
      this.loadAdminsWithClubs();
      this.loadClubsWithAdmins();
      this.toastr.success('Admin assigned to the club successfully');
    });
  }
  }


  updateAdminClubs(adminId: number, clubs: number[]): void{
    this.superAdminService.updateAdminClubs(adminId, clubs).subscribe(() => {
      this.loadAdminsWithClubs();
      this.loadClubsWithAdmins();
      this.toastr.success('Admin clubs updated successfully');
    });
  }

  loadUsers(): void{
    this.superAdminService.getAllUsers().subscribe((data) => {
      console.log('bdjvh');
      this.users = data;
      this.cdr.detectChanges();
    });
  }

  deleteAdmin(adminId: number): void{
    if(confirm('Are you sure you want to delete this admin?')){
      this.superAdminService.deleteAdmin(adminId).subscribe(() => {
        this.loadAdminsWithClubs();
        this.loadClubsWithAdmins();
        this.loadUsers();
        this.toastr.success('Admin deleted successfully');
      });
    }
  }

}
