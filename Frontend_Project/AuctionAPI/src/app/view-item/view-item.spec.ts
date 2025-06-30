import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { ActivatedRoute } from '@angular/router';

import { ViewItem } from './view-item';
import { ItemService } from '../services/item.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ItemAllBids } from '../models/all-bids';

const mockTokenService = {
  getRole: () => 'bidder',
  getDecodedToken: () => ({ role: 'bidder' })
};
const mockItem = {
  itemID: 'item1',
  title: 'Item 1',
  description: 'Description',
  startingPrice: 100,
  endDate: new Date().toISOString(),
  status: 'Active',
  category: 'Electronics',
  startDate: new Date().toISOString(),
  sellerName: 'John Doe'
};

const mockBids: ItemAllBids[] = [
  {
    name: 'Alice',
    amount: 150,
    bid_timestamp: new Date().toISOString(),
    title: 'Item 1',
    bidder_id: 'bidder1'
  },
  {
    name: 'Bob',
    amount: 200,
    bid_timestamp: new Date().toISOString(),
    title: 'Item 1',
    bidder_id: 'bidder2'
  }
];

describe('ViewItem', () => {
  let component: ViewItem;
  let fixture: ComponentFixture<ViewItem>;
  let itemServiceSpy: jasmine.SpyObj<ItemService>;

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('ItemService', ['getItemById', 'getAllBidsForItem']);

    await TestBed.configureTestingModule({
      imports: [ViewItem, HttpClientTestingModule],
      providers: [
        { provide: ItemService, useValue: spy },
        {provide: mockTokenService, useValue: mockTokenService },
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: {
                get: () => 'item1'
              }
            }
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ViewItem);
    component = fixture.componentInstance;
    itemServiceSpy = TestBed.inject(ItemService) as jasmine.SpyObj<ItemService>;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load item and bids on init', () => {
    itemServiceSpy.getItemById.and.returnValue(of({ success:true, data: mockItem, message:'item fetched' }));
    itemServiceSpy.getAllBidsForItem.and.returnValue(of({ success:true, data: mockBids, message:'item fetched' }));

    fixture.detectChanges(); 

    expect(component.item).toEqual(mockItem);
    expect(component.bids.length).toBe(2);
    expect(component.highestAmount).toBe(200);
    expect(component.displayedBids.length).toBeGreaterThan(0);
  });

  it('should handle error when loading item fails', () => {
    itemServiceSpy.getItemById.and.returnValue(throwError(() => new Error('Failed to fetch')));
    itemServiceSpy.getAllBidsForItem.and.returnValue(of({ success:false, data: [], message:'error' }));

    fixture.detectChanges();

    expect(component.errorMessage).toContain('Failed to load item.');
  });


  it('should sort bids by highest amount', () => {
    itemServiceSpy.getItemById.and.returnValue(of({ success:true, data: mockItem, message:'highest bid fetched' }));
    itemServiceSpy.getAllBidsForItem.and.returnValue(of({ success:true, data: mockBids, message:'item fetched' }));

    fixture.detectChanges();

    component.sortOption = 'highest';
    component.sortBids();

    expect(component.bids[0].amount).toBeGreaterThanOrEqual(component.bids[1].amount);
  });

});
