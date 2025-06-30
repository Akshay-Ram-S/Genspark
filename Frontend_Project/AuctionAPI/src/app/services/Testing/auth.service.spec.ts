import { TestBed } from '@angular/core/testing';
import { AuthService } from '../auth.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { NotificationService } from '../notification.service';
import { LoginRequest } from '../../models/login-request';


describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;
  let notificationServiceSpy: jasmine.SpyObj<NotificationService>;

  const baseUrl = 'http://localhost:5205/api/v1/auth';

  beforeEach(() => {
    const spy = jasmine.createSpyObj('NotificationService', ['clearNotifications']);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        AuthService,
        { provide: NotificationService, useValue: spy }
      ]
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
    notificationServiceSpy = TestBed.inject(NotificationService) as jasmine.SpyObj<NotificationService>;

  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();

  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should send login request and return response', () => {
    const mockLogin: LoginRequest = { email: 'user@example.com', password: 'pass123' };
    const mockResponse = { token: 'token123', refreshToken: 'refreshToken123' };

    service.login(mockLogin).subscribe(res => {
      expect(res).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`${baseUrl}/login`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(mockLogin);

    req.flush(mockResponse);
  });

  it('should call getMe with Authorization header', () => {
    const token = 'test-token';
    localStorage.setItem('token', token);

    const mockUser = { id: '1', name: 'Test User' };

    service.getMe().subscribe(res => {
      expect(res).toEqual(mockUser);
    });

    const req = httpMock.expectOne(`${baseUrl}/me`);
    expect(req.request.method).toBe('GET');
    expect(req.request.headers.get('Authorization')).toBe(`Bearer ${token}`);

    req.flush(mockUser);
  });

  it('should return true when token is present', () => {
    localStorage.setItem('token', 'valid-token');
    expect(service.isAuthenticated()).toBeTrue();
  });

  it('should return false when token is not present', () => {
    localStorage.removeItem('token');
    expect(service.isAuthenticated()).toBeFalse();
  });



});
