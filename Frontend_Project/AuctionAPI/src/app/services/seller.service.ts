import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TokenService } from './token.service';
import { User } from '../models/user';

@Injectable({ providedIn: 'root' })
export class SellerService {
  private baseUrl = 'http://localhost:5205/api/v1/Seller';

  constructor(private http: HttpClient, public tokenService: TokenService) {}

  getItemsBySeller(id:string): Observable<ApiResponse<any[]>> {
    if(id){
      return this.http.get<ApiResponse<any[]>>(`${this.baseUrl}/Items/${id}`);
    }
    const Id = this.tokenService.getUserId();
    const token = localStorage.getItem('token');

    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });

    return this.http.get<ApiResponse<any[]>>(`${this.baseUrl}/Items/${Id}`,{headers});
  }

  getSellers(): Observable<ApiResponse<any[]>> {
    return this.http.get<ApiResponse<any[]>>(`${this.baseUrl}`);
  }

  getSellerById(sellerId: string): Observable<ApiResponse<any>> {
    return this.http.get<ApiResponse<any>>(`${this.baseUrl}/${sellerId}`);
  }
}
