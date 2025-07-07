import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { TokenService } from '../services/token.service';
import { AuthService } from '../services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-home',
  imports: [CommonModule, RouterModule],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home implements OnInit{
  hovering = false;
  userRole: string | null = '';
  isAuthenticated: boolean = false;
  ngOnInit(){
    this.isAuthenticated = this.authService.isAuthenticated();
    const token = localStorage.getItem('token');
    if (token) {
      const payload = JSON.parse(atob(token.split('.')[1]));
      this.userRole = payload?.role?.toLowerCase();
    }
    
  }

  constructor(private authService: AuthService) {}

}
