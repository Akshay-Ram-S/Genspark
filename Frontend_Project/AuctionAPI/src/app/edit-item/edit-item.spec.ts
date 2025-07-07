import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EditItem } from './edit-item';
import { of, throwError } from 'rxjs';
import { ItemService } from '../services/item.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';

describe('EditItem', () => {
  let component: EditItem;
  let fixture: ComponentFixture<EditItem>;
  let mockItemService: jasmine.SpyObj<ItemService>;
  let mockRouter: jasmine.SpyObj<Router>;
  let mockLocation: jasmine.SpyObj<Location>;

  beforeEach(async () => {
    mockItemService = jasmine.createSpyObj('ItemService', ['getItemById', 'updateItem', 'deleteItem']);
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);
    mockLocation = jasmine.createSpyObj('Location', ['back']);

    await TestBed.configureTestingModule({
      imports: [EditItem],
      providers: [
        { provide: ItemService, useValue: mockItemService },
        { provide: Router, useValue: mockRouter },
        { provide: Location, useValue: mockLocation },
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: {
                get: (key: string) => (key === 'itemId' ? 'item123' : null)
              }
            }
          }
        }
      ]
    }).compileComponents();
  });

  beforeEach(() => {
    const mockItem = {
      itemID: '1',
      title: 'Sample Item',
      description: 'A sample item',
      status: 'Active',
      category: 'Books',
      startingPrice: 100,
      startDate: '2025-06-29T12:00:00Z',
      endDate: '2025-07-05T12:00:00Z',
      currentBid: 150,
      currentBidderName: 'Alice',
      sellerName: 'Bob',
      imageUrl: 'http://example.com/image.jpg'
    };

    mockItemService.getItemById.and.returnValue(of({ success: true, data: mockItem, message: 'Item edited' }));

    fixture = TestBed.createComponent(EditItem);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load item data on init', () => {
    expect(mockItemService.getItemById).toHaveBeenCalledWith('item123');
    expect(component.editForm.value.title).toBe('Sample Item');
  });

  it('should not submit form if invalid', () => {
    component.editForm.controls['title'].setValue('');
    component.onSubmit();
    expect(mockItemService.updateItem).not.toHaveBeenCalled();
  });

  it('should submit form and call updateItem, then go back', () => {
    component.editForm.setValue({
      title: 'Updated Title',
      startingPrice: 100,
      endDate: '2025-08-01',
      category: 'Electronics',
      description: 'Updated description',
      image: null
    });

    mockItemService.updateItem.and.returnValue(of({ success: true }));
    component.onSubmit();

    expect(mockItemService.updateItem).toHaveBeenCalled();
    expect(mockLocation.back).toHaveBeenCalled();
  });

  it('should call deleteItem and navigate to /items on success', () => {
    mockItemService.deleteItem.and.returnValue(of({ success: true }));
    component.deleteItem();
    expect(mockItemService.deleteItem).toHaveBeenCalledWith('item123');
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/items']);
  });

  it('should handle error when deleteItem fails', () => {
    mockItemService.deleteItem.and.returnValue(throwError(() => new Error('delete error')));
    spyOn(console, 'error');
    component.deleteItem();
    expect(console.error).toHaveBeenCalled();
  });
});
