import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Profile } from './profile-component';
import { ActivatedRoute, Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { UserService } from '../services/user.service';
import { SellerService } from '../services/seller.service';
import { BidderService } from '../services/bidder.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('ProfileComponent', () => {
  let component: Profile;
  let fixture: ComponentFixture<Profile>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let userServiceSpy: jasmine.SpyObj<UserService>;
  let sellerServiceSpy: jasmine.SpyObj<SellerService>;
  let bidderServiceSpy: jasmine.SpyObj<BidderService>;
  let routerSpy: jasmine.SpyObj<Router>;

  const routeStub: any = {
    snapshot: {
      paramMap: {
        get: (key: string) => null
      },
      url: []
    }
  };

  const mockUser = {
    role: 'Seller',
    sellerId: 's1',
    bidder: { bidderId: 'b1' }
  };

  beforeEach(async () => {
    authServiceSpy = jasmine.createSpyObj('AuthService', ['getMe']);
    userServiceSpy = jasmine.createSpyObj('UserService', ['delete']);
    sellerServiceSpy = jasmine.createSpyObj('SellerService', ['getSellerById']);
    bidderServiceSpy = jasmine.createSpyObj('BidderService', ['getBidderById']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [Profile, HttpClientTestingModule],
      providers: [
        { provide: ActivatedRoute, useValue: routeStub },
        { provide: Router, useValue: routerSpy },
        { provide: AuthService, useValue: authServiceSpy },
        { provide: UserService, useValue: userServiceSpy },
        { provide: SellerService, useValue: sellerServiceSpy },
        { provide: BidderService, useValue: bidderServiceSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(Profile);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load own profile using getMe', () => {
    authServiceSpy.getMe.and.returnValue(of({ data: mockUser }));

    component.ngOnInit();

    expect(authServiceSpy.getMe).toHaveBeenCalled();
    expect(component.sellerId).toBe('s1');
    expect(component.bidderId).toBe('b1');
  });

  it('should redirect to login if getMe fails', () => {
    authServiceSpy.getMe.and.returnValue(throwError(() => new Error()));

    component.ngOnInit();

    expect(routerSpy.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should return true for seller role', () => {
    component.user = { role: 'seller' };
    expect(component.isSeller()).toBeTrue();
  });

  it('should return true for bidder role', () => {
    component.user = { role: 'bidder' };
    expect(component.isBidder()).toBeTrue();
  });

  it('should return false for other roles', () => {
    component.user = { role: 'admin' };
    expect(component.isSeller()).toBeFalse();
    expect(component.isBidder()).toBeFalse();
  });

  it('should log error on delete failure', () => {
    spyOn(console, 'error');
    userServiceSpy.delete.and.returnValue(throwError(() => new Error('delete failed')));

    component.onDelete();

    expect(console.error).toHaveBeenCalledWith('Error deleting user', jasmine.any(Error));
  });
});
