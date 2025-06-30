import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { ItemsComponent } from './items-component';
import { of, throwError, Subscription } from 'rxjs';
import { ItemService } from '../services/item.service';
import { Router } from '@angular/router';

describe('ItemsComponent', () => {
  let component: ItemsComponent;
  let fixture: ComponentFixture<ItemsComponent>;
  let mockItemService: jasmine.SpyObj<ItemService>;
  let mockRouter: jasmine.SpyObj<Router>;

  const mockResponse = {
    success: true,
    message: 'Item tested',
    data: [
      {
        itemID: '1',
        title: 'Item 1',
        description: 'Test item 1',
        status: 'Active',
        category: 'Electronics',
        startingPrice: 100,
        startDate: '2025-06-29T12:00:00Z',
        endDate: '2025-07-05T12:00:00Z',
        sellerName: 'Alice',
        imageUrl: ''
      },
      {
        itemID: '2',
        title: 'Item 2',
        description: 'Test item 2',
        status: 'Sold',
        category: 'Sports',
        startingPrice: 200,
        startDate: '2025-06-28T12:00:00Z',
        endDate: '2025-07-03T12:00:00Z',
        sellerName: 'Bob',
        imageUrl: ''
      }
    ],
    pagination: {
      currentPage: 1,
      pageSize: 10,
      totalPages: 3,
      totalCount: 26
    },
    errors: null
  };


  beforeEach(async () => {
    mockItemService = jasmine.createSpyObj('ItemService', ['getFilteredItems']);
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [ItemsComponent],
      providers: [
        { provide: ItemService, useValue: mockItemService },
        { provide: Router, useValue: mockRouter }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ItemsComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch items on init and set items & totalPages', () => {
    mockItemService.getFilteredItems.and.returnValue(of(mockResponse));
    component.ngOnInit();
    expect(mockItemService.getFilteredItems).toHaveBeenCalled();
    expect(component.items.length).toBe(2);
    expect(component.totalPages).toBe(3);
  });

  it('should trigger fetchFilteredItems when search changes', fakeAsync(() => {
    mockItemService.getFilteredItems.and.returnValue(of(mockResponse));
    component.ngOnInit();
    component.onSearchChange('test');
    tick(1000);
    expect(mockItemService.getFilteredItems).toHaveBeenCalledTimes(2); // init + search
  }));

  it('should handle fetchFilteredItems error', () => {
    mockItemService.getFilteredItems.and.returnValue(throwError(() => new Error('error')));
    component.fetchFilteredItems();
    expect(component.noItemsFound).toBeTrue();
    expect(component.isLoading).toBeFalse();
  });

  it('should navigate to post-item page', () => {
    component.onPostItem();
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/post-item']);
  });

  it('should go to specific valid page', () => {
    mockItemService.getFilteredItems.and.returnValue(of(mockResponse));
    component.totalPages = 3;
    component.goToPage(2);
    expect(component.currentPage).toBe(2);
    expect(mockItemService.getFilteredItems).toHaveBeenCalled();
  });

  it('should not go to invalid page number', () => {
    component.totalPages = 3;
    component.currentPage = 1;
    component.goToPage(5);
    expect(component.currentPage).toBe(1); // should not change
  });

  it('should go to previous page if currentPage > 1', () => {
    mockItemService.getFilteredItems.and.returnValue(of(mockResponse));
    component.currentPage = 2;
    component.goPrevious();
    expect(component.currentPage).toBe(1);
  });

  it('should not go previous if on first page', () => {
    component.currentPage = 1;
    component.goPrevious();
    expect(component.currentPage).toBe(1);
  });

  it('should go to next page if not on last page', () => {
    mockItemService.getFilteredItems.and.returnValue(of(mockResponse));
    component.totalPages = 3;
    component.currentPage = 1;
    component.goNext();
    expect(component.currentPage).toBe(2);
  });

  it('should not go next if on last page', () => {
    component.totalPages = 3;
    component.currentPage = 3;
    component.goNext();
    expect(component.currentPage).toBe(3);
  });

  it('should clear filters and call fetchFilteredItems', () => {
    spyOn(component, 'fetchFilteredItems');
    component.clearFilters();
    expect(component.searchTerm).toBe('');
    expect(component.startingPrice).toBeNull();
    expect(component.endingPrice).toBeNull();
    expect(component.currentPage).toBe(1);
    expect(component.fetchFilteredItems).toHaveBeenCalled();
  });

  it('should unsubscribe on destroy', () => {
    component['searchSub'] = new Subscription(); // manually initialize
    const unsubscribeSpy = spyOn(component['searchSub'], 'unsubscribe');
    component.ngOnDestroy();
    expect(unsubscribeSpy).toHaveBeenCalled();
  });
});
