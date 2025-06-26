import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LoginRequest } from '../models/login-request';
import { RegisterRequest } from '../models/register';
import { NotificationService } from './notification.service';
import { not } from 'rxjs/internal/util/not';

@Injectable({ providedIn: 'root' })

export class AuthService {
  private baseUrl = 'http://localhost:5205/api/v1/auth';

  constructor(private http: HttpClient,
              private notficationService: NotificationService
  ) {}

  login(data: LoginRequest): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/login`, data);
  }

  logout(): void {
    const refreshToken = localStorage.getItem('refreshToken');
    const token = localStorage.getItem('token');

    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`
    });
    const body = { refreshToken };
    this.http.post(`${this.baseUrl}/logout`, body, {headers}).subscribe({
      next: () => {
        localStorage.removeItem('token');
        localStorage.removeItem('refreshToken');
        this.notficationService.clearNotifications();
        
      },
      error: () => {
        localStorage.removeItem('token');
        localStorage.removeItem('refreshToken');
        this.notficationService.clearNotifications();
      }
    });
  }

  getMe(): Observable<any> {
    const token = localStorage.getItem('token');

    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });

    return this.http.get<any>(`${this.baseUrl}/me`, { headers });
  }

  isAuthenticated(): boolean {
    return !!localStorage.getItem('token');
  }

  
}
