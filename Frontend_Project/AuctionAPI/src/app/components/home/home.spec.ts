import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Home } from './home';
import { AuthService } from '../services/auth.service';
import { TokenService } from '../services/token.service';

describe('Home', () => {
  let component: Home;
  let fixture: ComponentFixture<Home>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;

  beforeEach(async () => {
    const authSpy = jasmine.createSpyObj('AuthService', ['isAuthenticated']);

    await TestBed.configureTestingModule({
      imports: [Home], // ✅ since it's a standalone component
      providers: [
        { provide: AuthService, useValue: authSpy },
        TokenService
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(Home);
    component = fixture.componentInstance;
    authServiceSpy = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
  });

  afterEach(() => {
    localStorage.clear();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should set isAuthenticated and userRole on init', () => {
    const dummyToken = createFakeToken({ role: 'Seller' });
    localStorage.setItem('token', dummyToken);

    authServiceSpy.isAuthenticated.and.returnValue(true);

    component.ngOnInit();

    expect(component.isAuthenticated).toBeTrue();
    expect(component.userRole).toBe('seller');
  });

  it('should set userRole to empty string if no token', () => {
    authServiceSpy.isAuthenticated.and.returnValue(false);

    component.ngOnInit();

    expect(component.isAuthenticated).toBeFalse();
    expect(component.userRole).toBe('');
  });
  
});

function createFakeToken(payload: any): string {
  const header = btoa(JSON.stringify({ alg: 'HS256', typ: 'JWT' }));
  const body = btoa(JSON.stringify(payload));
  return `${header}.${body}.signature`;
}
