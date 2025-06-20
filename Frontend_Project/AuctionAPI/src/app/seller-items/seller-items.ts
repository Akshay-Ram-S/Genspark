import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { SellerService } from '../services/seller.service';

@Component({
  selector: 'app-seller-items',
  imports: [CommonModule],
  templateUrl: './seller-items.html',
  styleUrl: './seller-items.css'
})

export class SellerItems implements OnInit {
  items: Item[] = [];
  public fallbackImage: string = 'https://images.pexels.com/photos/414612/pexels-photo-414612.jpeg';

  constructor(private sellerService: SellerService) {}

  errorMessage: string = '';

  ngOnInit(): void {
    this.sellerService.getItemsBySeller().subscribe({
      next: (res) => {
        this.items = res.data ?? [];
        console.log(this.items);
        this.errorMessage = '';
      },
      error: (err) => {
        this.errorMessage = err?.error?.errors?.Exception?.[0] ||
                            err?.error?.message || 
                            'Something went wrong. Please try again.';
        this.items = []
      }
    });
  }

  editItem(item:any): void{
    console.log("Done");
  }
  
}
