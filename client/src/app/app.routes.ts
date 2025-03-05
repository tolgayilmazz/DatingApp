import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { EventsComponent } from './events/events.component';
import { HotEventsComponent } from './events/hot-events/hot-events.component';
import { MessagesComponent } from './messages/messages.component';
import { authGuard } from './_guards/auth.guard';
import { TestErrorsComponent } from './errors/test-errors/test-errors.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { SuperAdminComponent } from './super-admin/super-admin.component';
import { superAdminGuard } from './_guards/super-admin.guard';
import { AdminComponent } from './admin/admin.component';
import { adminGuard } from './_guards/admin.guard';
import { AdminsAndClubsComponent } from './super-admin/admins-and-clubs/admins-and-clubs.component';

export const routes: Routes = [
    {path: '', component: HomeComponent},
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [authGuard],
        children: [
            {path: 'members', component: MemberListComponent},
            {path: 'members/details', component: MemberDetailComponent},
            {path: 'events', component: EventsComponent, canActivate: [authGuard]},
            {path: 'messages', component: MessagesComponent},
        ]
    },
    {path: 'super-admin', component: SuperAdminComponent, canActivate: [superAdminGuard]},
    {path: 'admin', component: AdminComponent, canActivate: [adminGuard]},
    {path: 'errors', component: TestErrorsComponent},
    {path: 'not-found', component: NotFoundComponent},
    {path: 'server-error', component: ServerErrorComponent},
    {path: 'admins-clubs', component: AdminsAndClubsComponent, canActivate: [superAdminGuard]},
    {path: '**', component: HomeComponent, pathMatch: 'full'},
];
