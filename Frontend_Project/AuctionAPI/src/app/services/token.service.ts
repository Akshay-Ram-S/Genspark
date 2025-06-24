import { Injectable } from '@angular/core';
import {jwtDecode} from 'jwt-decode';

@Injectable({ providedIn: 'root' })
export class TokenService {
  getDecodedToken(): any | null {
    const token = localStorage.getItem('token');
    return token ? jwtDecode<any>(token) : null;
  }

  getUserId(): string | null {
    return this.getDecodedToken()?.Id ?? null;
  }

  getRole(): string | null {
    return this.getDecodedToken()?.role ?? null;
  }
}