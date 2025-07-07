import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Dashboard } from './dashboard';
import { SellerService } from '../services/seller.service';
import { BidderService } from '../services/bidder.service';
import { ItemService } from '../services/item.service';
import { of } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('Dashboard', () => {
  let component: Dashboard;
  let fixture: ComponentFixture<Dashboard>;
  let sellerServiceSpy: jasmine.SpyObj<SellerService>;
  let bidderServiceSpy: jasmine.SpyObj<BidderService>;
  let itemServiceSpy: jasmine.SpyObj<ItemService>;
  let routerSpy: jasmine.SpyObj<Router>;
  const activatedRouteStub = {
    snapshot: {
      paramMap: {
        get: (key: string) => null
      },
      url: []
    }
  };

  beforeEach(async () => {
    sellerServiceSpy = jasmine.createSpyObj('SellerService', ['getSellers']);
    bidderServiceSpy = jasmine.createSpyObj('BidderService', ['getBidders']);
    itemServiceSpy = jasmine.createSpyObj('ItemService', ['getFilteredItems']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [Dashboard, HttpClientTestingModule],
      providers: [
        { provide: SellerService, useValue: sellerServiceSpy },
        { provide: BidderService, useValue: bidderServiceSpy },
        { provide: ItemService, useValue: itemServiceSpy },
        { provide: Router, useValue: routerSpy },
        { provide: ActivatedRoute, useValue: activatedRouteStub } 
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(Dashboard);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load counts and categorize items on init', () => {
    const mockSellers = { data: [{}, {}, {}] };
    const mockBidders = { data: [{}, {}] }; 
    const mockItems = {
      data: [
        { status: 'Sold' },
        { status: 'Active' },
        { status: 'Active' },
        { status: 'Unsold' }
      ]
    };

    sellerServiceSpy.getSellers.and.returnValue(of({
      success: true,
      message: 'Fetched sellers',
      data: [{}, {}, {}]
    }));
    bidderServiceSpy.getBidders.and.returnValue(of({
      success: true,
      message: 'Fetched bidders',
      data: [{}, {}]
    }));
    
    itemServiceSpy.getFilteredItems.and.returnValue(of({
      success: true,
      message: 'Fetched items',
      data: [
        {
          itemID: '1',
          title: 'Laptop',
          category: 'Electronics',
          startingPrice: 500,
          currentBid: 600,
          startDate: new Date().toISOString(),         
          endDate: new Date().toISOString(),
          description: 'A powerful gaming laptop',      
          sellerName: 'John Doe',
          sellerId: "123",
          boughtBy: 'Jane Smith',
          status: 'Sold'
        },
        {
          itemID: '2',
          title: 'Book',
          category: 'Books',
          startingPrice: 20,
          currentBid: 0,
          startDate: new Date().toISOString(),         
          endDate: new Date().toISOString(),
          description: 'A best-selling novel',    
          sellerId: "123",    
          sellerName: 'Alice',
          boughtBy: null,
          status: 'Unsold'
        }
      ],
      pagination: {
        currentPage: 1,
        pageSize: 10,
        totalPages: 1,
        totalCount: 2
      },
      errors: null
    }));
    

    component.loadCounts();

    expect(sellerServiceSpy.getSellers).toHaveBeenCalled();
    expect(bidderServiceSpy.getBidders).toHaveBeenCalled();
    expect(itemServiceSpy.getFilteredItems).toHaveBeenCalled();

    expect(component.sellerCount).toBe(3);
    expect(component.bidderCount).toBe(2);
    expect(component.totalItems).toBe(2);
    expect(component.soldItems).toBe(1);
    expect(component.activeItems).toBe(0);
    expect(component.unsoldItems).toBe(1);
  });

  it('should navigate to correct route', () => {
    component.goTo('bidders');
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/users/bidder']);

    component.goTo('sellers');
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/users/seller']);

    component.goTo('items');
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/items']);
  });

  it('should download CSV with correct filename and structure', () => {
    spyOn(document, 'createElement').and.callFake(() => {
      return {
        setAttribute: () => {},
        click: () => {},
        style: {},
      } as any;
    });
    spyOn(document.body, 'appendChild');
    spyOn(document.body, 'removeChild');

    component.allItems = [
      {
        itemID: '1',
        title: 'Item 1',
        category: 'Books',
        startingPrice: 100,
        currentBid: 150,
        endDate: new Date().toISOString(),
        sellerName: 'John Doe',
        boughtBy: 'Jane Doe',
        status: 'Sold'
      }
    ];

    component.downloadItemsCSV('Sold');

    expect(document.createElement).toHaveBeenCalledWith('a');
    expect(document.body.appendChild).toHaveBeenCalled();
    expect(document.body.removeChild).toHaveBeenCalled();
  });
});
