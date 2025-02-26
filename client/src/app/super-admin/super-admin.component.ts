import { Component, OnInit } from '@angular/core';
import { RoleService } from '../_services/role.service';
import { ToastrService } from 'ngx-toastr';


@Component({
  selector: 'app-super-admin',
  standalone: true,
  imports: [],
  templateUrl: './super-admin.component.html',
  styleUrl: './super-admin.component.css'
})
export class SuperAdminComponent implements OnInit{
  constructor(private roleService: RoleService, private toastr: ToastrService){}

  ngOnInit(): void {
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
}
