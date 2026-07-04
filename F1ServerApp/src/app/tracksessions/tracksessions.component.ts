import { ChangeDetectorRef, Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { SessionViewApiData } from '../data/sessiondata_api';
import { SessionViewData } from '../data/sessionviewdata';
import { ActivatedRoute, RouterModule } from '@angular/router';

@Component(
{
  selector: 'app-sessions',
  imports: [RouterModule],
  templateUrl: './tracksessions.component.html',
  styleUrls: ['./tracksessions.component.css'],
})

export class TrackSessionsComponent
{
  sessions: Set<SessionViewData> = new Set();
  trackId: number = 0;
  trackName: string = "";
  currentSelectedSession: SessionViewData | undefined;

  constructor(http: HttpClient, private route: ActivatedRoute, @Inject('BASE_URL') baseUrl: string, private readonly changeDetector: ChangeDetectorRef)
  {
    this.route.queryParams.subscribe(params =>
    {
      this.trackId = params['trackId'];
      this.trackName = params['trackName'];
    });

    http.get<SessionViewApiData[]>(baseUrl + 'api/sessions/sessionsoftrack/' + this.trackId).subscribe(
    {
      next: (result) =>
      {
        result.forEach((sessionData) =>
        {
          let sessionViewData = new SessionViewData(http, baseUrl);

          sessionViewData.setSessionApiData(sessionData, () => this.changeDetector.markForCheck());

          this.sessions.add(sessionViewData);
        });

        this.changeDetector.markForCheck();
      },
      error: (err) => { console.error(err); }
    });
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
