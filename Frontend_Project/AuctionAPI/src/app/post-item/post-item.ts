import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ItemService } from '../services/item.service';
import { TokenService } from '../services/token.service';
import { futureDateValidator } from '../validators/customValidator';

@Component({
  selector: 'app-post-item',
  templateUrl: './post-item.html',
  styleUrls: ['./post-item.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule]
})
export class PostItem implements OnInit {
  postForm!: FormGroup;
  imageFile!: File | null;
  errorMessage: string = '';
  categories: string[] = [
    'Electronics',
    'Fashion & Apparel',
    'Home & Furniture',
    'Collectibles & Antiques',
    'Automotive',
    'Books, Music & Media',
    'Sports & Outdoors',
    'Toys & Games',
    'Art & Crafts',
    'Real Estate',
    'Other'
  ];

  constructor(
    private fb: FormBuilder,
    private itemService: ItemService,
    private router: Router,
    private tokenService: TokenService 
  ) {}

  ngOnInit(): void {
    this.postForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(3)]],
      startingPrice: [null, [Validators.required, Validators.min(0)]],
      endDate: ['', [Validators.required, futureDateValidator]],
      category: ['', Validators.required],
      description: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(1000)]],
      image: [null]
    });
  }

  onImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;

    if (!input.files || input.files.length === 0) {
      return;
    }

    const file = input.files[0];
    const allowedTypes = ['image/jpeg', 'image/png', 'image/jpg', 'image/webp'];
    const allowedExtensions = ['.jpg', '.jpeg', '.png', '.webp'];

    const fileTypeValid = allowedTypes.includes(file.type);
    const fileExtension = file.name.substring(file.name.lastIndexOf('.')).toLowerCase();
    const extensionValid = allowedExtensions.includes(fileExtension);

    if (!fileTypeValid || !extensionValid) {
      this.errorMessage = 'Only image files (.jpg, .jpeg, .png, .webp) are allowed.';
      this.imageFile = null;
      return;
    }

    this.errorMessage = '';
    this.imageFile = file;
  }


  onSubmit(): void {
    if (this.postForm.invalid) return;

    const sellerID = this.tokenService.getUserId();
    console.log('Seller ID:', sellerID);

    const formData = new FormData();

    const istDateStr = this.postForm.value.endDate;
    const istDate = new Date(istDateStr); 
    const utcDateStr = istDate.toISOString(); 

    formData.append('title', this.postForm.value.title);
    formData.append('startingPrice', this.postForm.value.startingPrice);
    formData.append('endDate', utcDateStr); 
    formData.append('category', this.postForm.value.category || '');
    formData.append('description', this.postForm.value.description || '');
    formData.append('sellerID', String(sellerID));

    if (this.imageFile) {
      formData.append('image', this.imageFile);
    }

    this.itemService.postItem(formData).subscribe({
      next: () => this.router.navigate(['/items']),
      error: (err) => {
        console.error('Error posting item', err)
        this.errorMessage = err.error?.errors?.Exception[0];
      }
    });
  }

}
