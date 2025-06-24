import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  private hubConnection!: signalR.HubConnection;
  private notificationsSubject = new BehaviorSubject<any[]>([]);
  notifications$ = this.notificationsSubject.asObservable();
  private unreadCount = 0;
  private unreadSubject = new BehaviorSubject<number>(0);
  unreadCount$ = this.unreadSubject.asObservable();

  constructor(){
    const stored = localStorage.getItem('notifications');
    if (stored) {
        this.notificationsSubject.next(JSON.parse(stored));
    }
    this.startConnection();
  }

  startConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`http://localhost:5205/auctionHub`, {
            withCredentials: false
        })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR connection established'))
      .catch(err => console.error('SignalR error:', err));

    this.hubConnection.on('ReceiveBid', (bid: any) => {
        const current = this.notificationsSubject.getValue();
        const updated = [bid, ...current];

        this.notificationsSubject.next(updated);
        localStorage.setItem('notifications', JSON.stringify(updated));

        this.unreadCount++;
        this.unreadSubject.next(this.unreadCount);
    });
  }

  getNotifications() {
    return this.notifications$;
  }

  clearNotifications() {
    this.notificationsSubject.next([]);
    localStorage.removeItem('notifications');
    this.unreadCount = 0;
    this.unreadSubject.next(0);
    }

  markAllAsRead() {
    this.unreadCount = 0;
    this.unreadSubject.next(0);
  }
}
