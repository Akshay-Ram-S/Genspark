import { ComponentFixture, TestBed } from '@angular/core/testing';
import { UserComponent } from './user-component';
import { Router } from '@angular/router';
import { UserService } from '../services/user.service';
import { of, throwError } from 'rxjs';
import { User } from '../models/user';

describe('UserComponent', () => {
  let component: UserComponent;
  let fixture: ComponentFixture<UserComponent>;
  let mockRouter: jasmine.SpyObj<Router>;
  let mockUserService: jasmine.SpyObj<UserService>;

  const sampleUser: User = {
    name: 'Ram',
    email: 'ram@gmail.com',
    role: 'seller',
    userId: '101',
    sellerId: '102',
    status: 'Active'
  };

  beforeEach(async () => {
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);
    mockUserService = jasmine.createSpyObj('UserService', ['changeState']);

    await TestBed.configureTestingModule({
      imports: [UserComponent],
      providers: [
        { provide: Router, useValue: mockRouter },
        { provide: UserService, useValue: mockUserService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(UserComponent);
    component = fixture.componentInstance;
    component.user = { ...sampleUser };
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should set isAdmin to false for seller', () => {
    expect(component.isAdmin).toBeFalse();
  });

  it('should navigate to seller profile onViewProfile', () => {
    component.onViewProfile();
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/users/seller/102']);
  });

  it('should navigate to bidder profile if role is bidder', () => {
    component.user = { ...sampleUser, role: 'bidder', bidderId: '123' };
    fixture.detectChanges();

    component.onViewProfile();
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/users/bidder/123']);
  });

  it('should log error if ID is missing in onViewProfile', () => {
    spyOn(console, 'error');
    component.user = { ...sampleUser, role: 'seller', sellerId: undefined };
    fixture.detectChanges();

    component.onViewProfile();
    expect(console.error).toHaveBeenCalledWith('Missing sellerId or bidderId for role:', 'seller');
  });

  it('should toggle status to Disabled on success', () => {
    mockUserService.changeState.and.returnValue(of({ success: true }));

    component.onToggleStatus();

    expect(component.user.status).toBe('Disabled');
    expect(mockUserService.changeState).toHaveBeenCalledWith({
      email: component.user.email,
      status: 'Disabled'
    });
  });

  it('should handle toggle status failure', () => {
    const error = new Error('Failed');
    spyOn(console, 'error');
    spyOn(window, 'alert');

    mockUserService.changeState.and.returnValue(throwError(() => error));

    component.onToggleStatus();

    expect(console.error).toHaveBeenCalledWith('Status toggle failed:', error);
    expect(window.alert).toHaveBeenCalledWith('Failed to update status. Please try again.');
  });
});
