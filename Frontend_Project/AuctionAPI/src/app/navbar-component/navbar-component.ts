import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { Notification } from "../notification/notification";
import { TokenService } from '../services/token.service';

@Component({
  selector: 'app-navbar-component',
  imports: [CommonModule, RouterModule, Notification],
  templateUrl: './navbar-component.html',
  styleUrl: './navbar-component.css'
})

export class Navbar implements OnInit{

  isSeller: boolean = false;
  isAdmin: boolean = false;
  get isLoggedIn(): boolean {
    return this.authService.isAuthenticated();
  }

  constructor(private authService: AuthService, 
              private router: Router) {
    }

  ngOnInit(){
    const token = localStorage.getItem('token');
    if (token) {
      const payload = JSON.parse(atob(token.split('.')[1]));
      this.isSeller = payload?.role?.toLowerCase() === 'seller';
      this.isAdmin = payload?.role?.toLowerCase() === 'admin';
    }
  
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']).then(() => window.location.reload());
    
  }

  
}
