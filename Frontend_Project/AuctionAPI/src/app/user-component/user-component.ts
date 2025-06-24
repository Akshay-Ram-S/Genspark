import { Component, Input } from '@angular/core';
import { User } from '../models/user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user',
  imports: [],
  templateUrl: './user-component.html',
  styleUrl: './user-component.css'
})
export class UserComponent {
  @Input() user!: User;

  constructor(private router: Router) {}

  onViewProfile(): void {
    const role = this.user.role.toLowerCase();

    let id: string | undefined;
    if (role === 'seller') {
      id = this.user.sellerId;
    } 
    else if (role === 'bidder') {
      id = this.user.bidderId;
    }

    if (id) {
      this.router.navigate([`/users/${role}/${id}`]);
    } 
    else {
      console.error('Missing sellerId or bidderId for role:', role);
    }
  }
}
