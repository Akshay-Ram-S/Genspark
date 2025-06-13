import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AsyncPipe } from '@angular/common';
import { WeatherCardComponent } from '../weather-card/weather-card';
import { CitySearchComponent } from '../city-search/city-search';
import { WeatherService } from '../../services/weather';

@Component({
  selector: 'app-weather-dashboard',
  standalone: true,
  imports: [CommonModule, CitySearchComponent, WeatherCardComponent, AsyncPipe],
  templateUrl: './weather-dashboard.html',
  styleUrls: ['./weather-dashboard.css']
})
export class WeatherDashboardComponent {
  weather$: ReturnType<WeatherService['getWeather']>;
  error$: ReturnType<WeatherService['getError']>;

  constructor(private weatherService: WeatherService) {
    this.weather$ = this.weatherService.getWeather();
    this.error$ = this.weatherService.getError();
  }
}
