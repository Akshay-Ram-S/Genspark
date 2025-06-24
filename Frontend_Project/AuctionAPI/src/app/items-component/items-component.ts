import { Component, OnInit, OnDestroy } from '@angular/core';
import { ItemService } from '../services/item.service';
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
  categories: string[] = ['Electronics', 'Sports', 'Collectibles'];
  isSeller = false;
  noItemsFound: boolean = false;
  isLoading: boolean = false;

  searchTerm: string = '';
  selectedCategory: string = '';
  startingPrice: number | null = null;
  endingPrice: number | null = null;
  endDateBefore: string = '';

  currentPage: number = 1;
  pageSize: number = 5;
  totalPages: number = 1;

  private searchSubject = new Subject<string>();
  private searchSub!: Subscription;

  constructor(private itemService: ItemService,
              private router: Router
  ) {}

  ngOnInit(): void {
    this.isLoading = true;
    const token = localStorage.getItem('token');
    if (token) {
      const payload = JSON.parse(atob(token.split('.')[1]));
      this.isSeller = payload?.role?.toLowerCase() === 'seller';
    }
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
        this.noItemsFound = false;
        this.items = response.data ?? [];
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
    this.searchSub.unsubscribe();
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

}
