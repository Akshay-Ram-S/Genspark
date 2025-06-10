import { Component } from '@angular/core';

@Component({
  selector: 'app-customer-details',
  imports: [],
  templateUrl: './customer-details.html',
  styleUrl: './customer-details.css'
})
export class CustomerDetails {
  custId:number;
  name:string;
  email:string;
  age:number;
  likeCount:number;
  dislikeCount:number;

  constructor(){
    this.custId = 101
    this.name = "User"
    this.email = "user@gmail.com"
    this.age = 25
    this.likeCount = 0;
    this.dislikeCount = 0;
  }

  like() {
    this.likeCount++;
  }

  dislike() {
    this.dislikeCount++;
  }

}
