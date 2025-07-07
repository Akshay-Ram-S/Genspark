import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { PostItem } from './post-item';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { ItemService } from '../services/item.service';
import { TokenService } from '../services/token.service';

describe('PostItem', () => {
  let component: PostItem;
  let fixture: ComponentFixture<PostItem>;
  let itemServiceSpy: jasmine.SpyObj<ItemService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let tokenServiceSpy: jasmine.SpyObj<TokenService>;

  beforeEach(async () => {
    itemServiceSpy = jasmine.createSpyObj('ItemService', ['postItem']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    tokenServiceSpy = jasmine.createSpyObj('TokenService', ['getUserId']);

    await TestBed.configureTestingModule({
      imports: [PostItem],
      providers: [
        { provide: ItemService, useValue: itemServiceSpy },
        { provide: Router, useValue: routerSpy },
        { provide: TokenService, useValue: tokenServiceSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(PostItem);
    component = fixture.componentInstance;
    tokenServiceSpy.getUserId.and.returnValue('user-123');
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should not submit form if invalid', () => {
    component.postForm.controls['title'].setValue('');
    component.onSubmit();
    expect(itemServiceSpy.postItem).not.toHaveBeenCalled();
  });

  it('should call postItem and navigate on successful submit', () => {
    component.postForm.setValue({
      title: 'Item Name',
      startingPrice: 100,
      endDate: '2025-08-01',
      category: 'Electronics',
      description: 'Some valid description',
      image: null
    });

    itemServiceSpy.postItem.and.returnValue(of({ success: true }));
    component.onSubmit();

    expect(itemServiceSpy.postItem).toHaveBeenCalled();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/items']);
  });

  it('should set errorMessage when post fails', () => {
    const errorResponse = {
      error: { errors: { Exception: ['Something went wrong'] } }
    };

    itemServiceSpy.postItem.and.returnValue(throwError(() => errorResponse));
    component.postForm.setValue({
      title: 'Item Name',
      startingPrice: 100,
      endDate: '2025-08-01',
      category: 'Electronics',
      description: 'Some valid description',
      image: null
    });

    component.onSubmit();
    expect(component.errorMessage).toBe('Something went wrong');
  });

  describe('onImageSelected', () => {

    it('should reject invalid file type', () => {
      const file = new File([''], 'test.txt', { type: 'text/plain' });
      const event = { target: { files: [file] } } as unknown as Event;

      component.onImageSelected(event);
      expect(component.errorMessage).toContain('Only image files');
      expect(component.imageFile).toBeNull();
    });

    it('should do nothing if no file selected', () => {
      const event = { target: { files: [] } } as unknown as Event;
      component.imageFile = null;
      component.onImageSelected(event);
      expect(component.imageFile).toBeNull();
    });
  });
});
