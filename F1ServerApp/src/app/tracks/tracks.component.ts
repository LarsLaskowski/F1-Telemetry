import { ChangeDetectorRef, Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { TrackViewApiData } from '../data/trackviewdata_api';
import { TrackViewData } from '../data/trackviewdata';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCardContent, MatCardHeader, MatCardTitle, MatCardSubtitle, MatCardTitleGroup, MatCard } from '@angular/material/card';
import { MatSlideToggle, MatSlideToggleChange } from '@angular/material/slide-toggle';

@Component(
 {
    imports: [
    MatProgressSpinnerModule,
    MatCardContent,
    MatCardHeader,
    MatCardTitleGroup,
    MatCardSubtitle,
    MatCardTitle,
    MatCard,
    MatSlideToggle
],
  selector: 'app-tracks',
  templateUrl: './tracks.component.html',
  styleUrls: ['./tracks.component.css']
})

export class TracksComponent
{
  private readonly http!: HttpClient;
  private readonly apiUrl!: string;
  onlyTracksWithSessions = true;
  withReferenceTime = false;
  public tracks: Set<TrackViewData> = new Set();

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string, private readonly changeDetector: ChangeDetectorRef)
  {
    this.apiUrl = baseUrl;
    this.http = http;

    this.loadTracks();
  }

  loadTracks()
  {
    if (this.http != null)
    {
      this.tracks.clear();

      this.http.get<TrackViewApiData[]>(this.apiUrl + 'api/tracks').subscribe(
      {
        next: (result) =>
        {
          result.forEach((trackData) =>
          {
            let trackViewData = new TrackViewData(this.http, this.apiUrl);

            trackViewData.setTrackApiData(trackData, () => this.changeDetector.markForCheck());

            if ((trackViewData.hasSession && this.onlyTracksWithSessions)
              || (trackViewData.hasSession == false && this.onlyTracksWithSessions == false))
            {
              this.tracks.add(trackViewData);
            }
          })

          this.changeDetector.markForCheck();
        },
        error: (err) => { console.error(err); }
      });
    }
  }

  trackIsLoadingLaps(track: TrackViewData): boolean
  {
    let isLoading = false;

    if (track != null)
    {
      isLoading = track.isLoadingLaps;
    }

    return isLoading;
  }

  trackSessionToggled(event: MatSlideToggleChange)
  {
    this.onlyTracksWithSessions = event.checked;

    if (event.checked)
    {
      this.withReferenceTime = false;

      this.loadTracks();
    }
    
  }

  trackReferenceTimeToggled(event: MatSlideToggleChange)
  {
    this.withReferenceTime = event.checked;
  }
}
