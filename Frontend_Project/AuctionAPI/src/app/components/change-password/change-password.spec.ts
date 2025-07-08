import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { ChangePassword } from './change-password';
import { UserService } from '../services/user.service';
import { of, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';

describe('ChangePassword', () => {
  let component: ChangePassword;
  let fixture: ComponentFixture<ChangePassword>;
  let userServiceSpy: jasmine.SpyObj<UserService>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    userServiceSpy = jasmine.createSpyObj('UserService', ['update']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [ChangePassword, ReactiveFormsModule],
      providers: [
        { provide: UserService, useValue: userServiceSpy },
        { provide: Router, useValue: routerSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ChangePassword);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize form with required fields', () => {
    const form = component.changeForm;
    expect(form).toBeTruthy();
    expect(form.controls['oldPassword']).toBeTruthy();
    expect(form.controls['newPassword']).toBeTruthy();
    expect(form.controls['confirmPassword']).toBeTruthy();
  });

  it('should mark form as invalid if required fields are empty', () => {
    component.changeForm.setValue({
      oldPassword: '',
      newPassword: '',
      confirmPassword: ''
    });
    expect(component.changeForm.valid).toBeFalse();
  });

  it('should show mismatch error when passwords do not match', () => {
    component.changeForm.setValue({
      oldPassword: 'old',
      newPassword: 'newPass123',
      confirmPassword: 'differentPass'
    });
    expect(component.changeForm.errors?.['mismatch']).toBeTrue();
  });

  it('should toggle password visibility flags', () => {
    component.toggle('old');
    expect(component.showOld).toBeTrue();
    component.toggle('new');
    expect(component.showNew).toBeTrue();
    component.toggle('confirm');
    expect(component.showConfirm).toBeTrue();
  });

  it('should call userService.update and reset form on success', fakeAsync(() => {
    const response = { success: true };
    userServiceSpy.update.and.returnValue(of(response));

    component.changeForm.setValue({
      oldPassword: 'oldPass',
      newPassword: 'newPass123',
      confirmPassword: 'newPass123'
    });

    component.onSubmit();
    expect(userServiceSpy.update).toHaveBeenCalledWith({
      CurrentPassword: 'oldPass',
      NewPassword: 'newPass123'
    });

    tick(3000);
    expect(component.successMessage).toBe('');
    expect(routerSpy.navigate).toHaveBeenCalledWith(['profile']);
  }));

  it('should show error message on update failure', fakeAsync(() => {
    const errorResponse = {
      error: { errors: { Exception: ['Something went wrong'] } }
    };
    userServiceSpy.update.and.returnValue(throwError(() => errorResponse));

    component.changeForm.setValue({
      oldPassword: 'oldPass',
      newPassword: 'newPass123',
      confirmPassword: 'newPass123'
    });

    component.onSubmit();
    expect(userServiceSpy.update).toHaveBeenCalled();
    expect(component.errorMessage).toBe('Something went wrong');

    tick(5000);
    expect(component.errorMessage).toBe('');
  }));
});
