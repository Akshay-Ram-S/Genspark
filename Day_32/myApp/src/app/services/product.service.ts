import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product } from '../models/product.model';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private apiUrl = 'https://dummyjson.com/products';

  constructor(private http: HttpClient) {}

  searchProducts(query: string, limit: number, skip: number): Observable<{ products: Product[] }> {
    return this.http.get<{ products: Product[] }>(
      `${this.apiUrl}/search?q=${query}&limit=${limit}&skip=${skip}`
    );
  }

  getProductById(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/${id}`);
  }
}
