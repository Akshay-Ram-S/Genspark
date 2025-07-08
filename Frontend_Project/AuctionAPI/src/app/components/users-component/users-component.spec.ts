import { ComponentFixture, TestBed } from '@angular/core/testing';
import { UsersComponent } from './users-component';
import { ActivatedRoute } from '@angular/router';
import { of, throwError } from 'rxjs';
import { SellerService } from '../services/seller.service';
import { BidderService } from '../services/bidder.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('UsersComponent', () => {
  let component: UsersComponent;
  let fixture: ComponentFixture<UsersComponent>;
  let mockSellerService: jasmine.SpyObj<SellerService>;
  let mockBidderService: jasmine.SpyObj<BidderService>;

  const mockSellerResponse = {
    success: true,
    message: 'Fetched sellers',
    data: [
      {
        sellerId: 's1',
        user: {
          name: 'Seller One',
          email: 'seller1@example.com',
          role: 'seller',
          userId: 'u1',
          status: 'Active'
        }
      }
    ],
    errors: null
  };

  const mockBidderResponse = {
    success: true,
    message: 'Fetched bidders',
    data: [
      {
        bidderId: 'b1',
        user: {
          name: 'Bidder One',
          email: 'bidder1@example.com',
          role: 'bidder',
          userId: 'u2',
          status: 'Inactive'
        }
      }
    ],
    errors: null
  };

  beforeEach(() => {
    mockSellerService = jasmine.createSpyObj('SellerService', ['getSellers']);
    mockBidderService = jasmine.createSpyObj('BidderService', ['getBidders']);
  });

  function setupTest(role: string) {
    TestBed.configureTestingModule({
      imports: [UsersComponent, HttpClientTestingModule],
      providers: [
        { provide: SellerService, useValue: mockSellerService },
        { provide: BidderService, useValue: mockBidderService },
        {
          provide: ActivatedRoute,
          useValue: {
            paramMap: of({
              get: (key: string) => (key === 'role' ? role : null)
            })
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(UsersComponent);
    component = fixture.componentInstance;
  }

  it('should create the component', async () => {
    setupTest('seller');
    fixture.detectChanges();
    expect(component).toBeTruthy();
  });

  it('should load sellers if role is seller', async () => {
    setupTest('seller');
    mockSellerService.getSellers.and.returnValue(of(mockSellerResponse));

    fixture.detectChanges();

    expect(component.roleType).toBe('seller');
    expect(component.users.length).toBe(1);
    expect(component.users[0].name).toBe('Seller One');
  });

  it('should handle seller service error', async () => {
    setupTest('seller');
    const error = new Error('Failed to load sellers');
    mockSellerService.getSellers.and.returnValue(throwError(() => error));

    spyOn(console, 'error');
    fixture.detectChanges();

    expect(console.error).toHaveBeenCalledWith('Error loading sellers:', error);
  });

  it('should load bidders if role is bidder', async () => {
    setupTest('bidder');
    mockBidderService.getBidders.and.returnValue(of(mockBidderResponse));

    fixture.detectChanges();

    expect(component.roleType).toBe('bidder');
    expect(component.users.length).toBe(1);
    expect(component.users[0].name).toBe('Bidder One');
  });

  it('should handle bidder service error', async () => {
    setupTest('bidder');
    const error = new Error('Failed to load bidders');
    mockBidderService.getBidders.and.returnValue(throwError(() => error));

    spyOn(console, 'error');
    fixture.detectChanges();

    expect(console.error).toHaveBeenCalledWith('Error loading bidders:', error);
  });
});
