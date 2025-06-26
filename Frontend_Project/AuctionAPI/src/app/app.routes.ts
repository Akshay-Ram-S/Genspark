import { Routes } from '@angular/router';
import { LoginComponent } from './login-component/login-component';
import { RegisterComponent } from './register-component/register-component';
import { ProfileComponent } from './profile-component/profile-component';
import { ItemsComponent } from './items-component/items-component';
import { ViewItemComponent } from './view-item/view-item';
import { PostItemComponent } from './post-item/post-item';
import { EditItemComponent } from './edit-item/edit-item';
import { UsersComponent } from './users-component/users-component';
import { SellerGuard } from './guards/seller-guard';
import { UnauthorizedComponent } from './unauthorized/unauthorized';
import { BidderGuard } from './guards/bidder-guard';
import { LiveBidComponent } from './live-bid/live-bid';
import { ChangePasswordComponent } from './change-password/change-password';
import { AuthGuard } from './guards/auth-guard';

export const routes: Routes = [
    //{ path: '', component: HomeComponent },
    { path: 'login', component: LoginComponent},
    { path: 'register', component: RegisterComponent},
    { path: 'profile', component: ProfileComponent, canActivate: [AuthGuard] },
    { path: 'items', component: ItemsComponent},
    { path: 'post-item', component: PostItemComponent, canActivate: [SellerGuard] },
    { path: 'items/edit-item/:itemId', component: EditItemComponent, canActivate: [SellerGuard]  },
    { path: 'live-bid/:itemId', component: LiveBidComponent, canActivate: [BidderGuard] },
    { path: 'view-item/:id', component: ViewItemComponent },
    { path: 'users/:role', component: UsersComponent },
    { path: 'users/seller/:id', component: ProfileComponent },
    { path: 'users/bidder/:id', component: ProfileComponent },
    { path: 'unauthorized', component: UnauthorizedComponent },
    { path:'profile/change-password', component: ChangePasswordComponent},


];
