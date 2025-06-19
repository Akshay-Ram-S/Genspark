import { Routes } from '@angular/router';
import { Home } from './home/home';
import { UserFormComponent } from './user-form/user-form';
import { UserSearchComponent } from './user-search/user-search';
import { UserListComponent } from './user-list/user-list';

export const routes: Routes = [
    {
    path: '',
    component: Home,
    children: [
      { path: 'add', component: UserFormComponent },
      { path: 'search', component: UserSearchComponent },
      { path: '', redirectTo: 'add', pathMatch: 'full' }
    ]
  }
];
