import { Component } from '@angular/core';
import { Router, RouterModule, RouterOutlet } from '@angular/router';
import { LoginComponent } from './login-component/login-component';
import { RegisterComponent } from './register-component/register-component';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from "./navbar-component/navbar-component";
import { ItemsComponent } from './items-component/items-component';
import { ProfileComponent } from "./profile-component/profile-component";

@Component({
  selector: 'app-root',
  imports: [CommonModule, NavbarComponent, RouterOutlet, RouterModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'AuctionAPI';
}
