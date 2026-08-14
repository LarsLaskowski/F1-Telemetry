import { ChangeDetectorRef, Component, Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { GameViewApiData } from '../data/gameviewdata_api';
import { NotificationService } from '../services/notification.service';

@Component(
{
  imports: [],
  selector: 'app-games',
  templateUrl: './games.component.html'
})
export class GamesComponent implements OnInit {
  public games: GameViewApiData[] = [];

  constructor(private readonly http: HttpClient, @Inject('BASE_URL') private readonly baseUrl: string, private readonly changeDetector: ChangeDetectorRef, private readonly notificationService: NotificationService)
  {
  }

  ngOnInit(): void
  {
    this.http.get<GameViewApiData[]>(this.baseUrl + 'api/games').subscribe(
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
