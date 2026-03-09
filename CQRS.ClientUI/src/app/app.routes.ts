import { Routes } from '@angular/router';
import { authGuard } from './_guards/auth-guard';
import { Batchserial } from './batchserial/batchserial';
import { Joborder } from './joborder/joborder';
import { Accessories } from './line1/accessories/accessories';
import { ScanmainserialComponent } from './line1/scanmainserial/scanmainserial';
import { Login } from './login/login';
import { ProductComponent } from './product/product';
import { User } from './user/user';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: Login, canActivate: [authGuard] },
  { path: 'batchserial', component: Batchserial },
  { path: 'joborder', component: Joborder },
  { path: 'user', component: User },
  { path: 'product', component: ProductComponent },
  { path: 'line1/accessories', component: Accessories },
  { path: 'line1/scanmainserial', component: ScanmainserialComponent },
];
