import { Component } from '@angular/core';
import { VideoService } from '../../services/VideoService';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-videos',
  imports: [CommonModule],
  templateUrl: './videos.html',
  styleUrl: './videos.css'
})
export class Videos {
  videos: any[] = [];

  constructor(private videoService: VideoService, private router: Router) {}

  ngOnInit(): void {
    this.videoService.getAllVideos().subscribe({
      next: (res) => this.videos = res,
      error: (err) => console.error('Error fetching videos', err)
    });
  }

  openFullscreen(video: any) {
    this.router.navigate(['/video', video.id, 'fullscreen']);
  }
}
