import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { AuthService } from './services/auth.service';
import { HttpClient } from '@angular/common/http';
import { TokenService } from './services/misc.service';
import { SellerService } from './services/seller.service';
import { ItemService } from './services/item.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    HttpClient,
    AuthService,
    TokenService,
    SellerService,
    ItemService
  ]
};
