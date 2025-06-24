import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TokenService } from './token.service';

@Injectable({ providedIn: 'root' })
export class BidderService {
  private baseUrl = 'http://localhost:5205/api/v1/Bidder';

  constructor(private http: HttpClient, public tokenService: TokenService) {}

  getBidsByBidder(id:string): Observable<ApiResponse<any[]>> {

    return this.http.get<ApiResponse<any[]>>(`${this.baseUrl}/Bids/${id}`);
  }

  getBidders(): Observable<ApiResponse<any[]>> {
    return this.http.get<ApiResponse<any[]>>(`${this.baseUrl}`);
  }
  
  getBidderById(id: string): Observable<ApiResponse<any>> {
    return this.http.get<ApiResponse<any>>(`${this.baseUrl}/${id}`);
  }

}