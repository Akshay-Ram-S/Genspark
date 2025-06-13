import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, Observable, catchError, throwError, switchMap, timer } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class WeatherService {
  private readonly API_KEY = '40e8635e2e89be6bfa035a2acd1338c2';
  private readonly API_URL = 'https://api.openweathermap.org/data/2.5/weather';

  private selectedCity$ = new BehaviorSubject<string>('Chennai');
  private weatherData$ = new BehaviorSubject<any | null>(null);
  private weatherError$ = new BehaviorSubject<string | null>(null);
  
  getError(): Observable<string | null> {
    return this.weatherError$.asObservable();
  }


  constructor(private http: HttpClient) {
  }

  fetchWeather(city: string): Observable<any> {
    if (!city || !city.trim()) return throwError(() => new Error('Invalid city'));

    const url = `${this.API_URL}?q=${city.trim()}&appid=${this.API_KEY}&units=metric`;

    return this.http.get(url).pipe(
      catchError(this.handleError)
    );
  }

  setCity(city: string) {
  const trimmed = city.trim();
  if (!trimmed) return;

  this.selectedCity$.next(trimmed);
  this.weatherError$.next(null);

  this.fetchWeather(trimmed).subscribe({
    next: data => this.weatherData$.next(data),
    error: err => this.weatherError$.next(err.message)
  });
}

  getWeather(): Observable<any | null> {
    return this.weatherData$.asObservable();
  }

  private handleError(error: HttpErrorResponse) {
    let msg = 'An error occurred';
    if (error.status === 404) msg = 'City not found';
    else if (error.status === 401) msg = 'Unauthorized: Check your API key';
    return throwError(() => new Error(msg));
  }
}
