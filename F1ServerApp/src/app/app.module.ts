import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { Routes, RouterModule } from '@angular/router';
import { provideCharts, withDefaultRegisterables } from 'ng2-charts';
import { MaterialModule } from './material.module';

import { SignalrService } from './services/signalr.service';

export const routes: Routes = [
  { path: '', loadComponent: () => import('./home/home.component').then(m => m.HomeComponent) },
  { path: 'games', loadComponent: () => import('./games/games.component').then(m => m.GamesComponent) },
  { path: 'sessionsview', loadComponent: () => import('./sessions/sessions.component').then(m => m.SessionsComponent) },
  { path: 'tracks', loadComponent: () => import('./tracks/tracks.component').then(m => m.TracksComponent) },
  { path: 'livesession', loadComponent: () => import('./livesession/livesession.component').then(m => m.LiveSessionComponent) },
  { path: 'tracksessionsview', loadComponent: () => import('./tracksessions/tracksessions.component').then(m => m.TrackSessionsComponent) },
  { path: 'lastsession', loadComponent: () => import('./lastsession/lastsession.component').then(m => m.LastSessionComponent) },
  { path: 'cartelemetry', loadComponent: () => import('./cartelemetry/cartelemetry.component').then(m => m.CarTelemetryComponent) },
  { path: 'showsession', loadComponent: () => import('./showsession/showsession.component').then(m => m.ShowSessionComponent) },
  { path: 'deletesession', loadComponent: () => import('./sessions/deletesession.component').then(m => m.DeleteSessionComponent) },
  { path: 'statistics', loadComponent: () => import('./statistics/statistics.component').then(m => m.StatisticsComponent) },
  { path: 'championships', loadComponent: () => import('./championships/championships.component').then(m => m.ChampionshipsComponent) },
  { path: 'createchampionship', loadComponent: () => import('./championships/createchampionship.component').then(m => m.CreateChampionshipComponent) }
];

@NgModule({
  declarations: [],
  imports: [
    BrowserModule,
    FormsModule,
    RouterModule.forRoot(routes, { useHash: true }),
    MaterialModule
  ],
  providers: [
    SignalrService,
    provideHttpClient(withInterceptorsFromDi()),
    provideCharts(withDefaultRegisterables())
  ]
})
export class AppModule {}
