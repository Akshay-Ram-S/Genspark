import { Component, OnInit, OnDestroy } from '@angular/core';
import { ItemService } from '../../services/item.service';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
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
  categories: string[] = [
    'Electronics',
    'Fashion & Apparel',
    'Home & Furniture',
    'Collectibles & Antiques',
    'Automotive',
    'Books, Music & Media',
    'Sports & Outdoors',
    'Toys & Games',
    'Art & Crafts',
    'Real Estate',
    'Other'
  ];
  noItemsFound: boolean = false;
  isLoading: boolean = false;

  searchTerm: string = '';
  selectedCategory: string = '';
  startingPrice: number | null = null;
  endingPrice: number | null = null;
  endDateBefore: string = '';
  selectedStatus: string = '';
  status: string[] = ['Active', 'Sold', 'Unsold'];

  currentPage: number = 1;
  pageSize: number = 10;
  totalPages: number = 1;

  private searchSubject = new Subject<string>();
  private searchSub!: Subscription;

  constructor(private itemService: ItemService,
              private router: Router,
  ) {}

  ngOnInit(): void {
    this.isLoading = true;
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
    this.isLoading = true;

    this.itemService.getFilteredItems({
      search: this.searchTerm,
      category: this.selectedCategory,
      startingPrice: this.startingPrice!,
      endingPrice: this.endingPrice!,
      endDateBefore: this.endDateBefore,
      page: this.currentPage,
      pageSize: this.pageSize
    }).subscribe({
      next: (response) => {
        console.log(response.data);
        const statusOrder: Record<string, number> = {
          active: 1,
          sold: 2,
          unsold: 3
        };

        const filtered = (response.data ?? []).filter(item =>
          this.applyStatusFilter([item]).length > 0
        );

        this.items = filtered.sort((a, b) => {
          const statusA = statusOrder[a.status.toLowerCase()] ?? 999;
          const statusB = statusOrder[b.status.toLowerCase()] ?? 999;
          return statusA - statusB;
        });

        this.noItemsFound = filtered.length === 0;

        this.totalPages = response.pagination?.totalPages ?? 1;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Fetch failed:', err);
        this.noItemsFound = true;
        this.isLoading = false;
      }
    });
  }

  ngOnDestroy(): void {
    if (this.searchSub) {
      this.searchSub.unsubscribe();
    }
  }

  onPostItem(): void {
    this.router.navigate(['/post-item']);
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.fetchFilteredItems();
    }
  }
  get pages(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }

  goPrevious(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.fetchFilteredItems();
    }
  }

  goNext(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.fetchFilteredItems();
    }
  }

  clearFilters(): void {
    this.selectedCategory = '';
    this.endDateBefore = '';
    this.searchTerm = '';
    this.selectedStatus = '';
    this.currentPage = 1; 
    this.endingPrice = null;
    this.startingPrice = null;

    this.onFilterChange(); 
  }

  applyStatusFilter(items: Item[]): Item[] {
    if (!this.selectedStatus) 
      return items;
    return items.filter(item => item.status.toLowerCase() === this.selectedStatus.toLowerCase());
  }

}
