import { Component } from '@angular/core';
import { NavigationEnd, Router, RouterModule, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Navbar } from "./components/navbar-component/navbar-component";
import { filter } from 'rxjs';
import { Footer } from "./components/footer/footer";

@Component({
  selector: 'app-root',
  imports: [CommonModule, Navbar, RouterOutlet, RouterModule, Footer],
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
