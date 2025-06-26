import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-show-bids',
  imports: [CommonModule],
  templateUrl: './show-bids.html',
  styleUrl: './show-bids.css'
})
export class ShowBids {
  @Input() bids: any[] = [];
  @Input() highestAmount: number = 0;
  @Input() auctionEnded: boolean = false;
}
