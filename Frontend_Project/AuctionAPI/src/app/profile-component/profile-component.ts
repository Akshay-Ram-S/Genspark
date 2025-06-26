import { Component, ElementRef, ViewChild, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { SellerItems } from '../seller-items/seller-items';
import { BidderBids } from '../bidder-bids/bidder-bids';
import { AuthService } from '../services/auth.service';
import { SellerService } from '../services/seller.service';
import { BidderService } from '../services/bidder.service';
import { UserService } from '../services/user.service';
import { Modal } from 'bootstrap';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-profile-component',
  standalone: true,
  imports: [SellerItems, BidderBids, CommonModule, RouterModule],
  templateUrl: './profile-component.html',
  styleUrl: './profile-component.css'
})
export class ProfileComponent implements OnInit {
  user: any = null;
  isOwnProfile: boolean = true;
  bidderId: string = '';
  sellerId: string = '';

  @ViewChild('deleteModal', { static: false }) deleteModal!: ElementRef;

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
          },
          error: (err) => console.error('Error loading bidder profile', err)
        });
      }
    } 
    else {
      this.authService.getMe().subscribe({
        next: (res) => {
          this.user = res.data;
          this.sellerId = res.data.sellerId;
          this.bidderId = res.data.bidder.bidderId;
        },
        error: () => this.router.navigate(['/login'])
      });
    }
  }

  openModal(): void {
    if (this.deleteModal?.nativeElement) {
      const modal = new Modal(this.deleteModal.nativeElement);
      modal.show();
    }
  }

  changePassword(): void {
    this.router.navigate(['/profile/change-password']);
  }

  onDelete(): void {
    this.userService.delete().subscribe({
      next: () => {
        localStorage.removeItem('token');
        alert('User deleted successfully');
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
