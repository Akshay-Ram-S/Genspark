import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { BidService } from '../../services/bid.service';
import { TokenService } from '../../services/token.service';

@Component({
  selector: 'app-show-bids',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './show-bids.html',
  styleUrl: './show-bids.css'
})
export class ShowBids implements OnInit{
  @Input() bids: any[] = [];
  @Input() highestAmount: number = 0;
  @Input() auctionEnded: boolean = false;
  isAdmin = false;
  selectedBid: any = null;
  showConfirmModal = false;

  constructor(private bidService: BidService,
              private tokenService: TokenService
  ){

  }
  ngOnInit(){
    this.isAdmin = this.tokenService.getRole()?.toLowerCase() == 'admin';
  }

  openConfirmModal(bid: any) {
    console.log('Opening modal for bid:', bid);
    this.selectedBid = bid;
    this.showConfirmModal = true;
  }

  closeConfirmModal() {
    this.showConfirmModal = false;
    this.selectedBid = null;
  }

  confirmDeleteBid() {
    if (!this.selectedBid) 
      return;

    this.bidService.deleteBid(this.selectedBid.bidId).subscribe({
      next: () => {
        this.bids = this.bids.filter(b => b.bidId !== this.selectedBid.bidId);
        this.closeConfirmModal();
        window.location.reload();
      },
      error: (err) => {
        console.error('Delete failed:', err);
        alert('Failed to delete bid');
        this.closeConfirmModal();
      }
    });
  }

}
