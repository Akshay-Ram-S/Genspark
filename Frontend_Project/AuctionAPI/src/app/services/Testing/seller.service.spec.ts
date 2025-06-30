import { TestBed } from '@angular/core/testing';
import { SellerService } from '../seller.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TokenService } from '../token.service';

describe('SellerService', () => {
  let service: SellerService;
  let httpMock: HttpTestingController;
  let tokenServiceSpy: jasmine.SpyObj<TokenService>;
  const baseUrl = 'http://localhost:5205/api/v1/Seller';

  beforeEach(() => {
    tokenServiceSpy = jasmine.createSpyObj('TokenService', ['getUserId']);
    tokenServiceSpy.getUserId.and.returnValue('mock-user-id');

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        SellerService,
        { provide: TokenService, useValue: tokenServiceSpy }
      ]
    });

    service = TestBed.inject(SellerService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get items by seller with explicit ID', () => {
    const sellerId = 'seller123';
    const mockResponse = { success: true, data: [{ itemID: '1' }] };

    service.getItemsBySeller(sellerId).subscribe(res => {
      expect(res.data.length).toBe(1);
    });

    const req = httpMock.expectOne(`${baseUrl}/Items/${sellerId}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should get items by seller using tokenService when ID is not passed', () => {
    localStorage.setItem('token', 'mock-token');
    const mockResponse = { success: true, data: [{ itemID: '1' }] };

    service.getItemsBySeller('').subscribe(res => {
      expect(res.data.length).toBe(1);
    });

    const req = httpMock.expectOne(`${baseUrl}/Items/mock-user-id`);
    expect(req.request.method).toBe('GET');
    expect(req.request.headers.get('Authorization')).toBe('Bearer mock-token');
    req.flush(mockResponse);
  });

  it('should get all sellers', () => {
    const mockResponse = { success: true, data: [{ id: 's1', name: 'Seller 1' }] };

    service.getSellers().subscribe(res => {
      expect(res.data.length).toBe(1);
      expect(res.data[0].name).toBe('Seller 1');
    });

    const req = httpMock.expectOne(baseUrl);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should get seller by ID', () => {
    const sellerId = 's1';
    const mockResponse = { success: true, data: { id: 's1', name: 'Seller 1' } };

    service.getSellerById(sellerId).subscribe(res => {
      expect(res.data.name).toBe('Seller 1');
    });

    const req = httpMock.expectOne(`${baseUrl}/${sellerId}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });
});
