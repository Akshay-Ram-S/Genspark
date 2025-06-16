import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, Observable, of } from 'rxjs';

@Injectable({providedIn: 'root'})
export class ProductService {
  private url = 'https://dummyjson.com/products/search';

  constructor(private http: HttpClient) {}

  searchProducts(query: string, limit: number, skip: number): Observable<any> {
    return this.http.get<any>(`${this.url}?q=${query}&limit=${limit}&skip=${skip}`).pipe(
    catchError(error => {
      console.error('API error:', error);
      return of({ products: [], error: true }); 
    })
  );
  }
}
