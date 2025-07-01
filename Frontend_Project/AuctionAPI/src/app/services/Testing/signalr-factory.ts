import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';

@Injectable({ providedIn: 'root' })
export class SignalRFactory {
  createConnection(): signalR.HubConnection {
    return new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5205/auctionhub')
      .withAutomaticReconnect()
      .build();
  }
}