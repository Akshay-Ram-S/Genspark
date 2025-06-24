import { Component } from '@angular/core';
import { Router, RouterModule, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from "./navbar-component/navbar-component";
import { LiveBidComponent } from "./live-bid/live-bid";

@Component({
  selector: 'app-root',
  imports: [CommonModule, NavbarComponent, RouterOutlet, RouterModule, LiveBidComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'AuctionAPI';
}
