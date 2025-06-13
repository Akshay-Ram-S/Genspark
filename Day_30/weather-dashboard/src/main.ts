import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { WeatherDashboardComponent } from './app/components/weather-dashboard/weather-dashboard';

bootstrapApplication(WeatherDashboardComponent, appConfig)
  .catch(err => console.error(err));