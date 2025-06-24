import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { Notification } from "../notification/notification";

@Component({
  selector: 'app-navbar-component',
  imports: [CommonModule, RouterModule, Notification],
  templateUrl: './navbar-component.html',
  styleUrl: './navbar-component.css'
})

export class NavbarComponent {

  get isLoggedIn(): boolean {
    return this.authService.isAuthenticated();
  }

  constructor(private authService: AuthService, private router: Router) {}

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']); 
  }
}
