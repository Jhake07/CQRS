import { Injectable, signal } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs/operators';
export interface BreadcrumbItem {
  label: string;
  url: string;
}
@Injectable({ providedIn: 'root' })
export class BreadcrumbService {
  breadcrumbs = signal<BreadcrumbItem[]>([]);
  pageTitle = signal<string>(''); // auto title
  constructor(
    private router: Router,
    private route: ActivatedRoute,
  ) {
    this.router.events
      .pipe(filter((event) => event instanceof NavigationEnd))
      .subscribe(() => {
        const root = this.route.root;
        const breadcrumbList: BreadcrumbItem[] = [];
        this.buildBreadcrumbs(root, '', breadcrumbList);
        this.breadcrumbs.set(breadcrumbList);
        // Auto-title = last breadcrumb
        if (breadcrumbList.length > 0) {
          this.pageTitle.set(breadcrumbList[breadcrumbList.length - 1].label);
        }
      });
  }
  private buildBreadcrumbs(
    route: ActivatedRoute,
    url: string,
    breadcrumbs: BreadcrumbItem[],
  ) {
    if (!route.routeConfig) return;
    const routeUrl = route.routeConfig.path ?? '';
    const nextUrl = `${url}/${routeUrl}`;
    const label = route.routeConfig.data?.['breadcrumb'];
    if (label) {
      breadcrumbs.push({
        label,
        url: nextUrl,
      });
    }
    if (route.firstChild) {
      this.buildBreadcrumbs(route.firstChild, nextUrl, breadcrumbs);
    }
  }
}
