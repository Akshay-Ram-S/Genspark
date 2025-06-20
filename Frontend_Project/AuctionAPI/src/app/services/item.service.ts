import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ItemService {
  private baseUrl = 'http://localhost:5205/api/v1/Items'; 

  constructor(private http: HttpClient) {}

  getFilteredItems(filters: {
    search?: string;
    category?: string;
    startingPrice?: number;
    endingPrice?: number;
    endDateBefore?: string;
  }): Observable<ApiResponse<any[]>> {

    let params = new HttpParams();
    if (filters.search) 
        params = params.set('search', filters.search);
    if (filters.category) 
        params = params.set('category', filters.category);
    if (filters.startingPrice != null) 
        params = params.set('startingPrice', filters.startingPrice.toString());
    if (filters.endingPrice != null) 
        params = params.set('endingPrice', filters.endingPrice.toString());
    if (filters.endDateBefore) 
        params = params.set('endDateBefore', filters.endDateBefore);

    return this.http.get<ApiResponse<Item[]>>(this.baseUrl, { params });
  }
}
