import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { WeatherService } from '../../services/weather';

@Component({
  selector: 'app-city-search',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './city-search.html',
  styleUrls: ['./city-search.css']
})
export class CitySearchComponent {
  city = signal('');

  constructor(private weatherService: WeatherService) {}

  search() {
    const trimmed = this.city().trim();
    if (trimmed) {
      this.weatherService.setCity(trimmed);
    }
  }

  onInputChange(event: Event) {
    const input = event.target as HTMLInputElement;
    this.city.set(input.value);
  }
}
