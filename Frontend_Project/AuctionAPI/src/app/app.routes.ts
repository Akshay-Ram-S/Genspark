import { Routes } from '@angular/router';
import { LoginComponent } from './login-component/login-component';
import { RegisterComponent } from './register-component/register-component';
import { ProfileComponent } from './profile-component/profile-component';
import { ItemsComponent } from './items-component/items-component';

export const routes: Routes = [
    //{ path: '', component: HomeComponent },
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    { path: 'profile', component: ProfileComponent},
    {path: 'items', component: ItemsComponent}
];
