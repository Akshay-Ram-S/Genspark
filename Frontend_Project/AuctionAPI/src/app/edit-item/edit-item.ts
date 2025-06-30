import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule, Location } from '@angular/common';
import { ItemService } from '../services/item.service';
import { futureDateValidator } from '../validators/customValidator';

@Component({
  selector: 'app-edit-item',
  templateUrl: './edit-item.html',
  styleUrls: ['./edit-item.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule]
})
export class EditItem implements OnInit {
  itemId: string = '';
  editForm!: FormGroup;
  imageFile!: File | null;
  showDeleteModal = false;
  errorMessage = '';
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
    private route: ActivatedRoute,
    private router: Router,
    private location: Location
  ) {}

  ngOnInit(): void {
    this.itemId = this.route.snapshot.paramMap.get('itemId') ?? '';
    this.editForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(3)]],
      startingPrice: [null, [Validators.required, Validators.min(0)]],
      endDate: ['', [Validators.required, futureDateValidator]],
      category: ['',[Validators.required]],
      description: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(1000)]],
      image: [null]
    });
    this.itemService.getItemById(this.itemId).subscribe({
      next: (res) => {
        const item = res.data;
        this.editForm.patchValue({
          title: item.title,
          startingPrice: item.startingPrice,
          endDate: item.endDate ? item.endDate.split('T')[0] : '',
          category: item.category,
          description: item.description
        });
      },
      error: (err) => console.error('Error loading item data:', err)
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
    console.log(this.errorMessage);

    this.errorMessage = '';
    this.imageFile = file;
  }

  onSubmit(): void {
    if (this.editForm.invalid){
      this.editForm.markAllAsTouched();
      return;
    }

    const istDateStr = this.editForm.value.endDate;
    const istDate = new Date(istDateStr); 
    const utcDateStr = istDate.toISOString(); 

    const formData = new FormData();
    formData.append('title', this.editForm.value.title);
    formData.append('startingPrice', this.editForm.value.startingPrice);
    formData.append('endDate', utcDateStr);
    formData.append('category', this.editForm.value.category || '');
    formData.append('description', this.editForm.value.description || '');

    if (this.imageFile) {
      formData.append('image', this.imageFile);
    }

    this.itemService.updateItem(this.itemId, formData).subscribe({
      next: () => this.location.back(),
      error: (err) => console.error('Error updating item:', err)
    });
  }

  openModal(): void {
    this.showDeleteModal = true;
  }

  closeModal(): void {
    this.showDeleteModal = false;
  }

  confirmDelete() {
    this.deleteItem(); 
  }

  deleteItem(): void {
    this.itemService.deleteItem(this.itemId).subscribe({
    next: () => {
      this.closeModal();
      this.router.navigate(['/items']);
    },
    error: (err) => console.error('Error deleting item:', err)
    });
  }

  
}
