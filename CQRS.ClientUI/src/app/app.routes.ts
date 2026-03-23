import { Routes } from '@angular/router';
import { authGuard } from './_guards/auth-guard';
import { loginGuard } from './_guards/login.guard';
import { Batchserial } from './batchserial/batchserial';
import { DashboardComponent } from './dashboard/dashboard';
import { Joborder } from './joborder/joborder';
import { LayoutComponent } from './layout/layout/layout';
import { Accessories } from './line1/accessories/accessories';
import { ScanmainserialComponent } from './line1/scanmainserial/scanmainserial';
import { Login } from './login/login';
import { ProductComponent } from './product/product';
import { User } from './user/user';
export const routes: Routes = [
  // LOGIN PAGE
  {
    path: 'login',
    component: Login,
    canActivate: [loginGuard],
    data: { breadcrumb: null }, // no breadcrumb for login
  },
  // MAIN APPLICATION LAYOUT
  {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    canActivateChild: [authGuard],
    children: [
      {
        path: 'dashboard',
        component: DashboardComponent,
        data: { breadcrumb: 'Dashboard', title: 'Dashboard' },
      },
      {
        path: 'batchserial',
        component: Batchserial,
        data: { breadcrumb: 'Batch Serial', title: 'Batch Serial' },
      },
      {
        path: 'joborder',
        component: Joborder,
        data: { breadcrumb: 'Job Order', title: 'Job Order' },
      },
      {
        path: 'product',
        component: ProductComponent,
        data: { breadcrumb: 'Product', title: 'Product' },
      },
      {
        path: 'user',
        component: User,
        data: { breadcrumb: 'Users', title: 'Users' },
      },
      // FIX: Proper nested child route for line1
      {
        path: 'line1',
        data: { breadcrumb: 'Line 1' },
        children: [
          {
            path: 'accessories',
            component: Accessories,
            data: { breadcrumb: 'Accessories', title: 'Accessories' },
          },
          {
            path: 'scanmainserial',
            component: ScanmainserialComponent,
            data: { breadcrumb: 'Scan Main Serial', title: 'Scan Main Serial' },
          },
        ],
      },
      // Default when logged in
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
    ],
  },
  // WILDCARD → redirect to dashboard
  { path: '**', redirectTo: 'dashboard' },
];
