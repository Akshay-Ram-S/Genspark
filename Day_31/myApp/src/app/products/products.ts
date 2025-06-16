import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductComponent } from '../product/product';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule, ProductComponent],
  templateUrl: './products.html',
  styleUrl: './products.css'
})
export class ProductsComponent {
  @Input() products: any[] = [];
  
}
