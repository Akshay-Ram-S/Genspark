import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TokenService } from './token.service';
import { User } from '../models/user';

@Injectable({ providedIn: 'root' })
export class SellerService {
  private baseUrl = 'http://localhost:5205/api/v1/Seller';

  constructor(private http: HttpClient, private tokenService: TokenService) {}

  getItemsBySeller(id?: string): Observable<ApiResponse<any[]>> {
    const sellerId = id || this.tokenService.getUserId();
    return this.http.get<ApiResponse<any[]>>(`${this.baseUrl}/Items/${sellerId}`);
  }

  getSellers(): Observable<ApiResponse<any[]>> {
    return this.http.get<ApiResponse<any[]>>(`${this.baseUrl}`);
  }

  getSellerById(sellerId: string): Observable<ApiResponse<any>> {
    return this.http.get<ApiResponse<any>>(`${this.baseUrl}/${sellerId}`);
  }
}
