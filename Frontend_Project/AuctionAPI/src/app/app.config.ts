import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { AuthService } from './services/auth.service';
import { HttpClient } from '@angular/common/http';
import { TokenService } from './services/token.service';
import { SellerService } from './services/seller.service';
import { ItemService } from './services/item.service';
import { BidderService } from './services/bidder.service';
import { NotificationService } from './services/notification.service';
import { ImageService } from './services/image.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    HttpClient,
    AuthService,
    TokenService,
    SellerService,
    BidderService,
    ItemService,
    NotificationService,
    ImageService
  ]
};
