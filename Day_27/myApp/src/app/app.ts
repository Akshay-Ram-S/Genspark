import { Component } from '@angular/core';
import { CustomerDetails } from './customer-details/customer-details';
import { Products } from './products/products';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  styleUrl: './app.css',
  imports: [CustomerDetails,Products]
})
export class App {
  protected title = 'myApp';
}