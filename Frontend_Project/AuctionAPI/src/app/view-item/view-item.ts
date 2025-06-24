import { CommonModule } from "@angular/common";
import { ItemComponent } from "../item-component/item-component";
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { ItemService } from "../services/item.service";
import { ItemAllBids } from "../models/all-bids";

@Component({
  selector: 'app-view-item',
  templateUrl: './view-item.html',
  styleUrls: ['./view-item.css'],
  standalone: true,
  imports: [CommonModule, ItemComponent] 
})
export class ViewItemComponent implements OnInit {
  item!: Item;
  bids: ItemAllBids[] = [];
  isLoading = true;
  errorMessage = '';
  highestAmount: number = 0;

  constructor(private route: ActivatedRoute, private itemService: ItemService) {}

  ngOnInit(): void {
    const itemId = this.route.snapshot.paramMap.get('id')!;
    this.loadItem(itemId);
    this.loadBids(itemId);
  }

  loadItem(id: string) {
    this.itemService.getItemById(id).subscribe({
      next: res => this.item = res.data,
      error: err => this.errorMessage = 'Failed to load item.'
    });
  }

  loadBids(id: string) {
    this.itemService.getAllBidsForItem(id).subscribe({
      next: res => {
        this.bids = res.data;
        //console.log(res.data);
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
