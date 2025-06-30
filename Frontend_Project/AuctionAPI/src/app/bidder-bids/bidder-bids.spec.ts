import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { BidderBids } from './bidder-bids';
import { BidderService } from '../services/bidder.service';
import { of, throwError } from 'rxjs';
import { Router } from '@angular/router';

describe('BidderBids', () => {
  let component: BidderBids;
  let fixture: ComponentFixture<BidderBids>;
  let bidderServiceSpy: jasmine.SpyObj<BidderService>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(waitForAsync(() => {
    bidderServiceSpy = jasmine.createSpyObj('BidderService', ['getBidsByBidder']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      imports: [BidderBids],
      providers: [
        { provide: BidderService, useValue: bidderServiceSpy },
        { provide: Router, useValue: routerSpy }
      ]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BidderBids);
    component = fixture.componentInstance;
    component.id = 'test-bidder-id'; // Required input
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load bids on init (success case)', () => {
    const mockBids = [{ amount: 100 }, { amount: 200 }];
    bidderServiceSpy.getBidsByBidder.and.returnValue(
      of({ success: true, data: mockBids, message: 'Items fetched' })
    );

    component.ngOnInit();

    expect(component.isLoading).toBeFalse();
    expect(component.bids).toEqual(mockBids);
    expect(component.errorMessage).toBe('');
    expect(bidderServiceSpy.getBidsByBidder).toHaveBeenCalledWith('test-bidder-id');
  });

  it('should handle response failure (success: false)', () => {
    bidderServiceSpy.getBidsByBidder.and.returnValue(
      of({ success: false, message: 'No bids found.', data:[] })
    );

    component.ngOnInit();

    expect(component.bids).toEqual([]);
    expect(component.errorMessage).toBe('No bids found.');
    expect(component.isLoading).toBeFalse();
  });

  it('should handle error during bid fetch', () => {
    bidderServiceSpy.getBidsByBidder.and.returnValue(throwError(() => new Error('Server Error')));

    component.ngOnInit();

    expect(component.bids).toEqual([]);
    expect(component.errorMessage).toBe('Failed to load bids.');
    expect(component.isLoading).toBeFalse();
  });

  it('should navigate to item detail page on ViewItem()', () => {
    const itemId = 'item123';
    component.ViewItem(itemId);

    expect(routerSpy.navigate).toHaveBeenCalledWith(['/view-item', itemId]);
  });
});
