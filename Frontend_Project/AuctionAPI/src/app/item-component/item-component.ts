import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { TokenService } from '../services/misc.service';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-item',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './item-component.html',
  styleUrl: './item-component.css'
})
export class ItemComponent {
  @Input() item!: any;
  public fallbackImage: string = 'https://images.pexels.com/photos/414612/pexels-photo-414612.jpeg';
  public currentUserRole: string | null = '';
  public isAuthenticated: boolean = false;

  constructor(private tokenService: TokenService, private authService: AuthService) {
    this.currentUserRole = this.tokenService.getRole();
    this.isAuthenticated = this.authService.isAuthenticated();
  }
}
