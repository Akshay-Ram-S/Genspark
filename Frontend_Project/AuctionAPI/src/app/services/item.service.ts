import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ItemAllBids } from '../models/all-bids';

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
    page?: number;
    pageSize?: number;
  }): Observable<{
    success: boolean;
    message: string;
    data: Item[];
    pagination: {
      currentPage: number;
      pageSize: number;
      totalPages: number;
      totalCount: number;
    };
    errors: any;
  }> {
    let params = new HttpParams();

    if (filters.search) params = params.set('search', filters.search);
    if (filters.category) params = params.set('category', filters.category);
    if (filters.startingPrice != null) params = params.set('startingPrice', filters.startingPrice.toString());
    if (filters.endingPrice != null) params = params.set('endingPrice', filters.endingPrice.toString());
    if (filters.endDateBefore) params = params.set('endDateBefore', filters.endDateBefore);
    if (filters.page != null) params = params.set('page', filters.page.toString());
    if (filters.pageSize != null) params = params.set('pageSize', filters.pageSize.toString());

    return this.http.get<any>(`${this.baseUrl}`, { params });
  }

  getItemById(itemId: string): Observable<ApiResponse<Item>> {
    return this.http.get<ApiResponse<Item>>(`${this.baseUrl}/${itemId}`);
  }
  
  getAllBidsForItem(itemId: string): Observable<ApiResponse<ItemAllBids[]>> {
    return this.http.get<ApiResponse<ItemAllBids[]>>(`${this.baseUrl}/all-bids/${itemId}`);
  }

  postItem(formData: FormData): Observable<any> {
    return this.http.post(`${this.baseUrl}`, formData);
  }

  updateItem(itemId: string, formData: FormData): Observable<any> {
    console.log('Updating item with ID:', itemId);
    return this.http.put(`${this.baseUrl}/${itemId}`, formData);
  }

  deleteItem(itemId: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${itemId}`);
  }
}
