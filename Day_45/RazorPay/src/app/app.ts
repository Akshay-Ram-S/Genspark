import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Payment } from "./payment/payment";

@Component({
  selector: 'app-root',
  imports: [Payment],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'RazorPay';
}
