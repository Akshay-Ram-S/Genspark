import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ShowBids } from './show-bids';
import { BidService } from '../services/bid.service';
import { TokenService } from '../services/token.service';
import { of, throwError } from 'rxjs';

export class WindowService {
  reload(): void {
    window.location.reload();
  }
}

describe('ShowBids', () => {
  let component: ShowBids;
  let fixture: ComponentFixture<ShowBids>;
  let bidServiceSpy: jasmine.SpyObj<BidService>;
  let tokenServiceSpy: jasmine.SpyObj<TokenService>;

  const sampleBids = [
    { bidId: 'b1', amount: 100 },
    { bidId: 'b2', amount: 150 }
  ];

  beforeEach(async () => {
    bidServiceSpy = jasmine.createSpyObj('BidService', ['deleteBid']);
    tokenServiceSpy = jasmine.createSpyObj('TokenService', ['getRole']);

    await TestBed.configureTestingModule({
      imports: [ShowBids],
      providers: [
        { provide: BidService, useValue: bidServiceSpy },
        { provide: TokenService, useValue: tokenServiceSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ShowBids);
    component = fixture.componentInstance;

    component.bids = [...sampleBids];
    component.highestAmount = 150;
    component.auctionEnded = false;

    tokenServiceSpy.getRole.and.returnValue('admin'); 
    fixture.detectChanges();
     
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should set isAdmin to true if user role is admin', () => {
    expect(component.isAdmin).toBeTrue();
  });

  it('should set isAdmin to false if user role is not admin', () => {
    tokenServiceSpy.getRole.and.returnValue('bidder');
    component.ngOnInit();
    expect(component.isAdmin).toBeFalse();
  });

  it('should open confirm modal with selected bid', () => {
    const bid = sampleBids[0];
    component.openConfirmModal(bid);
    expect(component.showConfirmModal).toBeTrue();
    expect(component.selectedBid).toEqual(bid);
  });

  it('should close confirm modal and reset selectedBid', () => {
    component.selectedBid = sampleBids[0];
    component.showConfirmModal = true;
    component.closeConfirmModal();
    expect(component.showConfirmModal).toBeFalse();
    expect(component.selectedBid).toBeNull();
  });

  it('should handle error when bid deletion fails', () => {
    spyOn(console, 'error');
    spyOn(window, 'alert');

    component.selectedBid = sampleBids[1];
    bidServiceSpy.deleteBid.and.returnValue(throwError(() => new Error('Delete failed')));

    component.confirmDeleteBid();

    expect(console.error).toHaveBeenCalled();
    expect(window.alert).toHaveBeenCalledWith('Failed to delete bid');
    expect(component.showConfirmModal).toBeFalse();
  });

  it('should do nothing if selectedBid is null', () => {
    component.selectedBid = null;
    component.confirmDeleteBid();
    expect(bidServiceSpy.deleteBid).not.toHaveBeenCalled();
  });
});
