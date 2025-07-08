import { Component, Input, OnChanges, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { BidderService } from '../../services/bidder.service';


@Component({
  selector: 'app-bidder-bids',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './bidder-bids.html',
  styleUrl: './bidder-bids.css'
})
export class BidderBids implements OnInit {
  @Input() id: string = '';
  bids: any[] = [];
  errorMessage = '';
  isLoading = false;

  constructor(private bidderService: BidderService, public router: Router) {}

  ngOnInit(): void {
    this.isLoading = true;
    this.bidderService.getBidsByBidder(this.id ?? '').subscribe({
      next: (response) => {
        if (response.success) {
          this.bids = response.data;
        } else {
          this.errorMessage = response.message;
        }
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error fetching bids:', error);
        this.errorMessage = 'Failed to load bids.';
        this.isLoading = false;
      }
    });
  }

  ViewItem(itemId:string): void {
    this.router.navigate(['/view-item', itemId]);
  }
}
