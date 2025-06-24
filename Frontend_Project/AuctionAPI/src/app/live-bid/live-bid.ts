import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Modal } from 'bootstrap';
import { BidService, BidRequest } from '../services/bid.service';
import { ItemService } from '../services/item.service';
import { TokenService } from '../services/token.service';
import { CommonModule } from '@angular/common';

@Component({
  imports: [CommonModule, ReactiveFormsModule],
  selector: 'app-live-bid',
  templateUrl: './live-bid.html',
  styleUrls: ['./live-bid.css']
})
export class LiveBidComponent implements OnInit {
  item: Item | null = null;
  itemId: string | null = null;
  bidForm!: FormGroup;
  successMessage = '';
  errorMessage = '';
  bids: any[] = [];
  isLoading = true;
  highestAmount: number = 0;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder,
    private itemService: ItemService,
    private bidService: BidService,
    private tokenService: TokenService
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
    this.itemId = params.get('itemId');

    if (!this.itemId) {
      return;
    }

    this.itemService.getItemById(this.itemId).subscribe({
      next: (res) => (this.item = res.data),
      error: (err) => console.error('Failed to fetch item', err)
    });

    this.bidForm = this.fb.group({
      bidAmount: [null, [Validators.required, Validators.min(0.1)]]
    });

    this.loadBids();
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
    const modalElement = document.getElementById('confirmBidModal');
    if (modalElement) {
      const modalInstance = Modal.getInstance(modalElement);
      if (modalInstance) {
        modalInstance.hide();
      }
    }
  }

  showMessage(message: string, type: 'success' | 'error') {
    if (type === 'success') {
      this.successMessage = message;
      setTimeout(() => (this.successMessage = ''), 3000);
    } else {
      this.errorMessage = message;
      setTimeout(() => (this.errorMessage = ''), 3000);
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
        this.errorMessage = err.error?.message || 'Error loading bids';
        this.isLoading = false;
      }
    });
  }
}
