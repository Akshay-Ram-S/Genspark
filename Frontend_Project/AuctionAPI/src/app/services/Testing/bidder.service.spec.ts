import { TestBed } from '@angular/core/testing';
import { BidderService } from '../bidder.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TokenService } from '../token.service';

describe('BidderService', () => {
  let service: BidderService;
  let httpMock: HttpTestingController;

  const baseUrl = 'http://localhost:5205/api/v1/Bidder';
  const mockId = 'test-bidder-id';

  const mockApiResponse: ApiResponse<any> = {
    success: true,
    message: 'Success',
    data: { dummy: 'value' }
  };

  const mockArrayResponse: ApiResponse<any[]> = {
    success: true,
    message: 'Success',
    data: [{ dummy: 'value1' }, { dummy: 'value2' }]
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        BidderService,
        {
          provide: TokenService,
          useValue: {} 
        }
      ]
    });

    service = TestBed.inject(BidderService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get all bidders', () => {
    service.getBidders().subscribe(res => {
      expect(res).toEqual(mockArrayResponse);
    });

    const req = httpMock.expectOne(`${baseUrl}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockArrayResponse);
  });

  it('should get bidder by ID', () => {
    service.getBidderById(mockId).subscribe(res => {
      expect(res).toEqual(mockApiResponse);
    });

    const req = httpMock.expectOne(`${baseUrl}/${mockId}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockApiResponse);
  });

  it('should get bids by bidder ID', () => {
    service.getBidsByBidder(mockId).subscribe(res => {
      expect(res).toEqual(mockArrayResponse);
    });

    const req = httpMock.expectOne(`${baseUrl}/Bids/${mockId}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockArrayResponse);
  });

  it('should get items bought by bidder', () => {
    service.getItemsBought(mockId).subscribe(res => {
      expect(res).toEqual(mockApiResponse);
    });

    const req = httpMock.expectOne(`${baseUrl}/Items/${mockId}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockApiResponse);
  });

  it('should handle HTTP error', () => {
    service.getBidderById(mockId).subscribe({
      next: () => fail('Expected error, but got success'),
      error: (error) => {
        expect(error.status).toBe(500);
        expect(error.statusText).toBe('Internal Server Error');
      }
    });

    const req = httpMock.expectOne(`${baseUrl}/${mockId}`);
    req.flush({ message: 'Error' }, { status: 500, statusText: 'Internal Server Error' });
  });
});
