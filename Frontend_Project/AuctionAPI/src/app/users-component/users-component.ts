import { Component, OnInit } from '@angular/core';
import { User } from '../models/user';
import { UserComponent } from '../user-component/user-component';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { SellerService } from '../services/seller.service';
import { BidderService } from '../services/bidder.service';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [UserComponent, CommonModule, RouterModule],
  templateUrl: './users-component.html',
  styleUrls: ['./users-component.css']
})
export class UsersComponent implements OnInit {
  users: User[] = [];
  roleType: string = '';

  constructor(
    private sellerService: SellerService,
    private bidderService: BidderService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.roleType = params.get('role') ?? '';

      if (this.roleType === 'seller') {
        this.sellerService.getSellers().subscribe({
          next: (res) => {
            this.users = res.data.map(s => ({
              name: s.user.name,
              email: s.user.email,
              role: s.user.role,
              userId: s.user.userId,
              sellerId: s.sellerId
            }));
          },
          error: (err) => console.error('Error loading sellers:', err)
        });
      } 
      else if (this.roleType === 'bidder') {
        this.bidderService.getBidders().subscribe({
          next: (res) => {
            this.users = res.data.map(b => ({
              name: b.user.name,
              email: b.user.email,
              role: b.user.role,
              userId: b.user.userId,
              bidderId: b.bidderId
            }));
          },
          error: (err) => console.error('Error loading bidders:', err)
        });
      }
    });
  }


}
