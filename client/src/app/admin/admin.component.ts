import { Component, OnInit } from '@angular/core';
import { RoleService } from '../_services/role.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [],
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.css'
})
export class AdminComponent implements OnInit {
  constructor(private roleService: RoleService, private toastr: ToastrService){}

  ngOnInit(): void {
    this.roleService.getClubInfo().subscribe({
      next: (response) => {
        console.log('Acces granted to Admin.', response);
        this.toastr.success(response);      
      },
      error: (error) => {
        console.log('Access denied.' , error);
        if (typeof error === 'string'){
          this.toastr.error(error);
        }
        else{
          this.toastr.error("You do not have permission to access this resource!!");
        }
      }
    });
  }
}
