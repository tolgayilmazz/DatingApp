import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { EventsComponent } from './events/events.component';
import { authGuard } from './_guards/auth.guard';
import { TestErrorsComponent } from './errors/test-errors/test-errors.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { SuperAdminComponent } from './super-admin/super-admin.component';
import { superAdminGuard } from './_guards/super-admin.guard';
import { AdminComponent } from './admin/admin.component';
import { adminGuard } from './_guards/admin.guard';
import { AdminsAndClubsComponent } from './super-admin/admins-and-clubs/admins-and-clubs.component';
import { ClubPageComponent } from './admin/club-page/club-page.component';
import { LearnMoreComponent } from './events/learn-more/learn-more.component';
import { MyTicketsComponent } from './my-tickets/my-tickets.component';
import { AdminTicketListComponent } from './admin/admin-ticket-list/admin-ticket-list.component';

export const routes: Routes = [
    {path: '', component: HomeComponent},
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [authGuard],
        children: [
            
            
            {path: 'events', component: EventsComponent},
            { path: 'events/:id/learn-more', component: LearnMoreComponent },
            {path: 'my-tickets', component: MyTicketsComponent},
        ]
    },
    {
        path: 'admin',
        component: AdminComponent,
        canActivate: [adminGuard],
        children: [
            {path: 'club/:id', component: ClubPageComponent},
        ]
    },
    {
        path: 'admin/event/:eventId/tickets',
        component: AdminTicketListComponent,
        canActivate: [adminGuard] 
    },
      
    
    {path: 'super-admin', component: SuperAdminComponent, canActivate: [superAdminGuard]},
    
    {path: 'errors', component: TestErrorsComponent},  
    {path: 'not-found', component: NotFoundComponent},
    {path: 'server-error', component: ServerErrorComponent},
    {path: 'admins-clubs', component: AdminsAndClubsComponent, canActivate: [superAdminGuard]},
    {path: '**', component: HomeComponent, pathMatch: 'full'},
];
