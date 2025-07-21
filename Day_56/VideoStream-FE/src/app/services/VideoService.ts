import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class VideoService {
  private apiUrl = 'http://localhost:5082/api/videos';

  constructor(private http: HttpClient) {}

  uploadVideo(formData: FormData) {
    return this.http.post(`${this.apiUrl}/upload`, formData);
  }

  getAllVideos(){
    return this.http.get<any[]>(`${this.apiUrl}`);
  }

}
