import { Routes } from '@angular/router';
import { Login } from './login/login';
import { Batchserial } from './batchserial/batchserial';
import { User } from './user/user';
import { Product } from './product/product';
import { authGuard } from './_guards/auth-guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: Login, canActivate: [authGuard] },
  { path: 'batchserial', component: Batchserial },
  { path: 'user', component: User },
  { path: 'product', component: Product },
];
