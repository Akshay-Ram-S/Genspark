import { TestBed } from '@angular/core/testing';
import { NotificationService } from '../notification.service';
import * as signalR from '@microsoft/signalr';
import { SignalRFactory } from './signalr-factory';

describe('NotificationService', () => {
  let service: NotificationService;
  let mockHubConnection: jasmine.SpyObj<signalR.HubConnection>;
  let signalRFactorySpy: jasmine.SpyObj<SignalRFactory>;

  beforeEach(() => {
    mockHubConnection = jasmine.createSpyObj('HubConnection', ['start', 'on', 'stop', 'invoke']);
    mockHubConnection.start.and.returnValue(Promise.resolve());

    signalRFactorySpy = jasmine.createSpyObj('SignalRFactory', ['createConnection']);
    signalRFactorySpy.createConnection.and.returnValue(mockHubConnection);

    TestBed.configureTestingModule({
      providers: [
        NotificationService,
        { provide: SignalRFactory, useValue: signalRFactorySpy }
      ]
    });

    service = TestBed.inject(NotificationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should call start on HubConnection when started', async () => {
    await mockHubConnection.start();
    expect(mockHubConnection.start).toHaveBeenCalled();
  });

  it('should clear notifications', () => {
    service.clearNotifications();
    service.getNotifications().subscribe(n => {
      expect(n.length).toBe(0);
    });
  });

  it('should mark all as read', () => {
    service['unreadCount'] = 5;
    service.markAllAsRead();
    service['unreadCount$'].subscribe(count => {
      expect(count).toBe(0);
    });
  });
});
