import { Injectable, signal } from '@angular/core';
@Injectable({ providedIn: 'root' })
export class ThemeService {
  isDarkTheme = signal(false);
  constructor() {
    const saved = localStorage.getItem('theme');
    if (saved === 'dark') this.enableDark();
  }
  toggle() {
    this.isDarkTheme() ? this.enableLight() : this.enableDark();
  }
  enableDark() {
    document.body.classList.add('dark-theme');
    this.isDarkTheme.set(true);
    localStorage.setItem('theme', 'dark');
  }
  enableLight() {
    document.body.classList.remove('dark-theme');
    this.isDarkTheme.set(false);
    localStorage.setItem('theme', 'light');
  }
}
