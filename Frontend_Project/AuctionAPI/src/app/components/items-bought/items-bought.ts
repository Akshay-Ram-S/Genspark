import { Component, Input, OnInit } from '@angular/core';

import { CommonModule } from '@angular/common';
import { ItemComponent } from '../item-component/item-component';
import { BidderService } from '../../services/bidder.service';

@Component({
  selector: 'app-items-bought',
  templateUrl: './items-bought.html',
  styleUrl: './items-bought.css',
  standalone: true,
  imports: [CommonModule, ItemComponent]
})
export class ItemsBought implements OnInit{
  @Input() bidderId: string = '';
  items: any[] = [];
  errorMessage: string = '';

  ngOnInit(): void {
    this.getItems();
  }

  constructor(private bidderService: BidderService,
  ){
  }

  getItems(){
    this.bidderService.getItemsBought(this.bidderId).subscribe({
      next: (res) => {
        this.items = res.data;
        console.log(res.data);
      },
      error: (err) => {
        console.error('Error loading seller profile', err);
      }
    })
   
  }
  
}
