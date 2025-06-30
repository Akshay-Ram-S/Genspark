import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ItemComponent } from './item-component';
import { TokenService } from '../services/token.service';
import { AuthService } from '../services/auth.service';
import { ImageService } from '../services/image.service';
import { Router } from '@angular/router';
import { By } from '@angular/platform-browser';

describe('ItemComponent', () => {
  let component: ItemComponent;
  let fixture: ComponentFixture<ItemComponent>;
  let mockTokenService: jasmine.SpyObj<TokenService>;
  let mockAuthService: jasmine.SpyObj<AuthService>;
  let mockImageService: jasmine.SpyObj<ImageService>;
  let mockRouter: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    mockTokenService = jasmine.createSpyObj('TokenService', ['getRole']);
    mockAuthService = jasmine.createSpyObj('AuthService', ['isAuthenticated']);
    mockImageService = jasmine.createSpyObj('ImageService', ['getItemImage']);
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [ItemComponent],
      providers: [
        { provide: TokenService, useValue: mockTokenService },
        { provide: AuthService, useValue: mockAuthService },
        { provide: ImageService, useValue: mockImageService },
        { provide: Router, useValue: mockRouter }
      ]
    }).compileComponents();
  });

  beforeEach(() => {
    mockTokenService.getRole.and.returnValue('seller');
    mockAuthService.isAuthenticated.and.returnValue(true);
    mockImageService.getItemImage.and.returnValue('https://example.com/image.jpg');
    localStorage.setItem('token', btoa(JSON.stringify({})) + '.' + btoa(JSON.stringify({ role: 'admin' })) + '.abc');

    fixture = TestBed.createComponent(ItemComponent);
    component = fixture.componentInstance;

    component.item = {
      itemID: 'item123',
      title: 'Test Item'
    };

    fixture.detectChanges();
  });

  afterEach(() => {
    localStorage.removeItem('token');
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize with imageUrl and role', () => {
    expect(component.imageUrl).toBe('https://example.com/image.jpg');
    expect(component.isAdmin).toBeTrue();
    expect(component.isAuthenticated).toBeTrue();
    expect(component.currentUserRole).toBe('seller');
  });

  it('should navigate to live bid page when onPlaceBid is called and user is authenticated', () => {
    component.onPlaceBid();
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/items/live-bid', 'item123']);
  });

  it('should redirect to login if not authenticated when placing bid', () => {
    mockAuthService.isAuthenticated.and.returnValue(false);
    component.isAuthenticated = false;

    component.onPlaceBid();
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should navigate to view-item page on onViewAllBids', () => {
    component.onViewAllBids();
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/view-item', 'item123']);
  });

  it('should navigate to edit page on editItem()', () => {
    component.editItem();
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/items/edit-item', 'item123']);
  });

  it('should set imageLoaded to true on image load', () => {
    component.imageLoaded = false;
    component.onImageLoad();
    expect(component.imageLoaded).toBeTrue();
  });
});
