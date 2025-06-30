import { Component, Input, OnInit } from '@angular/core';
import { User } from '../models/user';
import { Router } from '@angular/router';
import { UserService } from '../services/user.service';
import { CommonModule } from '@angular/common';
import { TokenService } from '../services/token.service';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './user-component.html',
  styleUrl: './user-component.css'
})
export class UserComponent implements OnInit {
  @Input() user!: User;
  isAdmin: boolean = true;

  constructor(private router: Router, 
              private userService: UserService,
              private tokenService: TokenService) {}

  ngOnInit(): void {
    const role = this.tokenService.getRole();
    this.isAdmin = role?.toLowerCase() === 'admin';
    console.log(this.isAdmin);
  }

  onViewProfile(): void {
    const role = this.user.role.toLowerCase();
    let id: string | undefined;

    if (role === 'seller') {
      id = this.user.sellerId;
    } else if (role === 'bidder') {
      id = this.user.bidderId;
    }

    if (id) {
      this.router.navigate([`/users/${role}/${id}`]);
    } else {
      console.error('Missing sellerId or bidderId for role:', role);
    }
  }

  onToggleStatus(): void {
    const newStatus = this.user.status.toLowerCase() === 'active' ? 'Disabled' : 'Active';

    this.userService.changeState({
      email: this.user.email,
      status: newStatus
    }).subscribe({
      next: (res) => {
        this.user.status = newStatus;
      },
      error: (err) => {
        console.error('Status toggle failed:', err);
        alert('Failed to update status. Please try again.');
      }
    });
  }

}
