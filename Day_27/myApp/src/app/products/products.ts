import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-products',
  imports: [CommonModule],
  templateUrl: './products.html',
  styleUrl: './products.css'
})
export class Products {
  cartCount = 0;

  products = [
    { name: 'Headphones', price: 1000, description: 'Descripton 1', image: 'headphones.jpeg', quantity: 0 },
    { name: 'Lens', price: 5000, description: 'Descripton 2', image: 'lens.jpeg', quantity: 0 },
    { name: 'Skin-Care', price: 500, description: 'Descripton 3', image: 'skin-care.jpeg', quantity: 0 }
  ];

  addToCart(product: { name: string, price: number, description: string, image: string, quantity: number }) {
      this.cartCount++;
      product.quantity++;
  }

  removeFromCart(product: { name: string, price: number, description: string, image: string, quantity: number }) {
    this.cartCount--;
    product.quantity--;
    
  }
}
