import { Component, ElementRef, HostListener, ViewChild } from '@angular/core';
import { NotificationService } from '../services/notification.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-notification',
  imports: [CommonModule],
  templateUrl: './notification.html',
  styleUrl: './notification.css'
})
export class Notification {
  @ViewChild('bellWrapper', { static: true }) bellWrapper!: ElementRef;
  notifications: any[] = [];
  showPanel = false;
  unreadCount: number = 0;

  get notificationCount() {
    return this.notifications.length;
  }

  constructor(private notificationService: NotificationService) {}

  ngOnInit(): void {
    this.notificationService.getNotifications().subscribe((notifs) => {
      this.notifications = notifs;
    });
    this.notificationService.unreadCount$.subscribe(count => {
      this.unreadCount = count;
    });
  }

  @HostListener('document:click', ['$event'])
  onClickOutside(event: MouseEvent) {
    const clickedInside = this.bellWrapper.nativeElement.contains(event.target);
    if (!clickedInside) {
      this.showPanel = false;
    }
  }

  togglePanel() {
    this.showPanel = !this.showPanel;
    if (this.showPanel) {
      this.notificationService.markAllAsRead();
    }
  }
}
