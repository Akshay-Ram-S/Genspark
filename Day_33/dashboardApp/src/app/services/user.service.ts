import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class UserService {
  private baseUrl = 'https://dummyjson.com/users';

  constructor(private http: HttpClient) {}

  addUser(user: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/add`, user);
  }

  getUsers(): Observable<{ users: any[] }> {
    return this.http.get<{ users: any[] }>(this.baseUrl);
  }
}