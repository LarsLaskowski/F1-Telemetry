import { ChangeDetectorRef, Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { GameViewApiData } from '../data/gameviewdata_api';
import { NotificationService } from '../services/notification.service';

@Component(
{
  imports: [],
  selector: 'app-games',
  templateUrl: './games.component.html'
})
export class GamesComponent {
  public games: GameViewApiData[] = [];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string, private readonly changeDetector: ChangeDetectorRef, private readonly notificationService: NotificationService)
  {
    http.get<GameViewApiData[]>(baseUrl + 'api/games').subscribe(
    {
      next: (result) =>
      {
        this.games = result;
        this.changeDetector.markForCheck();
      },
      error: (err) =>
      {
        console.error(err);
        this.notificationService.showError('Failed to load games.');
      }
    });
  }
}
