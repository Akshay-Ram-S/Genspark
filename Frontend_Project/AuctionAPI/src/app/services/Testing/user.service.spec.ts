import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { UserService } from '../user.service';
import { RegisterRequest } from '../../models/register';

describe('UserService', () => {
  let service: UserService;
  let httpMock: HttpTestingController;
  const baseUrl = 'http://localhost:5205/api/v1/user';

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [UserService]
    });

    service = TestBed.inject(UserService);
    httpMock = TestBed.inject(HttpTestingController);
    localStorage.clear();
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should register a new user', () => {
    const requestData: RegisterRequest = {
        name: 'Akshay Ram',
        email: 'akshay@gmail.com',
        password: 'akshay123',
        pan: 'ABCDE1234F',
        aadhar: '1234567890',
        role: 'seller'
    };

    const mockResponse = { success: true, message: 'Registered successfully' };

    service.register(requestData).subscribe(res => {
        expect(res).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(baseUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(requestData);
    req.flush(mockResponse);
    });


  it('should delete the user', () => {
    localStorage.setItem('token', 'mock-token');
    const mockResponse = { success: true };

    service.delete().subscribe(res => {
      expect(res).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(baseUrl);
    expect(req.request.method).toBe('DELETE');
    expect(req.request.headers.get('Authorization')).toBe('Bearer mock-token');
    req.flush(mockResponse);
  });

  it('should update the user', () => {
    localStorage.setItem('token', 'mock-token');
    const updateData = { name: 'Updated Name' };
    const mockResponse = { success: true };

    service.update(updateData).subscribe(res => {
      expect(res).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(baseUrl);
    expect(req.request.method).toBe('PUT');
    expect(req.request.headers.get('Authorization')).toBe('Bearer mock-token');
    expect(req.request.body).toEqual(updateData);
    req.flush(mockResponse);
  });

  it('should change state of user', () => {
    localStorage.setItem('token', 'mock-token');
    const stateData = { state: 'Tamil Nadu' };
    const mockResponse = { success: true };

    service.changeState(stateData).subscribe(res => {
      expect(res).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`${baseUrl}/change-state`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.headers.get('Authorization')).toBe('Bearer mock-token');
    expect(req.request.body).toEqual(stateData);
    req.flush(mockResponse);
  });

});
