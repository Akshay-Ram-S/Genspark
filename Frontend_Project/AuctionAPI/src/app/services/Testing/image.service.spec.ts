import { TestBed } from '@angular/core/testing';
import { ImageService } from '../image.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('ImageService', () => {
  let service: ImageService;
  const baseUrl = 'http://localhost:5205/api/v1/image';

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule], 
      providers: [ImageService]
    });
    service = TestBed.inject(ImageService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should return correct image URL for an item ID', () => {
    const itemId = '123';
    const expectedUrl = `${baseUrl}/View/${itemId}`;
    
    const imageUrl = service.getItemImage(itemId);
    expect(imageUrl).toBe(expectedUrl);
  });
});
