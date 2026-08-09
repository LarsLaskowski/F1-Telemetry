import { ChangeDetectorRef, Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { StatisticsViewApiData } from '../data/statisticsviewdata_api';
import { Subscription, timer } from 'rxjs';

@Component(
{
  imports: [],
  selector: 'app-statistics',
  templateUrl: './statistics.component.html',
  styleUrls: ['./statistics.component.css']
})
export class StatisticsComponent {
  public stats: StatisticsViewApiData | undefined;
  private updateSubscription!: Subscription;
  private readonly http!: HttpClient;
  private readonly serviceUrl!: string;
  private readonly changeDetector: ChangeDetectorRef;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string, changeDetector: ChangeDetectorRef)
  {
    this.http = http;
    this.serviceUrl = baseUrl;
    this.changeDetector = changeDetector;
  }

   // Initialization
  ngOnInit()
  {
    this.scheduleNextUpdate();
  }

  // Deinitialization
  ngOnDestroy()
  {
    this.updateSubscription.unsubscribe();
  }

  // Schedule the next statistics update, started only once the current delay has elapsed
  private scheduleNextUpdate()
  {
    this.updateSubscription = timer(250).subscribe(() => { this.updateStatistics() });
  }

  // Update live session data
  private updateStatistics()
  {
    this.http.get<StatisticsViewApiData>(this.serviceUrl + 'api/statistics').subscribe(
    {
      next: (result) =>
      {
        this.stats = result;
        this.changeDetector.markForCheck();
        this.scheduleNextUpdate();
      },
      error: (err) =>
      {
        console.error(err);
        this.scheduleNextUpdate();
      }
    });
  }
}
