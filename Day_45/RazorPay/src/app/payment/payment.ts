import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, NgZone } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';

@Component({
  selector: 'app-payment',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './payment.html',
  styleUrls: ['./payment.css'],
})
export class Payment implements OnInit {
  paymentForm!: FormGroup;
  isProcessing = false;
  message = '';

  private fb = inject(FormBuilder);
  private zone = inject(NgZone);

  ngOnInit(): void {
    if (!(window as any).Razorpay) {
      console.error('Razorpay script not loaded!');
    }

    this.paymentForm = this.fb.group({
      amount: [null, [Validators.required, Validators.min(1)]],
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      contact: ['', [Validators.required, Validators.pattern(/^\d{10}$/)]],
    });
  }

  showError(controlName: string): string {
    const control = this.paymentForm.get(controlName);
    if (control?.touched && control.invalid) {
      if (control.errors?.['required']) 
        return 'Required';
      if (control.errors?.['email']) 
        return 'Invalid email';
      if (control.errors?.['pattern']) 
        return 'Must be 10 digits';
      if (control.errors?.['min']) 
        return 'Amount must be > 0';
    }
    return '';
  }

  pay() {
    if (this.paymentForm.invalid) {
      this.paymentForm.markAllAsTouched();
      return;
    }

    const form = this.paymentForm.value;

    const options = {
      key: 'rzp_test_1DP5mmOlF5G5ag',
      amount: form.amount * 100,
      currency: 'INR',
      name: form.name,
      description: 'Test UPI Payment',
      prefill: {
        name: form.name,
        email: form.email,
        contact: form.contact,
      },
      method: {
        upi: true,
        card: false,
        netbanking: false,
        wallet: false,
      },
      handler: (response: any) => {
        this.zone.run(() => {
          this.isProcessing = false;
          this.message = `Success Payment ID: ${response.razorpay_payment_id}`;
          this.paymentForm.reset();
        });
      },
      modal: {
        ondismiss: () => {
          this.zone.run(() => {
            this.isProcessing = false;
            this.message = 'Cancelled';
          });
        },
      },
      theme: {
        color: '#006f6d',
      },
    };

    this.isProcessing = true;
    const rzp = new (window as any).Razorpay(options);
    rzp.open();
  }

}