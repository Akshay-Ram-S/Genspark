import { Component } from '@angular/core';
import { Home } from "./home/home";
import { Navbar } from './navbar/navbar';

@Component({
  selector: 'app-root',
  imports: [Home, Navbar],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'userManagement';
}
