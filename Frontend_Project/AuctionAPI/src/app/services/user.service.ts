import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RegisterRequest } from '../models/register';

@Injectable({ providedIn: 'root' })
export class UserService {
  private baseUrl = 'http://localhost:5205/api/v1/user';

  constructor(private http: HttpClient) {}

  register(data: RegisterRequest): Observable<any> {
    const headers = new HttpHeaders({ 'skipAuth': 'true' });
    return this.http.post<any>(this.baseUrl, data, { headers });
  }

  delete(): Observable<any> {
    return this.http.delete<any>(this.baseUrl);
  }

  update(data: any): Observable<any> {
    return this.http.put<any>(this.baseUrl, data);
  }

  changeState(data: any): Observable<any> {
    return this.http.put<any>(`${this.baseUrl}/change-state`, data);
  }
}
