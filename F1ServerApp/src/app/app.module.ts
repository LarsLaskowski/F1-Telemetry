import { Routes } from '@angular/router';

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
