import { Component, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../services/product.service';
import { Product } from '../models/product.model';
import { Router } from '@angular/router';
import { debounceTime, distinctUntilChanged, Subject, switchMap } from 'rxjs';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './products.html',
  styleUrls: ['./products.css']
})
export class Products implements OnInit {
  products: Product[] = [];
  query = '';
  limit = 10;
  skip = 0;
  loading = false;
  searchSubject = new Subject<string>();

  constructor(private productService: ProductService, private router: Router) {}

  ngOnInit() {
    this.searchSubject.pipe(
      debounceTime(500),
      distinctUntilChanged(),
      switchMap((query) => {
        this.skip = 0;
        this.loading = true;
        return this.productService.searchProducts(query, this.limit, this.skip);
      })
    ).subscribe(res => {
      this.products = res.products;
      this.loading = false;
    });

    this.loadProducts();
  }

  @HostListener('window:scroll', [])
    onScroll(): void {
    if ((window.innerHeight + window.scrollY) >= document.body.scrollHeight - 50) {
      this.loadProducts(); 
    }
  }

  loadProducts() {
    this.loading = true;
    this.productService.searchProducts(this.query, this.limit, this.skip).subscribe(res => {
      this.products.push(...res.products);
      this.loading = false;
    });
  }

  onSearch() {
    this.searchSubject.next(this.query);
  }

  viewDetail(id: number) {
    this.router.navigate(['/products', id]);
  }
}
