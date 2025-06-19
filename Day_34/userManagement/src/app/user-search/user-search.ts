import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BehaviorSubject, combineLatest, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, catchError, tap } from 'rxjs/operators';
import { UserListComponent } from '../user-list/user-list';
import { User } from '../models/userModel';
import { UserService } from '../services/userService';


@Component({
  selector: 'app-user-search',
  standalone: true,
  imports: [CommonModule, UserListComponent],
  templateUrl: './user-search.html',
  styleUrl: './user-search.css'
})
export class UserSearchComponent {
  users: User[] = [];
  roles: string[] = ['admin', 'user', 'guest']; 

  isLoading = false;
  hasError = false;

  currentSearch = '';
  currentRole = '';

  private searchQuery$ = new BehaviorSubject<string>('');
  private roleFilter$ = new BehaviorSubject<string>('');

  constructor(private userService: UserService) {
    combineLatest([
      this.searchQuery$.pipe(debounceTime(500), distinctUntilChanged()),
      this.roleFilter$,
      this.userService.users$
    ])
      .pipe(
        tap(() => {
          this.isLoading = true;
          this.hasError = false;
        }),
        map(([search, role, users]) => {
          return users.filter(u =>
            u.username.toLowerCase().includes(search.toLowerCase()) &&
            (role === '' || u.role.toLowerCase() === role.toLowerCase())
          );
        }),
        tap(() => this.isLoading = false),
        catchError(() => {
          this.hasError = true;
          this.isLoading = false;
          return of([]);
        })
      )
      .subscribe(filtered => {
        this.users = filtered;
      });
  }

  onSearch(event: Event): void {
    const value = (event.target as HTMLInputElement).value ?? '';
    this.currentSearch = value;
    this.searchQuery$.next(value);
  }

  onRoleChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    this.currentRole = value;
    this.roleFilter$.next(value);
  }
}
