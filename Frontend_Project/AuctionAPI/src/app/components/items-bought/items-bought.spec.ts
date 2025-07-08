import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ItemsBought } from './items-bought';
import { of, throwError } from 'rxjs';
import { BidderService } from '../../services/bidder.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('ItemsBought', () => {
  let component: ItemsBought;
  let fixture: ComponentFixture<ItemsBought>;
  let bidderServiceSpy: jasmine.SpyObj<BidderService>;

  const mockItems = [
    { itemID: '1', title: 'Item 1' },
    { itemID: '2', title: 'Item 2' }
  ];

  beforeEach(async () => {
    bidderServiceSpy = jasmine.createSpyObj('BidderService', ['getItemsBought']);

    await TestBed.configureTestingModule({
      imports: [ItemsBought, HttpClientTestingModule],
      providers: [{ provide: BidderService, useValue: bidderServiceSpy }]
    }).compileComponents();

    fixture = TestBed.createComponent(ItemsBought);
    component = fixture.componentInstance;
    component.bidderId = 'test-bidder-id';
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should handle error while loading items', () => {
    const error = new Error('Failed to load');
    spyOn(console, 'error');
    bidderServiceSpy.getItemsBought.and.returnValue(throwError(() => error));
    fixture.detectChanges(); 

    expect(bidderServiceSpy.getItemsBought).toHaveBeenCalledWith('test-bidder-id');
    expect(console.error).toHaveBeenCalledWith('Error loading seller profile', error);
    expect(component.items.length).toBe(0); 
  });
});
