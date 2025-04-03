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
  adminsWithUsers: any[] = [];
  clubs: any[] = [];
  newClubName: string = '';
  newLogoUrl: string = '';
  newDescription: string = '';
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
    this.loadAdminsWithUsers();
    this.loadUsers();
    this.loadClubs();
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
      this.adminsWithClubs = data.map(admin => ({
        ...admin,
        selectedClubId: null
      }));
    });
  }

  loadAdminsWithUsers(): void{
    this.superAdminService.getAdminsWithUsers().subscribe((data) => {
      this.adminsWithUsers = data.map(admin => ({
        ...admin,
        selectedAdminId: null
      }));
    });
  }

  debugClubSelection(admin: any): void {
    console.log(`Admin ID: ${admin.adminId}, Selected Club ID: ${admin.selectedClubId}`);
  }
  

  assignAdmintoClub(adminId: number, clubId: number | null): void{
    if(!clubId){
      this.toastr.error('Please select a club');
      return;
    }

    clubId = Number(clubId);
    const assignedClubs = this.clubsWithAdmins.find(a => a.adminId === adminId)?.clubs || [];
    if(assignedClubs.some((k: {clubId: number}) => k.clubId === clubId)){
      this.toastr.error('Admin is already assigned to this club');
      return;
    }

    this.superAdminService.assignAdminToTheClubs(adminId, clubId).subscribe(() => {
      this.loadAdminsWithClubs();
      this.loadClubsWithAdmins();
      this.toastr.success('Admin assigned to the club successfully');
    }
    );
  }
  


  updateAdminClubs(adminId: number, clubs: number[]): void{
    if(!clubs || clubs.length === 0){
      this.toastr.error('Please select at least one club');
      return;
    }
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

  

  loadClubs(): void{
    this.superAdminService.getAllClubs().subscribe((data) => {
      this.clubs = data;
    });
  }

  addClub(): void{
    if(!this.newClubName.trim()){
      this.toastr.error('Club name is required');
      return;
    }

    const clubData = {
      clubName: this.newClubName,
      logoUrl: this.newLogoUrl,
      description: this.newDescription
    };

    this.superAdminService.addClub(clubData).subscribe((newClub) => {
      this.clubs.push(newClub);
      this.toastr.success('Club added successfully');
      this.newClubName = '';
      this.newLogoUrl = '';
      this.newDescription = '';
    });
  }

  deleteAdminClub(adminId: number): void{
    if (!confirm("Are you sure you want to delete this admin's club?")){
      return;
    }
    this.superAdminService.deleteAdminClub(adminId).subscribe(() => {
      this.adminsWithUsers = this.adminsWithUsers.map(admin => 
        admin.adminId === adminId ? {...admin, users: []} : admin
      );
      this.toastr.success('Admin club deleted successfully');
      this.loadAdminsWithClubs();
    },
    error => {
      this.toastr.error('Failed to delete admin club');
      console.error('Failed to delete admin club', error);
    }
    );
  }

  deleteClub(clubId: number): void{
    if (confirm('Are you sure you want to delete this club?')) {
      this.superAdminService.deleteClub(clubId).subscribe({
        next: (response) => {
          console.log('Club deleted:', response);
          alert('Club deleted successfully.');
          this.loadClubs(); 
          this.loadAdminsWithClubs();
        },
        error: (error) => {
          console.error('Error deleting club:', error);
          alert('Failed to delete club.');
        }
      });
    }
  }

  onLogoSelected(event: any): void {
    const file: File = event.target.files[0];
  
    if (!file) return;
  
    
    if (file.type !== 'image/jpeg') {
      alert('Only JPEG files are allowed.');
      event.target.value = '';
      return;
    }
  
    
    const maxSizeInMB = 5;
    if (file.size > maxSizeInMB * 1024 * 1024) {
      alert('Image size should not exceed 5MB.');
      event.target.value = '';
      return;
    }
  
    
    const reader = new FileReader();
    reader.onload = () => {
      this.newLogoUrl = reader.result as string;
    };
    reader.readAsDataURL(file);
  }
    
}
