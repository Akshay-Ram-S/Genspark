import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Router } from '@angular/router';
import { UserService } from '../services/user.service';
import { RegisterRequest } from '../models/register';

@Component({
  standalone: true,
  selector: 'app-register',
  imports: [CommonModule, ReactiveFormsModule, HttpClientModule],
  templateUrl: './register-component.html',
  styleUrls: ['./register-component.css']
})
export class RegisterComponent {
  registerForm: FormGroup;
  errorMessage = '';

  constructor(private fb: FormBuilder, private http: HttpClient, public userService: UserService, public router: Router) {
    this.registerForm = this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
      pan: ['', Validators.required],
      aadhar: ['', Validators.required],
      role: ['', Validators.required]
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
          this.errorMessage = error.error?.message || 'Registration failed. Please try again.';
        }
      });

    } else {
      this.registerForm.markAllAsTouched(); 
    }
  }

}
