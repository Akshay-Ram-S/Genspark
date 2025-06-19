import { Component } from '@angular/core';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserService } from '../services/userService';
import { bannedWordsValidator, matchPasswords, validatePassword } from '../validators/customValidator';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, CommonModule],
  templateUrl: './user-form.html',
  styleUrl: './user-form.css'
})
export class UserFormComponent {
  userForm: FormGroup;

  constructor(private userService: UserService) {
    this.userForm = new FormGroup({
    username: new FormControl(null, [Validators.required, bannedWordsValidator(['admin', 'root', 'user'])]),
    email: new FormControl(null, Validators.required),
    password: new FormControl(null, [Validators.required, validatePassword()]), 
    confirmPassword: new FormControl(null),
    role: new FormControl(null, Validators.required),
    }, 
    {
    validators: matchPasswords('password', 'confirmPassword') 
    });
  }

  handleSubmit() {
    if (this.userForm.valid) {
      const { username, email, password, role } = this.userForm.value;
      console.log("User added: ",username, email, password, role);
      this.userService.addUser({ username, email, password, role });
      this.userForm.reset();
    }
  }
}