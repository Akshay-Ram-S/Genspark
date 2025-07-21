import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpEventType } from '@angular/common/http';
import { VideoService } from '../../services/VideoService';

@Component({
  selector: 'app-upload-video',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './upload-video.html',
  styleUrl: './upload-video.css'
})
export class UploadVideo {
  title: string = '';
  description: string = '';
  file: File | null = null;
  previewUrl: string | null = null;
  isImage: boolean = false;
  isVideo: boolean = false;
  isUploading: boolean = false;

  constructor(private videoService: VideoService) {}

  onFileSelected(event: any) {
    const file = event.target.files[0];
    this.file = file;

    if (!file) return;

    const fileType = file.type;

    this.isImage = fileType.startsWith('image/');
    this.isVideo = fileType.startsWith('video/');

    const reader = new FileReader();
    reader.onload = () => {
      this.previewUrl = reader.result as string;
    };
    reader.readAsDataURL(file);
  }

  onSubmit() {
    if (!this.file || !this.title || !this.description) return;

    this.isUploading = true;

    const formData = new FormData();
    formData.append('file', this.file);
    formData.append('title', this.title);
    formData.append('description', this.description);

    this.videoService.uploadVideo(formData).subscribe({
      next: () => {
        this.isUploading = false;
        alert('Upload successful!');
        this.resetForm();
      },
      error: (err) => {
        this.isUploading = false;
        console.error(err);
        alert('Upload failed!');
      }
    });

  }

  private resetForm() {
    this.title = '';
    this.description = '';
    this.file = null;
    this.previewUrl = null;
    this.isImage = false;
    this.isVideo = false;
    this.isUploading = false;
  }
}
