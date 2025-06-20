import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LoginRequest } from '../models/login-request';
import { RegisterRequest } from '../models/register';

@Injectable({ providedIn: 'root' })

export class UserService {
  private baseUrl = 'http://localhost:5205/api/v1/user';

  constructor(private http: HttpClient) {}


  register(data: RegisterRequest): Observable<any> {

    const url = `${this.baseUrl}`;

    return this.http.post<any>(url, data);
  }

}
