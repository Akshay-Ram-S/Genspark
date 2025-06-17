import { Routes } from '@angular/router';
import { Login } from './login/login';
import { Products } from './products/products';
import { ProductDetail } from './product/product';
import { AuthGuard } from './auth-guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: Login },
  { path: 'products', component: Products, canActivate: [AuthGuard] },
  { path: 'products/:id', component: ProductDetail, canActivate: [AuthGuard] }
];
