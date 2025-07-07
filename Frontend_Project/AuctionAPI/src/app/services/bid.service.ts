import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
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
    return this.http.post<ApiResponse<any>>(this.apiUrl, bid);
  }

  deleteBid(bidId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${bidId}`);
  }
}
