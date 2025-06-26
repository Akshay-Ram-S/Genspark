import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ImageService {
  private baseUrl = 'http://localhost:5205/api/v1/image'; 

  constructor(private http: HttpClient) {}

  getItemImage(itemId: string) {
    return this.http.get(`${this.baseUrl}/View/${itemId}`);
  }
}
