import { Routes } from '@angular/router';
import { Login } from './login-component/login-component';
import { Register } from './register-component/register-component';
import { ItemsComponent } from './items-component/items-component';
import { ViewItem } from './view-item/view-item';
import { PostItem } from './post-item/post-item';
import { UsersComponent } from './users-component/users-component';
import { SellerGuard } from './guards/seller-guard';
import { Unauthorized} from './unauthorized/unauthorized';
import { LiveBid } from './live-bid/live-bid';
import { AuthGuard } from './guards/auth-guard';
import { Dashboard } from './dashboard/dashboard';
import { AdminGuard } from './guards/admin-guard';
import { Home } from './home/home';
import { ChangePassword } from './change-password/change-password';
import { EditItem } from './edit-item/edit-item';
import { Profile } from './profile-component/profile-component';

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
