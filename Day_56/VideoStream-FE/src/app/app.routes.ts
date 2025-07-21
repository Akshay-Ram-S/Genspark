import { Routes } from '@angular/router';
import { Videos } from './components/videos/videos';
import { UploadVideo } from './components/upload-video/upload-video';

export const routes: Routes = [
    { path: 'videos', component: Videos },
    { path: 'upload', component: UploadVideo}
];
