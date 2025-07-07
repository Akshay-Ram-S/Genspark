import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { SellerService } from '../services/seller.service';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ItemService } from '../services/item.service';
import { ItemComponent } from '../item-component/item-component';

@Component({
  selector: 'app-seller-items',
  imports: [CommonModule, RouterModule, ItemComponent],
  templateUrl: './seller-items.html',
  styleUrl: './seller-items.css'
})

export class SellerItems implements OnInit {
  @Input() id: string='';
  @Input() sellerName: string = '';
  items: Item[] = [];
  @Input() ownProfile: boolean = false;
  public fallbackImage: string = 'https://images.pexels.com/photos/414612/pexels-photo-414612.jpeg';

  constructor(private sellerService: SellerService, 
              private router: Router,
              private route: ActivatedRoute) {}

  errorMessage: string = '';

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    const urlPath = this.route.snapshot.url[1]?.path;

    if (id != null && urlPath == "seller") {
      this.ownProfile = false;
    }
    
    this.sellerService.getItemsBySeller(this.id).subscribe({
      next: (res) => {
        this.items = res.data ?? [];
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

  
  LiveBids(item:Item): void {
    this.router.navigate(['/live-bid', item.itemID]);
  }

}
