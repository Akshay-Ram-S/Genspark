// user.service.ts
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { User } from '../models/userModel';

@Injectable({ providedIn: 'root' })
export class UserService {
  private usersSubject = new BehaviorSubject<User[]>([
  {
    username: 'johndoe',
    email: 'john@example.com',
    password: 'Test@123',
    role: 'admin'
  }
]);
  users$ = this.usersSubject.asObservable();

  addUser(user: User) {
    const currentUsers = this.usersSubject.getValue();
    this.usersSubject.next([...currentUsers, user]);
  }

  filterUsers(term: string): Observable<User[]> {
    const allUsers = this.usersSubject.getValue();
    const filtered = allUsers.filter(user =>
      user.username.toLowerCase().includes(term.toLowerCase()) ||
      user.role.toLowerCase().includes(term.toLowerCase())
    );
    return of(filtered);
  }
}
