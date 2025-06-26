import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { TokenService } from '../services/token.service';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';
import { ImageService } from '../services/image.service';
import {Modal} from 'bootstrap';

@Component({
  selector: 'app-item',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './item-component.html',
  styleUrl: './item-component.css'
})
export class ItemComponent implements OnInit {
  @Input() item!: any;
  public fallbackImage: string = 'https://images.pexels.com/photos/414612/pexels-photo-414612.jpeg';
  @Input() currentUserRole: string | null = '';
  public isAuthenticated: boolean = false;
  @Input() bidButton: boolean = true;
  @Input() viewBidButton: boolean = true;
  @Input() sellerName: string = '';
  imageLoaded = false;
  imageUrl: string  = '';

  ngOnInit() {
    this.imageUrl = 'http://localhost:5205/api/v1/Image/view/' + this.item.itemID;
    
    console.log('Image URL:', this.imageUrl);
  }

  onImageLoad() {
    this.imageLoaded = true;
  }

  constructor(private tokenService: TokenService, 
              private authService: AuthService, 
              private router: Router,
              private imageService: ImageService
            ) {
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
