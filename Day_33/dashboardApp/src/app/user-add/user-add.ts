import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { UserService } from '../services/user.service';

@Component({
  standalone: true,
  selector: 'app-user-add',
  imports: [CommonModule, ReactiveFormsModule, FormsModule, RouterModule],
  templateUrl: './user-add.html',
  styleUrl: './user-add.css'
})

export class UserAddComponent {
  userForm: FormGroup;

  constructor(private userService: UserService) {
    this.userForm = new FormGroup({
      firstName: new FormControl(null, Validators.required),
      lastName: new FormControl(null, Validators.required),
      gender: new FormControl(null, Validators.required),
      role: new FormControl(null, Validators.required),
      state: new FormControl(null, Validators.required),
    });
  }

  addUser() {
    if (this.userForm.valid) {
      this.userService.addUser(this.userForm.value).subscribe({
      next: (res) => {
        console.log('User added:', res);
        alert('User successfully added!');
        this.userForm.reset();
      },
      error: (err) => {
        console.error('Failed to add user:', err);
        alert('Failed to add user');
      }
    });
    }
  }
}
