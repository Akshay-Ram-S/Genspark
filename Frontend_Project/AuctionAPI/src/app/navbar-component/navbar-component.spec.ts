import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Navbar } from './navbar-component';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';
import { Component } from '@angular/core';
import { RouterTestingModule } from '@angular/router/testing';

@Component({
  selector: 'app-notification',
  standalone: true,
  template: ''
})
class StubNotificationComponent {}

describe('NavbarComponent', () => {
  let component: Navbar;
  let fixture: ComponentFixture<Navbar>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let router: Router;

  beforeEach(async () => {
    authServiceSpy = jasmine.createSpyObj('AuthService', ['isAuthenticated', 'logout']);

    await TestBed.configureTestingModule({
      imports: [Navbar, StubNotificationComponent, RouterTestingModule.withRoutes([])],
      providers: [
        { provide: AuthService, useValue: authServiceSpy }
      ]
    }).compileComponents();

    router = TestBed.inject(Router);
    fixture = TestBed.createComponent(Navbar);
    component = fixture.componentInstance;
    localStorage.clear();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should return true from isLoggedIn when authenticated', () => {
    authServiceSpy.isAuthenticated.and.returnValue(true);
    expect(component.isLoggedIn).toBeTrue();
  });

  it('should return false from isLoggedIn when not authenticated', () => {
    authServiceSpy.isAuthenticated.and.returnValue(false);
    expect(component.isLoggedIn).toBeFalse();
  });

  it('should call AuthService.logout and navigate to /login', () => {
    spyOn(router, 'navigate');
    authServiceSpy.logout.and.stub();

    component.logout();

    expect(authServiceSpy.logout).toHaveBeenCalled();
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should set isAdmin to true if token has admin role', () => {
    const token = createFakeTokenWithRole('admin');
    localStorage.setItem('token', token);
    component.ngOnInit();
    expect(component.isAdmin).toBeTrue();
  });

  it('should set isSeller to true if token has seller role', () => {
    const token = createFakeTokenWithRole('seller');
    localStorage.setItem('token', token);
    component.ngOnInit();
    expect(component.isSeller).toBeTrue();
  });

  it('should leave isAdmin and isSeller false if no token is present', () => {
    localStorage.removeItem('token');
    component.ngOnInit();
    expect(component.isAdmin).toBeFalse();
    expect(component.isSeller).toBeFalse();
  });
});

function createFakeTokenWithRole(role: string): string {
  const base64 = (obj: any) => btoa(JSON.stringify(obj));
  const header = base64({ alg: 'HS256', typ: 'JWT' });
  const payload = base64({ role });
  const signature = 'signature';
  return `${header}.${payload}.${signature}`;
}
