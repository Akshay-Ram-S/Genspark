import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule, Location } from '@angular/common';
import { ItemService } from '../../services/item.service';
import { futureDateValidator } from '../../validators/customValidator';

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
    'Fashion',
    'Home',
    'Antiques',
    'Automotive',
    'Books',
    'Sports',
    'Toys',
    'Art',
    'Other'
  ];
  imagePreviewUrl: string | null = null;

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
          endDate: this.formatToDateTimeLocal(item.endDate),
          category: item.category,
          description: item.description
        });
      },
      error: (err) => console.error('Error loading item data:', err)
    });
  }

  formatToDateTimeLocal(date: string | Date): string {
    const d = new Date(date);
    const offset = d.getTimezoneOffset();
    const local = new Date(d.getTime() - offset * 60 * 1000);
    return local.toISOString().slice(0, 16); 
  }

  onImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;

    if (!input.files || input.files.length === 0) {
      return;
    }

    const file = input.files[0];
    const allowedTypes = ['image/jpeg', 'image/png', 'image/jpg', 'image/webp'];
    const fileExtension = file.name.substring(file.name.lastIndexOf('.')).toLowerCase();
    const extensionValid = ['.jpg', '.jpeg', '.png', '.webp'].includes(fileExtension);

    if (!allowedTypes.includes(file.type) || !extensionValid) {
      this.errorMessage = 'Only image files (.jpg, .jpeg, .png, .webp) are allowed.';
      this.imageFile = null;
      this.imagePreviewUrl = null;
      return;
    }

    const img = new Image();
    img.src = URL.createObjectURL(file);

    img.onload = () => {
      const width = img.naturalWidth;
      const height = img.naturalHeight;

      const minWidth = 600;
      const minHeight = 400;

      if (width < minWidth || height < minHeight) {
        this.errorMessage = `Image too small. Minimum is ${minWidth}px ×${minHeight}px.`;
        this.imageFile = null;
        this.imagePreviewUrl = null;
        return;
      }

      this.convertToWebp(file)
        .then(webpFile => {
          this.errorMessage = '';
          this.imageFile = webpFile;
          this.imagePreviewUrl = img.src;
        })
        .catch(err => {
          console.error('Image conversion failed:', err);
          this.errorMessage = 'Failed to convert image to WebP.';
          this.imagePreviewUrl = null;
        });
    };

    img.onerror = () => {
      this.errorMessage = 'Invalid image file.';
      this.imageFile = null;
      this.imagePreviewUrl = null;
    };
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

  private convertToWebp(file: File): Promise<File> {
    return new Promise((resolve, reject) => {
      const img = new Image();
      const reader = new FileReader();

      reader.onload = (e) => {
        img.onload = () => {
          const canvas = document.createElement('canvas');
          canvas.width = img.width;
          canvas.height = img.height;
          const ctx = canvas.getContext('2d');
          if (!ctx) 
            return reject('Canvas context not available');

          ctx.drawImage(img, 0, 0);

          canvas.toBlob((blob) => {
            if (!blob) return reject('WebP conversion failed');
            const webpFile = new File([blob], file.name.replace(/\.\w+$/, '.webp'), { type: 'image/webp' });
            resolve(webpFile);
          }, 'image/webp', 0.8); 
        };
        img.onerror = reject;
        img.src = e.target?.result as string;
      };

      reader.onerror = reject;
      reader.readAsDataURL(file);
    });
  }

  
}
