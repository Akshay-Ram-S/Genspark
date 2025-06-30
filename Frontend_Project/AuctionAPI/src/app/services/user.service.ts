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

  delete(): Observable<any> {
    const token = localStorage.getItem('token');

    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });

    return this.http.delete<any>(this.baseUrl, { headers });
  }

  update(data: any): Observable<any>{
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
    return this.http.put<any>(this.baseUrl, data, {headers});
  }

  changeState(data: any): Observable<any>{
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
    return this.http.put<any>(`${this.baseUrl}/change-state`, data, {headers});
  }

}
