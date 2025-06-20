import { Component, OnInit, OnDestroy } from '@angular/core';
import { ItemService } from '../services/item.service';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { ItemComponent } from '../item-component/item-component';

@Component({
  selector: 'app-items-component',
  standalone: true,
  imports: [CommonModule, ItemComponent, RouterModule, FormsModule],
  templateUrl: './items-component.html',
  styleUrl: './items-component.css'
})
export class ItemsComponent implements OnInit, OnDestroy {
  items: Item[] = [];
  categories: string[] = ['Electronics', 'Furniture', 'Art'];

  searchTerm: string = '';
  selectedCategory: string = '';
  startingPrice: number | null = null;
  endingPrice: number | null = null;
  endDateBefore: string = '';

  private searchSubject = new Subject<string>();
  private searchSub!: Subscription;

  constructor(private itemService: ItemService) {}

  ngOnInit(): void {
    this.fetchFilteredItems();

    this.searchSub = this.searchSubject.pipe(
      debounceTime(1000),
      distinctUntilChanged()
    ).subscribe(search => {
      this.searchTerm = search;
      this.fetchFilteredItems();
    });
  }

  onSearchChange(search: string) {
    this.searchSubject.next(search);
  }

  onFilterChange() {
    this.fetchFilteredItems();
  }

  fetchFilteredItems(): void {
    this.itemService.getFilteredItems({
      search: this.searchTerm,
      category: this.selectedCategory,
      startingPrice: this.startingPrice!,
      endingPrice: this.endingPrice!,
      endDateBefore: this.endDateBefore
    }).subscribe({
      next: (response) => {
        console.log('API Response:', response);
        this.items = response.data ?? [];
      },
      error: (err) => console.error('❌ Filter fetch failed:', err)
    });
  }

  ngOnDestroy(): void {
    this.searchSub.unsubscribe();
  }
}
