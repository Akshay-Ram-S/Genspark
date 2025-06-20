import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { SellerItems } from '../seller-items/seller-items';
import { BidderBids } from '../bidder-bids/bidder-bids';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-profile-component',
  imports: [SellerItems, BidderBids],
  templateUrl: './profile-component.html',
  styleUrl: './profile-component.css'
})
export class ProfileComponent {
  user: any = null;

  constructor(private router: Router, private authService: AuthService) {}

  ngOnInit(): void {
    this.authService.getMe().subscribe({
      next: (res) => {
        this.user = res.data;
      },
      error: (err) => {
        this.router.navigate(['/login']);
      }
    });
  }

  onEdit(): void {
    this.router.navigate(['/edit-profile']);
  }

  isSeller(): boolean {
    return this.user?.role?.toLowerCase() === 'seller';
  }

}
