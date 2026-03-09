import { provideHttpClient } from '@angular/common/http';
import {
  ApplicationConfig,
  importProvidersFrom,
  provideBrowserGlobalErrorListeners,
  provideZoneChangeDetection,
} from '@angular/core';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideRouter } from '@angular/router';
import { ModalModule } from 'ngx-bootstrap/modal';
import { provideToastr } from 'ngx-toastr';
import { routes } from './app.routes';
export const appConfig: ApplicationConfig = {
  providers: [
    // Global runtime diagnostics
    provideBrowserGlobalErrorListeners(),
    // Http client WITHOUT interceptor (we now use ApiHandlerService)
    provideHttpClient(),
    // Performance optimization
    provideZoneChangeDetection({ eventCoalescing: true }),
    // Angular routing
    provideRouter(routes),
    // Required for ngx-toastr
    provideAnimations(),
    // Toastr global settings
    provideToastr({
      positionClass: 'toast-top-right',
      preventDuplicates: true,
      closeButton: true,
      timeOut: 3500,
    }),
    // Bootstrap modal support
    importProvidersFrom(ModalModule.forRoot()),
  ],
};
