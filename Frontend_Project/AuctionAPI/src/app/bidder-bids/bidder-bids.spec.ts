import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BidderBids } from './bidder-bids';

describe('BidderBids', () => {
  let component: BidderBids;
  let fixture: ComponentFixture<BidderBids>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BidderBids]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BidderBids);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
