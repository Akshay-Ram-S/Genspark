import { Component } from '@angular/core';
import { NavigationEnd, Router, RouterModule, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Navbar } from "./navbar-component/navbar-component";
import { filter } from 'rxjs';

@Component({
  selector: 'app-root',
  imports: [CommonModule, Navbar, RouterOutlet, RouterModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'AuctionAPI';
  isHomePage = false;

  constructor(private router: Router) {
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: any) => {
        this.isHomePage = event.url === '/' || event.urlAfterRedirects === '/';
      });
  }
}
