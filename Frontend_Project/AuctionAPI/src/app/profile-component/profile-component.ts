import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { SellerItems } from '../seller-items/seller-items';
import { BidderBids } from '../bidder-bids/bidder-bids';
import { CommonModule } from '@angular/common';
import { ItemsBought } from '../items-bought/items-bought';
import { AuthService } from '../services/auth.service';
import { UserService } from '../services/user.service';
import { SellerService } from '../services/seller.service';
import { BidderService } from '../services/bidder.service';

@Component({
  selector: 'app-profile-component',
  standalone: true,
  imports: [SellerItems, BidderBids, CommonModule, RouterModule,ItemsBought],
  templateUrl: './profile-component.html',
  styleUrl: './profile-component.css'
})
export class Profile implements OnInit {
  user: any = null;
  isOwnProfile: boolean = true;
  bidderId: string | null = '';
  sellerId: string | null = '';


  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private authService: AuthService,
    private userService: UserService,
    private sellerService: SellerService,
    private bidderService: BidderService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    const urlPath = this.route.snapshot.url[1]?.path;

    if (id && urlPath) {
      this.isOwnProfile = false;

      if (urlPath === 'seller') {
        this.sellerService.getSellerById(id).subscribe({
          next: (res) => {
            this.user = res.data;
            this.user.role = res.data.user.role.toLowerCase();
            this.sellerId = res.data.sellerId;
            this.user.email = res.data.user.email;
            this.user.name = res.data.user.name;
            this.user.phone = res.data.user.phone;
          },
          error: (err) => console.error('Error loading seller profile', err)
        });
      } 
      else if (urlPath === 'bidder') {
        this.bidderService.getBidderById(id).subscribe({
          next: (res) => {
            this.user = res.data;
            this.user.role = res.data.user.role.toLowerCase();
            this.bidderId = res.data.bidderId;
            this.user.email = res.data.user.email;
            this.user.name = res.data.user.name;
            this.user.phone = res.data.user.phone;
          },
          error: (err) => console.error('Error loading bidder profile', err)
        });
      }
    } 
    else {
      this.authService.getMe().subscribe({
        next: (res) => {
          this.user = res.data;
          this.sellerId = res.data?.sellerId ?? '';
          this.bidderId = res.data?.bidder?.bidderId ?? '';
        },
        error: () => this.router.navigate(['/login'])
      });
    }
  }
  

  changePassword(): void {
    this.router.navigate(['/profile/change-password']);
  }

  confirmDelete(): void {
    this.userService.delete().subscribe({
      next: () => {
        localStorage.removeItem('token');
        this.router.navigate(['/login']);
        window.location.reload();
      },
      error: (err) => console.error('Error deleting user', err)
    });
  }

  isSeller(): boolean {
    return this.user?.role?.toLowerCase() === 'seller';
  }
  isBidder(): boolean {
    return this.user?.role?.toLowerCase() === 'bidder';
  }
}
