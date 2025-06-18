import { Routes } from '@angular/router';
import { UserAddComponent } from './user-add/user-add';
import { DashboardComponent } from './dashboard/dashboard';

export const routes: Routes = [
    {path:'add',component:UserAddComponent},
    {path:'dashboard',component:DashboardComponent}
];
