import { ChangeDetectorRef, Component, Inject } from '@angular/core';
import { SignalrService } from '../services/signalr.service';
import { HttpClient, HttpResponse } from '@angular/common/http';

import { GameViewChartData } from '../data/gameviewchartdata';
import { GameViewApiData } from '../data/gameviewdata_api';
import { Chart, ChartData, ChartOptions, ChartType } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';
import { lastValueFrom, Observable, Subscription, timer } from 'rxjs';
import { Router } from '@angular/router';

@Component(
{
  imports: [BaseChartDirective],
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})

export class HomeComponent
{
  apiUrl: string;
  httpClient: HttpClient | undefined;
  games: Set<GameViewChartData> = new Set();
  chartGameLabels: string[] = [];
  chartGameDatas: number[] = [];
  gameChartOptions: ChartOptions = { responsive: true, plugins: { legend: { display: false } }, scales: { y: { beginAtZero: true } } };
  gameChartData: ChartData<'bar'> = { labels: [], datasets: [{ data: [] }] };
  gameChartType: ChartType = 'bar';
  private lastApiHealthState: boolean = false;
  private lastHubHealthState: boolean = false;
  private healthTimerSub: Subscription | undefined;
  private sessionsTimerSub: Subscription | undefined;

  // Constructor
  constructor(http: HttpClient, public liveSessionService: SignalrService, private router: Router, @Inject('BASE_URL') baseUrl: string, private readonly changeDetector: ChangeDetectorRef)
  {
    this.apiUrl = baseUrl;
    this.httpClient = http;

    console.log("Constructor - HomeComponent - start");

    Chart.defaults.font.weight = 'bold';
    Chart.defaults.font.size = 15;

    this.startSessionsReload();
    this.startConnectionCheck();

    console.log("Constructor - HomeComponent - end");
  }

  // Get game session from backend
  getGameSessions()
  {
    if (this.httpClient != null)
    {
      this.games.clear();

      this.httpClient.get<GameViewApiData[]>(this.apiUrl + 'api/games').subscribe(
      {
        next: (result) =>
        {
          console.log("GamesViewApiData: " + result.length)

          result.forEach((gameData) =>
          {
            let gameViewChartData = new GameViewChartData();

            gameViewChartData.setGameData(gameData);

            this.games.add(gameViewChartData);
          });

          this.lastApiHealthState = true;
        },
        error: (err) =>
        {
          this.lastApiHealthState = false;

          console.error(err);
        },
        complete: () =>
        {
          this.fillChart();
          this.changeDetector.markForCheck();
        }
      });
    }
  }

  // Fill the chart
  fillChart()
  {
    console.log("fillChart - start");

    if (this.games != null && this.games.size > 0)
    {
      console.info("Games in chart: " + this.games.size);

      this.chartGameDatas.length = 0;
      this.chartGameLabels.length = 0;

      this.games.forEach(game =>
      {
        console.log("Game: ", game.gameVersion, game.gameSessions);

        this.chartGameLabels.push(game.gameVersion);
        this.chartGameDatas.push(game.gameSessions);
      })

      this.gameChartData =
      {
        labels: this.chartGameLabels,
        datasets: [
          {
            data: this.chartGameDatas
          }
        ]
      };
    }

    console.log("fillChart - end");
  }

  // Page is destroyed
  ngOnDestroy()
  {
    console.info("Home: in ngOnDestroy");

    if (this.healthTimerSub != null)
    {
      this.healthTimerSub.unsubscribe();
    }

    if (this.sessionsTimerSub != null)
    {
      this.sessionsTimerSub.unsubscribe();
    }
  }

  // Reload current page
  private reloadCurrentPage()
  {
    let currentUrl = this.router.url;

    this.router.navigateByUrl('/', { skipLocationChange: true }).then(() =>
    {
      this.router.navigate([currentUrl]).catch(() => console.error("Error while call navigate"));
    }, () => console.error("Err while navigate to url: " + currentUrl));
  }

  // Start reload of game sessions
  private startSessionsReload()
  {
    console.info("Home: in startSessionsReload")

    this.sessionsTimerSub = timer(1, 60000).subscribe(() => this.getGameSessions());
  }

  // Start connection check
  private startConnectionCheck()
  {
    console.info("Home: in startConnectionCheck");

    this.healthTimerSub = timer(1, 30000).subscribe(async () =>
    {
      let currentApiHealthState = await this.getHealthState();
      let currentHubHealthState = this.liveSessionService.isConnected();

      console.info("API health: " + currentApiHealthState);
      console.info("HUB health: " + currentHubHealthState);

      if (this.lastApiHealthState != currentApiHealthState
        || this.lastHubHealthState != currentHubHealthState)
      {
        this.lastApiHealthState = currentApiHealthState;
        this.lastHubHealthState = currentHubHealthState;

        this.reloadCurrentPage();
      }

      this.changeDetector.markForCheck();
    });
  }

  // Get health state
  private async getHealthState(): Promise<boolean>
  {
    const response$ = this.getHealthStateData();

    let healthResponse = await lastValueFrom(response$);

    return healthResponse.status == 200;
  }

  // Get health state data
  private getHealthStateData(): Observable<HttpResponse<Object>>
  {
    if (this.httpClient != null)
    {
      return this.httpClient.get(this.apiUrl + 'api/health', { observe: 'response' });
    }

    return new Observable<HttpResponse<Object>>();
  }

  // Chart is clicked
  public chartClicked(e: any): void
  {
    console.log(e);
  }

  // Chart is hovered
  public chartHovered(e: any): void
  {
    console.log(e);
  }

  // Online?
  public isOnline(): boolean
  {
    let isOnline = this.isHubOnline() && this.isApiOnline();

    return isOnline;
  }

  // Is hub online?
  public isHubOnline(): boolean
  {
    return this.lastHubHealthState;
  }

  // Is API online?
  public isApiOnline(): boolean
  {
    return this.lastApiHealthState;
  }
}
