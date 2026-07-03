import { enableProdMode, provideZoneChangeDetection } from '@angular/core';
import { environment } from './environments/environment';

import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { provideRouter, withHashLocation } from '@angular/router';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideCharts, withDefaultRegisterables } from 'ng2-charts';
import { routes } from './app/app.module';

export function getBaseUrl()
{
  if (environment.production)
  {
    return environment.apiUrl ?? "https://localhost";
  }

  return "http://localhost:4812/";
}

if (environment.production)
{
  enableProdMode();
}

bootstrapApplication(AppComponent,
  {
    providers:
    [
      provideZoneChangeDetection(),provideRouter(routes, withHashLocation()),
      provideHttpClient(withInterceptorsFromDi()),
      provideAnimations(),
      provideCharts(withDefaultRegisterables()),
      { provide: 'BASE_URL', useFactory: getBaseUrl, deps: [] }
    ]
})
.catch(err => console.error(err));
