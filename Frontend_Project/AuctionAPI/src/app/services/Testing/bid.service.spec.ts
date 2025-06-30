import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { BidRequest, BidService } from '../bid.service';

describe('BidService', () => {
  let service: BidService;
  let httpMock: HttpTestingController;

  const mockBid: BidRequest = {
    itemId: 'item123',
    bidderId: 'bidder456',
    Amount: 500
  };

  const mockResponse = {
    success: true,
    message: 'Bid placed successfully',
    data: {}
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [BidService]
    });

    service = TestBed.inject(BidService);
    httpMock = TestBed.inject(HttpTestingController);
    localStorage.setItem('token', 'test-token'); 
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear(); 
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should call POST and return ApiResponse with Authorization header', () => {
    service.placeBid(mockBid).subscribe(response => {
      expect(response).toEqual(mockResponse);
    });

    const req = httpMock.expectOne('http://localhost:5205/api/v1/Bid');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(mockBid);

    const authHeader = req.request.headers.get('Authorization');
    expect(authHeader).toBe('Bearer test-token');

    req.flush(mockResponse);
  });

  it('should handle HTTP error response correctly', () => {
    const errorMessage = 'Internal Server Error';

    service.placeBid(mockBid).subscribe({
      next: () => fail('Expected an error, not a response'),
      error: (error: HttpErrorResponse) => {
        expect(error.status).toBe(500);
        expect(error.statusText).toBe('Internal Server Error');
      }
    });

    const req = httpMock.expectOne('http://localhost:5205/api/v1/Bid');
    expect(req.request.method).toBe('POST');

    req.flush(
      { message: errorMessage },
      { status: 500, statusText: 'Internal Server Error' }
    );
  });

});
