import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Router } from '@angular/router';
import { UserService } from '../services/user.service';
import { RegisterRequest } from '../models/register';
import { aadharValidator, matchPasswords, panValidator } from '../validators/customValidator';

@Component({
  standalone: true,
  selector: 'app-register',
  imports: [CommonModule, ReactiveFormsModule, HttpClientModule],
  templateUrl: './register-component.html',
  styleUrls: ['./register-component.css']
})
export class Register {
  registerForm: FormGroup;
  errorMessage = '';
  showPassword = false;

  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }

  constructor(private fb: FormBuilder, private http: HttpClient, public userService: UserService, public router: Router) {
    this.registerForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required],
      phone: ['', [Validators.required, Validators.pattern(/^[6-9]\d{9}$/)] ],
      pan: ['', [Validators.required, panValidator]],
      aadhar: ['', [Validators.required, aadharValidator ]],
      role: ['', Validators.required]
    },
    {
      validators: matchPasswords('password', 'confirmPassword')
    });
  }

  isInvalid(controlName: string): boolean {
    const control = this.registerForm.get(controlName);
    return !!control && control.invalid && control.touched;
  }

  onSubmit(): void {
    if (this.registerForm.valid) {
      const formData: RegisterRequest = this.registerForm.value;
      console.log('Registering user:', formData);

      this.userService.register(formData).subscribe({
        next: (response) => {
          console.log('Registration successful:', response);
          this.router.navigate(['/login']);
        },
        error: (error) => {
          console.error('Registration failed:', error);
          const exceptionList = error.error?.errors?.Exception;

          this.errorMessage = Array.isArray(exceptionList) && exceptionList.length
            ? exceptionList[0]
            : (error.error?.message || 'Registration failed. Please try again.');

          setTimeout(() => this.errorMessage = '', 5000);
        }
      });

    } else {
      this.registerForm.markAllAsTouched(); 
    }
  }

}
