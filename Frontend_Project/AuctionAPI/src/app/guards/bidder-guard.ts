import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { TokenService } from '../services/token.service';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class BidderGuard implements CanActivate {
  constructor(private tokenService: TokenService, 
              private authService: AuthService, 
              private router: Router) {}

  canActivate(): boolean {
    if(!this.authService.isAuthenticated()) {
      this.router.navigate(['/login']);
      return false;
    }
    const role = this.tokenService.getRole();

    if (role === 'Bidder' || role ==='Admin') {
      return true;
    }

    this.router.navigate(['/unauthorized']);
    return false;
  }
}
