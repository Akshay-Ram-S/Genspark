import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface BidRequest {
  itemId: string;
  bidderId: string;
  Amount: number;
}

@Injectable({
  providedIn: 'root'
})
export class BidService {
  private apiUrl = 'http://localhost:5205/api/v1/Bid';

  constructor(private http: HttpClient) {}

  placeBid(bid: BidRequest): Observable<ApiResponse<any>> {
    
    const token = localStorage.getItem('token'); 

    const headers = new HttpHeaders({
        Authorization: `Bearer ${token}`
    });
    return this.http.post<ApiResponse<any>>(this.apiUrl, bid, {headers});
  }

  
}
