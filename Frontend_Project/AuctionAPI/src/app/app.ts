import { Component } from '@angular/core';
import { RouterModule, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from "./navbar-component/navbar-component";

@Component({
  selector: 'app-root',
  imports: [CommonModule, NavbarComponent, RouterOutlet, RouterModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'AuctionAPI';
}
