import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 'app-bidder-bids',
  imports: [CommonModule],
  templateUrl: './bidder-bids.html',
  styleUrl: './bidder-bids.css'
})
export class BidderBids {
  bids = [
    { item: 'Antique Lamp', amount: 1800 },
    { item: 'Wooden Chair', amount: 950 }
  ];
}
