import { Component, Input } from '@angular/core';
import { UserService } from '../services/userService';
import { CommonModule } from '@angular/common';
import { User } from '../models/userModel';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './user-list.html',
  styleUrl: './user-list.css'
})
export class UserListComponent {
  @Input() users: User[] = [];
  @Input() searchTerm = '';
}
