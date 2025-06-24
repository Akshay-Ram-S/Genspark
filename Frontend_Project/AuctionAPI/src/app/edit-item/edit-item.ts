import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ItemService } from '../services/item.service';

@Component({
  selector: 'app-edit-item',
  templateUrl: './edit-item.html',
  styleUrls: ['./edit-item.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule]
})
export class EditItemComponent implements OnInit {
  itemId: string = '';
  editForm!: FormGroup;
  imageFile!: File | null;

  constructor(
    private fb: FormBuilder,
    private itemService: ItemService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.itemId = this.route.snapshot.paramMap.get('itemId') ?? '';
    this.editForm = this.fb.group({
      title: ['', Validators.required],
      startingPrice: [null, [Validators.required, Validators.min(0)]],
      endDate: ['', Validators.required],
      category: [''],
      description: ['', Validators.required],
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

  onImageSelected(event: any): void {
    this.imageFile = event.target.files[0] ?? null;
  }

  onSubmit(): void {
    if (this.editForm.invalid) return;

    const formData = new FormData();
    formData.append('title', this.editForm.value.title);
    formData.append('startingPrice', this.editForm.value.startingPrice);
    formData.append('endDate', this.editForm.value.endDate);
    formData.append('category', this.editForm.value.category || '');
    formData.append('description', this.editForm.value.description || '');

    if (this.imageFile) {
      formData.append('image', this.imageFile);
    }

    this.itemService.updateItem(this.itemId, formData).subscribe({
      next: () => this.router.navigate(['/profile']),
      error: (err) => console.error('Error updating item:', err)
    });
  }
}
