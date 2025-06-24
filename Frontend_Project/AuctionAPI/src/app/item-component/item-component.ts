import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { TokenService } from '../services/token.service';
import { AuthService } from '../services/auth.service';
import { Route, Router } from '@angular/router';
import { ItemService } from '../services/item.service';

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
  @Input() currentUserRole: string | null = '';
  public isAuthenticated: boolean = false;
  @Input() bidButton: boolean = true;
  @Input() viewBidButton: boolean = true;

  constructor(private tokenService: TokenService, 
              private authService: AuthService, 
              private itemService: ItemService,
              private router: Router) {
    this.currentUserRole = this.tokenService.getRole();
    this.isAuthenticated = this.authService.isAuthenticated();
    
  }

  onPlaceBid(): void {
    if(!this.isAuthenticated){
       this.router.navigate(['/login']);
       return;
    }
    this.router.navigate(['/live-bid', this.item.itemID]);
  }

  onViewAllBids(): void {
    this.router.navigate(['/view-item', this.item.itemID]);
  }

}
