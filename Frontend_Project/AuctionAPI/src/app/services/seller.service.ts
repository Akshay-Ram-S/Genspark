import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TokenService } from './misc.service';

@Injectable({ providedIn: 'root' })
export class SellerService {
  private baseUrl = 'http://localhost:5205/api/v1/Seller';

  constructor(private http: HttpClient, public tokenService: TokenService) {}

  getItemsBySeller(): Observable<ApiResponse<any[]>> {
    const Id = this.tokenService.getUserId();
    const token = localStorage.getItem('token');

    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });

    return this.http.get<ApiResponse<any[]>>(`${this.baseUrl}/Items/${Id}`,{headers});
  }
}
