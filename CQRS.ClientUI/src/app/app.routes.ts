import { Routes } from '@angular/router';
import { Login } from './login/login';
import { Batchserial } from './batchserial/batchserial';
import { User } from './user/user';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: Login },
  { path: 'batchserial', component: Batchserial },
  { path: 'user', component: User },
];
