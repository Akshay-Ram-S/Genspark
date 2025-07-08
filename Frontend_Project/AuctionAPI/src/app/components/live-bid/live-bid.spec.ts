import { ComponentFixture, fakeAsync, TestBed, tick } from '@angular/core/testing';
import { LiveBid } from './live-bid';
import { ActivatedRoute } from '@angular/router';
import { of, Subject } from 'rxjs';
import { BidService } from '../../../services/bid.service';
import { ItemService } from '../../../services/item.service';
import { TokenService } from '../../../services/token.service';
import { NotificationService } from '../../../services/notification.service';
import { FormBuilder } from '@angular/forms';

describe('LiveBid', () => {
  let component: LiveBid;
  let fixture: ComponentFixture<LiveBid>;

const mockItem = {
  itemID: '123',
  title: 'Sample Item',
  description: 'Mock description',
  status: 'open',
  category: 'Electronics',
  startingPrice: 100,
  sellerName:'Jack',
  sellerID: 'seller-1',
  startDate: new Date().toISOString(),
  endDate: new Date(Date.now() + 3600 * 1000).toISOString(),
  imageUrl: 'https://example.com/image.jpg' 
};


  const activatedRouteStub = {
    paramMap: of({ get: (key: string) => '123' })
  };

  const bidServiceSpy = jasmine.createSpyObj('BidService', ['placeBid']);
  const itemServiceSpy = jasmine.createSpyObj('ItemService', ['getItemById', 'getAllBidsForItem']);
  const tokenServiceSpy = jasmine.createSpyObj('TokenService', ['getRole', 'getUserId']);
  const notificationServiceStub = {
    bidPlaced$: new Subject<any>()
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LiveBid],
      providers: [
        FormBuilder,
        { provide: ActivatedRoute, useValue: activatedRouteStub },
        { provide: BidService, useValue: bidServiceSpy },
        { provide: ItemService, useValue: itemServiceSpy },
        { provide: TokenService, useValue: tokenServiceSpy },
        { provide: NotificationService, useValue: notificationServiceStub }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(LiveBid);
    component = fixture.componentInstance;

    tokenServiceSpy.getRole.and.returnValue('bidder');
    tokenServiceSpy.getUserId.and.returnValue('user-1');

    itemServiceSpy.getItemById.and.returnValue(of({ data: mockItem }));
    itemServiceSpy.getAllBidsForItem.and.returnValue(of({ data: [] }));

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load item and bids on init', () => {
    
    expect(component.bids).toEqual([]);
    expect(component.auctionEnded).toBeFalse();
  });

  it('should calculate countdown correctly', fakeAsync(() => {
    component.item = mockItem;
    component.startCountdown();

    tick(1000); 
    fixture.detectChanges();

    expect(component.countdown).toContain('H');
  }));

  it('should unsubscribe on destroy', () => {
    const subSpy = jasmine.createSpyObj('Subscription', ['unsubscribe']);
    component['countdownSubscription'] = subSpy;
    component.ngOnDestroy();
    expect(subSpy.unsubscribe).toHaveBeenCalled();
  });
});
