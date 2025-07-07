import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LoginRequest } from '../models/login-request';
import { NotificationService } from './notification.service';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private baseUrl = 'http://localhost:5205/api/v1/auth';

  constructor(
    private http: HttpClient,
    private notificationService: NotificationService
  ) {}

  login(data: LoginRequest): Observable<any> {
    const headers = new HttpHeaders({ 'skipAuth': 'true' });
    return this.http.post<any>(`${this.baseUrl}/login`, data, { headers });
  }

  logout(): void {
    const refreshToken = localStorage.getItem('refreshToken');
    const body = { refreshToken };

    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    this.notificationService.clearNotifications();

    this.http.post(`${this.baseUrl}/logout`, body).subscribe({
      next: () => window.location.reload(),
      error: (err) => console.error('Error during logout', err)
    });
  }

  getMe(): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/me`);
  }

  isAuthenticated(): boolean {
    return !!localStorage.getItem('token');
  }
}
