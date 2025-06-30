import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { TokenService } from '../services/token.service';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';
import { ImageService } from '../services/image.service';

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
  @Input() isOwnProfile: boolean = false; 
  imageLoaded = false;
  imageUrl: string  = '';
  isAdmin = false;
  showDescriptionModal = false;
  modalDescription: string = '';
  modalTitle: string = '';


  ngOnInit() {
    this.imageUrl = this.imageService.getItemImage(this.item.itemID);
    const token = localStorage.getItem('token');
    if (token) {
      const payload = JSON.parse(atob(token.split('.')[1]));
      this.isAdmin = payload?.role?.toLowerCase() === 'admin';
    }
  }

  onImageLoad() {
    this.imageLoaded = true;
  }

  constructor(private tokenService: TokenService, 
              private authService: AuthService, 
              private router: Router,
              private imageService: ImageService,
            ) {
    this.currentUserRole = this.tokenService.getRole();
    this.isAuthenticated = this.authService.isAuthenticated();
    
  }



  onPlaceBid(): void {
    if(!this.isAuthenticated){
       this.router.navigate(['/login']);
       return;
    }
    this.router.navigate(['/items/live-bid', this.item.itemID]);
  }

  onViewAllBids(): void {
    this.router.navigate(['/view-item', this.item.itemID]);
  }

  editItem(): void{
    this.router.navigate(['/items/edit-item', this.item.itemID]);
  }

  openDescriptionModal(description: string, title: string): void {
    this.modalDescription = description;
    this.modalTitle = title;
    this.showDescriptionModal = true;
  }

  closeDescriptionModal(): void {
    this.showDescriptionModal = false;
  }


}
