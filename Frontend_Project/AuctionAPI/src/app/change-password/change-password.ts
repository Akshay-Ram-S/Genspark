import { Component, inject } from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule, ValidatorFn, FormGroup, ValidationErrors, AbstractControl } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { UserService } from '../services/user.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-change-password',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './change-password.html',
  styleUrl: './change-password.css'
})
export class ChangePasswordComponent {
  changeForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private router: Router
  ) 
  {
    this.changeForm = this.fb.group({
      oldPassword: ['', Validators.required],
      newPassword: ['', [Validators.required]],
      confirmPassword: ['', Validators.required]
    }, {
      validators: [this.passwordMatchValidator()]
    });
  }

  showOld = false;
  showNew = false;
  showConfirm = false;
  successMessage = '';
  errorMessage = '';

  passwordMatchValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const form = control as FormGroup;
    const newPass = form.get('newPassword')?.value;
    const confirmPass = form.get('confirmPassword')?.value;
    return newPass === confirmPass ? null : { mismatch: true };
  };
}

  toggle(field: 'old' | 'new' | 'confirm') {
    if (field === 'old') 
      this.showOld = !this.showOld;
    if (field === 'new') 
      this.showNew = !this.showNew;
    if (field === 'confirm') 
      this.showConfirm = !this.showConfirm;
  }

  onSubmit() {
    if (this.changeForm.invalid) {
      this.changeForm.markAllAsTouched(); 
      return;
    }

    const formValues = this.changeForm.value;

    const body = {
      CurrentPassword: formValues.oldPassword,
      NewPassword: formValues.newPassword
    };

    this.userService.update(body).subscribe({
      next: (data) => {
        this.successMessage = "Password changed Successfully"
        this.showMessage('Password changed successfully', 'success');
        this.changeForm.reset();
      },
      error: (err) => {
        this.showMessage('Failed to change password', 'error');
        this.errorMessage = err?.error?.errors?.Exception?.[0]|| 'Failed to change password.'
      }
    });    

  }

  showMessage(message: string, type: 'success' | 'error') {
    if (type === 'success') {
      this.successMessage = message;
      setTimeout(() => {
        this.successMessage = '';
        this.router.navigate(["profile"]);
      }, 3000);
    } else {
      this.errorMessage = message;
      setTimeout(() => (this.errorMessage = ''), 5000);
    }

    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
}
