import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Modal } from 'bootstrap';
import { BidService, BidRequest } from '../../services/bid.service';
import { ItemService } from '../../services/item.service';
import { TokenService } from '../../services/token.service';
import { CommonModule } from '@angular/common';
import { interval, Subscription } from 'rxjs';
import { ShowBids } from '../show-bids/show-bids';
import { NotificationService } from '../../services/notification.service';

@Component({
  imports: [CommonModule, ReactiveFormsModule, ShowBids],
  selector: 'app-live-bid',
  templateUrl: './live-bid.html',
  styleUrls: ['./live-bid.css']
})

export class LiveBid implements OnInit {
  item: Item | null = null;
  itemId: string | null = null;
  bidForm!: FormGroup;
  successMessage = '';
  errorMessage = '';
  bids: any[] = [];
  isLoading = true;
  highestAmount: number = 0;
  auctionEnded: boolean = false;
  countdown: string = '';
  progressPercent: number = 0;
  userRole: string | null = '';
  private countdownSubscription: Subscription | null = null;

  constructor(
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private itemService: ItemService,
    private bidService: BidService,
    private tokenService: TokenService,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
    this.itemId = params.get('itemId');
    this.userRole = this.tokenService.getRole();
    if (!this.itemId) {
      return;
    }

    this.itemService.getItemById(this.itemId).subscribe({
      next: (res) => {
        this.item = res.data;
        this.auctionEnded = this.hasAuctionEnded(this.item.endDate);
        this.startCountdown();
      },
      error: (err) => console.error('Failed to fetch item', err)
    });

    this.bidForm = this.fb.group({
      bidAmount: [null, [Validators.required, Validators.min(0.1)]]
    });

    this.loadBids();
    this.notificationService.bidPlaced$.subscribe(bid => {
      if (bid.itemId === this.itemId) {
        this.loadBids();
      }
    });
    
  });
  }

  submitBid() {
    if (this.bidForm.valid) {
      const modalEl = document.getElementById('confirmBidModal');
      if (modalEl) {
        const modal = new Modal(modalEl);
        modal.show();
      }
    }
  }

  confirmPlaceBid() {
    const bidAmount = this.bidForm.value.bidAmount;
    const bidderID = this.tokenService.getUserId() ?? '';

    const bidRequest: BidRequest = {
      itemId: this.itemId ?? '',
      bidderId: bidderID,
      Amount: bidAmount
    };

    this.bidService.placeBid(bidRequest).subscribe({
      next: (response) => {
        this.closeConfirmModal();
        if (response.success) {
          this.showMessage('Bid placed successfully!', 'success');
          this.loadBids();
        } else {
          this.showMessage('Failed: ' + response.message, 'error');
          this.loadBids(); 
        }
      },
      error: (err) => {
        this.closeConfirmModal();
        const backendMessage = err.error?.message || 'An unexpected error occurred.';
        this.showMessage(backendMessage, 'error');
      }
    });
  }

  closeConfirmModal() {
    const modal = document.getElementById('confirmBidModal');
    if (modal) {
      const modalInstance = Modal.getInstance(modal);
      if (modalInstance) {
        modalInstance.hide();
      }
    }
  }

  showMessage(message: string, type: 'success' | 'error') {
    if (type === 'success') {
      this.successMessage = message;
      setTimeout(() => (this.successMessage = ''), 6000);
    } else {
      this.errorMessage = message;
      setTimeout(() => (this.errorMessage = ''), 6000);
    }

    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  loadBids() {
    this.itemService.getAllBidsForItem(this.itemId ?? '').subscribe({
      next: res => {
        this.bids = res.data;
        this.highestAmount = Math.max(...this.bids.map(b => b.amount));
        this.isLoading = false;
      },
      error: err => {
        this.isLoading = false;
      }
    });
  }

  hasAuctionEnded(endDate: string | Date): boolean {
    return new Date(endDate) < new Date();
  }


  startCountdown() {
    if (!this.item?.endDate || !this.item?.startDate) {
      this.countdown = 'Auction Ended';
      return;
    }

    const startDateUTC = new Date(this.item.startDate);
    const endDateUTC = new Date(this.item.endDate);

    this.countdownSubscription = interval(1000).subscribe(() => {
      const nowIST = new Date(new Date().toLocaleString('en-US', { timeZone: 'Asia/Kolkata' }));
      const startIST = new Date(startDateUTC.toLocaleString('en-US', { timeZone: 'Asia/Kolkata' }));
      const endIST = new Date(endDateUTC.toLocaleString('en-US', { timeZone: 'Asia/Kolkata' }));

      const total = endIST.getTime() - startIST.getTime();
      const left = endIST.getTime() - nowIST.getTime();

      if (left <= 0) {
        this.countdown = 'Auction Ended';
        this.progressPercent = 0;
        this.auctionEnded = true;
        this.countdownSubscription?.unsubscribe();
        return;
      }

      const days = Math.floor(left / (1000 * 60 * 60 * 24));
      const hours = Math.floor((left % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
      const minutes = Math.floor((left % (1000 * 60 * 60)) / (1000 * 60));
      const seconds = Math.floor((left % (1000 * 60)) / 1000);

      this.countdown = `${days}D ${hours}H ${minutes}M ${seconds}S`;
      this.progressPercent = (left / total) * 100;
    });
  }


  ngOnDestroy() {
    if (this.countdownSubscription) {
      this.countdownSubscription.unsubscribe();
    }
  }

}
