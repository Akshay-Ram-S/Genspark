import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ItemService } from '../services/item.service';
import { TokenService } from '../services/token.service';

@Component({
  selector: 'app-post-item',
  templateUrl: './post-item.html',
  styleUrls: ['./post-item.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule]
})
export class PostItemComponent implements OnInit {
  postForm!: FormGroup;
  imageFile!: File | null;

  constructor(
    private fb: FormBuilder,
    private itemService: ItemService,
    private router: Router,
    private tokenService: TokenService 
  ) {}

  ngOnInit(): void {
    this.postForm = this.fb.group({
      title: ['', Validators.required],
      startingPrice: [null, [Validators.required, Validators.min(0)]],
      endDate: ['', Validators.required],
      category: [''],
      description: ['', Validators.required],
      image: [null]
    });
  }

  onImageSelected(event: any): void {
    this.imageFile = event.target.files[0] ?? null;
  }

  onSubmit(): void {
    if (this.postForm.invalid) return;
    const sellerID = this.tokenService.getUserId();
    console.log('Seller ID:', sellerID);

    const formData = new FormData();
    formData.append('title', this.postForm.value.title);
    formData.append('startingPrice', this.postForm.value.startingPrice);
    formData.append('endDate', this.postForm.value.endDate);
    formData.append('category', this.postForm.value.category || '');
    formData.append('description', this.postForm.value.description || '');
    formData.append('sellerID', String(sellerID));

    if (this.imageFile) {
      formData.append('image', this.imageFile);
    }

    this.itemService.postItem(formData).subscribe({
      next: () => this.router.navigate(['/items']),
      error: (err) => console.error('Error posting item', err)
    });

  }
}
