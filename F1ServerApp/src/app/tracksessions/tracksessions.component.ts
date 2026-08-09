import { ChangeDetectorRef, Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { SessionViewApiData } from '../data/sessiondata_api';
import { SessionViewData } from '../data/sessionviewdata';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { Subscription } from 'rxjs';

@Component(
{
  selector: 'app-sessions',
  imports: [RouterModule],
  templateUrl: './tracksessions.component.html',
  styleUrls: ['./tracksessions.component.css'],
})

export class TrackSessionsComponent implements OnInit, OnDestroy
{
  sessions: Set<SessionViewData> = new Set();
  trackId: number = 0;
  trackName: string = "";
  currentSelectedSession: SessionViewData | undefined;
  private readonly http: HttpClient;
  private readonly baseUrl: string;
  private queryParamsSubscription!: Subscription;

  constructor(http: HttpClient, private route: ActivatedRoute, @Inject('BASE_URL') baseUrl: string, private readonly changeDetector: ChangeDetectorRef)
  {
    this.http = http;
    this.baseUrl = baseUrl;
  }

  // Initialization
  ngOnInit()
  {
    this.queryParamsSubscription = this.route.queryParams.subscribe(params =>
    {
      this.trackId = params['trackId'];
      this.trackName = params['trackName'];
    });

    this.http.get<SessionViewApiData[]>(this.baseUrl + 'api/sessions/sessionsoftrack/' + this.trackId).subscribe(
    {
      next: (result) =>
      {
        result.forEach((sessionData) =>
        {
          let sessionViewData = new SessionViewData(this.http, this.baseUrl);

          sessionViewData.setSessionApiData(sessionData, () => this.changeDetector.markForCheck());

          this.sessions.add(sessionViewData);
        });

        this.changeDetector.markForCheck();
      },
      error: (err) => { console.error(err); }
    });
  }

  // Deinitialization
  ngOnDestroy()
  {
    this.queryParamsSubscription.unsubscribe();
  }

  expandSession(selectedSession: SessionViewData)
  {
    this.currentSelectedSession = selectedSession;

    if (this.currentSelectedSession.isExpanded == null || this.currentSelectedSession.isExpanded == false)
    {
      this.currentSelectedSession.expandSession(true, () => this.changeDetector.markForCheck());
    }
    else
    {
      this.currentSelectedSession.expandSession(false, () => this.changeDetector.markForCheck())
    }
  }
}
