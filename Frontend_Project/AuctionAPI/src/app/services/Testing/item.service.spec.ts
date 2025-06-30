import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ItemService } from '../item.service';
import { ItemAllBids } from '../../models/all-bids';

describe('ItemService', () => {
  let service: ItemService;
  let httpMock: HttpTestingController;
  const baseUrl = 'http://localhost:5205/api/v1/Items';

  const mockItem: Item = {
    itemID: '1',
    title: 'Test Item',
    description: 'A nice item',
    status: 'Active',
    category: 'Electronics',
    startingPrice: 100,
    startDate: '2025-06-01T00:00:00Z',
    endDate: '2025-07-01T00:00:00Z',
    currentBid: 150,
    currentBidderName: 'John Doe',
    sellerName: 'Seller A',
    imageUrl: 'http://example.com/image.jpg'
  };

  const mockBids: ItemAllBids[] = [
    {
      title: 'Test Item',
      bidder_id: 'bidder123',
      name: 'John Doe',
      amount: 150,
      bid_timestamp: '2025-06-29T10:00:00Z'
    }
  ];

  const mockFormData = new FormData();

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ItemService]
    });

    service = TestBed.inject(ItemService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get filtered items with query params', () => {
    const filters = {
      search: 'item',
      category: 'Electronics',
      startingPrice: 100,
      endingPrice: 200,
      endDateBefore: '2025-07-01',
      page: 1,
      pageSize: 10
    };

    const mockResponse = {
      success: true,
      message: 'Items fetched',
      data: [mockItem],
      pagination: {
        currentPage: 1,
        pageSize: 10,
        totalPages: 1,
        totalCount: 1
      },
      errors: null
    };

    service.getFilteredItems(filters).subscribe(res => {
      expect(res.data.length).toBe(1);
      expect(res.data[0]).toEqual(mockItem);
    });

    const req = httpMock.expectOne(req => req.url === baseUrl);
    expect(req.request.method).toBe('GET');
    expect(req.request.params.get('category')).toBe('Electronics');
    req.flush(mockResponse);
  });

  it('should get item by ID', () => {
    const mockResponse = { success: true, message: 'OK', data: mockItem };

    service.getItemById('1').subscribe(res => {
      expect(res.data).toEqual(mockItem);
    });

    const req = httpMock.expectOne(`${baseUrl}/1`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should get all bids for an item', () => {
    const mockResponse = { success: true, message: 'OK', data: mockBids };

    service.getAllBidsForItem('1').subscribe(res => {
      expect(res.data).toEqual(mockBids);
    });

    const req = httpMock.expectOne(`${baseUrl}/all-bids/1`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should post an item with Authorization header', () => {
    localStorage.setItem('token', 'token123');

    service.postItem(mockFormData).subscribe(res => {
      expect(res).toEqual({ success: true });
    });

    const req = httpMock.expectOne(baseUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.headers.get('Authorization')).toBe('Bearer token123');
    req.flush({ success: true });
  });

  it('should update an item with Authorization header', () => {
    localStorage.setItem('token', 'token123');

    service.updateItem('1', mockFormData).subscribe(res => {
      expect(res).toEqual({ success: true });
    });

    const req = httpMock.expectOne(`${baseUrl}/1`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.headers.get('Authorization')).toBe('Bearer token123');
    req.flush({ success: true });
  });

  it('should delete an item with Authorization header', () => {
    localStorage.setItem('token', 'token123');

    service.deleteItem('1').subscribe(res => {
      expect(res).toEqual({ success: true });
    });

    const req = httpMock.expectOne(`${baseUrl}/1`);
    expect(req.request.method).toBe('DELETE');
    expect(req.request.headers.get('Authorization')).toBe('Bearer token123');
    req.flush({ success: true });
  });

  it('should handle HTTP error for getItemById', () => {
    service.getItemById('1').subscribe({
      next: () => fail('Expected an error'),
      error: err => {
        expect(err.status).toBe(404);
        expect(err.statusText).toBe('Not Found');
      }
    });

    const req = httpMock.expectOne(`${baseUrl}/1`);
    req.flush({ message: 'Item not found' }, { status: 404, statusText: 'Not Found' });
  });

});
