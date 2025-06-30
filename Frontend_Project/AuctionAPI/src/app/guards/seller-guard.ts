import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { TokenService } from '../services/token.service';

@Injectable({
  providedIn: 'root'
})
export class SellerGuard implements CanActivate {
  constructor(private tokenService: TokenService, 
              private authService: AuthService, 
              private router: Router) {}

  canActivate(): boolean {

    if(!this.authService.isAuthenticated()) {
      this.router.navigate(['/login']);
      return false;
    }

    const role = this.tokenService.getRole();

    if (role === 'Seller' || role ==='Admin') {
      return true;
    }

    this.router.navigate(['/unauthorized']);
    return false;
  }
}
