import { Routes } from '@angular/router';
import { Login } from './components/login-component/login-component';
import { Register } from './components/register-component/register-component';
import { ItemsComponent } from './components/items-component/items-component';
import { ViewItem } from './components/view-item/view-item';
import { PostItem } from './components/post-item/post-item';
import { UsersComponent } from './components/users-component/users-component';
import { SellerGuard } from './guards/seller-guard';
import { Unauthorized} from './components/unauthorized/unauthorized';
import { LiveBid } from './components/live-bid/live-bid';
import { AuthGuard } from './guards/auth-guard';
import { Dashboard } from './components/dashboard/dashboard';
import { AdminGuard } from './guards/admin-guard';
import { Home } from './components/home/home';
import { ChangePassword } from './components/change-password/change-password';
import { EditItem } from './components/edit-item/edit-item';
import { Profile } from './components/profile-component/profile-component';

export const routes: Routes = [
    { path: '', component: Home },
    { path: 'login', component: Login},
    { path: 'register', component: Register},
    { path: 'profile', component: Profile, canActivate: [AuthGuard] },
    { path: 'items', component: ItemsComponent},
    { path: 'post-item', component: PostItem, canActivate: [SellerGuard] },
    { path: 'items/edit-item/:itemId', component: EditItem, canActivate: [SellerGuard]  },
    { path: 'items/live-bid/:itemId', component: LiveBid},
    { path: 'view-item/:id', component: ViewItem },
    { path: 'users/:role', component: UsersComponent },
    { path: 'users/seller/:id', component: Profile },
    { path: 'users/bidder/:id', component: Profile },
    { path: 'unauthorized', component: Unauthorized },
    { path: 'profile/change-password', component: ChangePassword},
    { path: 'dashboard', component: Dashboard, canActivate: [AdminGuard]}

];
