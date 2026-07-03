import { ChangeDetectorRef, Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { StatisticsViewApiData } from '../data/statisticsviewdata_api';
import { interval, Subscription } from 'rxjs';

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
    this.updateSubscription = interval(250).subscribe(() => { this.updateStatistics() });
  }

  // Deinitialization
  ngOnDestroy()
  {
    this.updateSubscription.unsubscribe();
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
      },
      error: (err) => { console.error(err); }
    });
  }
}
