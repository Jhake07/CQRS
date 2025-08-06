import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class PaginatorService {
  getPaginated<T>(data: T[], pageSize: number, currentPage: number): T[] {
    const start = (currentPage - 1) * pageSize;
    return data.slice(start, start + pageSize);
  }

  getTotalPages<T>(data: T[], pageSize: number): number {
    return Math.ceil(data.length / pageSize);
  }
}
