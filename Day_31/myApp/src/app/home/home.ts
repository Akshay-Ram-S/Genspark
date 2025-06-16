import { Component, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { debounceTime, Subject, switchMap, tap, catchError, of } from 'rxjs';
import { ProductService } from '../services/product';
import { ProductsComponent } from '../products/products';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, ProductsComponent],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class HomeComponent {
  products: any[] = [];
  isLoading = false;
  hasError = false;
  private limit = 10;
  private skip = 0;
  private query = '';
  private searchSubject = new Subject<string>();

  constructor(private productService: ProductService) {
    this.searchSubject.pipe(
      debounceTime(400),
      tap(q => {
        this.query = q;
        this.skip = 0;
        this.products = [];
        this.hasError = false;
        this.isLoading = true;
      }),
      switchMap(q =>
        this.productService.searchProducts(q, this.limit, this.skip).pipe(
          catchError(() => {
            this.hasError = true;
            this.isLoading = false;
            return of({ products: [] });
          })
        )
      )
    ).subscribe(res => {
      this.products = res.products;
      this.skip += this.limit;
      this.isLoading = false;
    });
  }

  onSearch(event: Event): void {
    const input = event.target as HTMLInputElement;
    const value = input?.value ?? '';
    this.searchSubject.next(value);
  }

  @HostListener('window:scroll', [])
  onScroll(): void {
    if ((window.innerHeight + window.scrollY) >= document.body.offsetHeight - 100 && !this.isLoading) {
      this.loadMore();
    }
  }

  scrollToTop(): void {
  window.scrollTo({ top: 0, behavior: 'smooth' });
  }
  loadMore(): void {
    this.isLoading = true;
    this.productService.searchProducts(this.query, this.limit, this.skip).subscribe(res => {
      this.products = [...this.products, ...res.products];
      this.skip += this.limit;
      this.isLoading = false;
    });
  }
}
