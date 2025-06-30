import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SellerItems } from './seller-items';
import { ActivatedRoute, Router } from '@angular/router';
import { SellerService } from '../services/seller.service';
import { of, throwError } from 'rxjs';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('SellerItems', () => {
  let component: SellerItems;
  let fixture: ComponentFixture<SellerItems>;
  let mockSellerService: jasmine.SpyObj<SellerService>;
  let mockRouter: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    mockSellerService = jasmine.createSpyObj('SellerService', ['getItemsBySeller']);
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [SellerItems, HttpClientTestingModule],
      providers: [
        { provide: SellerService, useValue: mockSellerService },
        { provide: Router, useValue: mockRouter },
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: new Map([['id', 'seller123']]),
              url: [{ path: 'users' }, { path: 'seller' }]
            }
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(SellerItems);
    component = fixture.componentInstance;
    component.id = 'seller123'; 
  });

  it('should create the component', () => {
    mockSellerService.getItemsBySeller.and.returnValue(of({ success: true, data: [], message:'No items' }));
    fixture.detectChanges();
    expect(component).toBeTruthy();
  });

  it('should navigate to live bid on LiveBids()', () => {
    const item = { itemID: 'item42' } as any;
    mockSellerService.getItemsBySeller.and.returnValue(of({ success: true, data: [], message:'No items'}));

    fixture.detectChanges();
    component.LiveBids(item);

    expect(mockRouter.navigate).toHaveBeenCalledWith(['/live-bid', 'item42']);
  });

  it('should handle error when getItemsBySeller fails', () => {
    const errorResponse = { error: { message: 'Failed to fetch' } };
    mockSellerService.getItemsBySeller.and.returnValue(throwError(() => errorResponse));

    fixture.detectChanges();
    expect(component.items).toEqual([]);
    expect(component.errorMessage).toBe('Failed to fetch');
  });

  

});
