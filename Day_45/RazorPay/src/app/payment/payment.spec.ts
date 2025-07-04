import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Payment } from './payment';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

describe('Payment Component', () => {
  let component: Payment;
  let fixture: ComponentFixture<Payment>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Payment, CommonModule, FormsModule, ReactiveFormsModule],
    }).compileComponents();

    fixture = TestBed.createComponent(Payment);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create component and initialize form', () => {
    expect(component).toBeTruthy();
    expect(component.paymentForm).toBeDefined();
    expect(component.paymentForm.valid).toBeFalse();
  });

  it('should invalidate form if fields are empty', () => {
    component.paymentForm.setValue({
      amount: null,
      name: '',
      email: '',
      contact: '',
    });
    expect(component.paymentForm.valid).toBeFalse();
  });

  it('should validate form with correct input', () => {
    component.paymentForm.setValue({
      amount: 10,
      name: 'Akshay',
      email: 'akshay@gmail.com',
      contact: '9999900000',
    });
    expect(component.paymentForm.valid).toBeTrue();
  });

  it('should return correct error messages from showError()', () => {
    const control = component.paymentForm.get('email');
    control?.markAsTouched();
    control?.setValue('invalid-email');
    expect(component.showError('email')).toBe('Invalid email');

    component.paymentForm.get('contact')?.setValue('123');
    component.paymentForm.get('contact')?.markAsTouched();
    expect(component.showError('contact')).toBe('Must be 10 digits');
  });

  it('should not call Razorpay if form is invalid', () => {
    const rzpSpy = jasmine.createSpy();
    (window as any).Razorpay = rzpSpy;

    component.paymentForm.setValue({
      amount: null,
      name: '',
      email: '',
      contact: '',
    });

    component.pay();
    expect(rzpSpy).not.toHaveBeenCalled();
  });

  it('should open Razorpay if form is valid', () => {
    const openSpy = jasmine.createSpy('open');
    const razorpayConstructorSpy = jasmine.createSpy().and.returnValue({ open: openSpy });
    (window as any).Razorpay = razorpayConstructorSpy;

    component.paymentForm.setValue({
      amount: 10,
      name: 'Akshay',
      email: 'akshay@gmail.com',
      contact: '9999900000',
    });

    component.pay();

    expect(razorpayConstructorSpy).toHaveBeenCalled();
    expect(openSpy).toHaveBeenCalled();
    expect(component.isProcessing).toBeTrue();
  });

  it('should handle Razorpay payment success', () => {
    let capturedOptions: any;

    (window as any).Razorpay = function (options: any) {
      capturedOptions = options;
      return { open: jasmine.createSpy() };
    };

    component.paymentForm.setValue({
      amount: 10,
      name: 'Akshay',
      email: 'akshay@gmail.com',
      contact: '9999900000',
    });

    component.pay();

    expect(capturedOptions).toBeDefined();

    const mockResponse = { razorpay_payment_id: 'pay_123456' };
    capturedOptions.handler(mockResponse);

    expect(component.isProcessing).toBeFalse();
    expect(component.message).toContain('Success Payment ID: pay_123456');
  });


  it('should handle Razorpay modal dismissal', () => {
    let capturedOptions: any;

    (window as any).Razorpay = function (options: any) {
      capturedOptions = options;
      return { open: jasmine.createSpy() };
    };

    component.paymentForm.setValue({
      amount: 10,
      name: 'Akshay',
      email: 'akshay@gmail.com',
      contact: '9999900000',
    });

    component.pay();

    expect(capturedOptions).toBeDefined();

    capturedOptions.modal.ondismiss();

    expect(component.isProcessing).toBeFalse();
    expect(component.message).toBe('Cancelled');
  });



});
