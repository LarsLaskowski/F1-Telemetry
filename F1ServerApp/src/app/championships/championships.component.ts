import { ChangeDetectorRef, Component, Inject } from '@angular/core';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { HttpClient } from '@angular/common/http';

import { ChampionshipViewApiData } from '../data/championshipdata_api';
import { ChampionshipViewData } from '../data/championshipviewdata';
import { CreateChampionshipComponent } from './createchampionship.component';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { MatOption } from '@angular/material/core';
import { MatFormField, MatSelect } from '@angular/material/select';
import { MatLabel } from '@angular/material/form-field';
import { MatButton } from '@angular/material/button'
import { ChampionshipTrackViewData } from '../data/championshiptrackviewdata'
import { ActivatedRoute } from '@angular/router';

@Component(
{
  imports: [MatTableModule, MatProgressSpinner, MatOption, MatSelect, MatLabel, MatFormField, MatButton],
  selector: 'app-championships',
  templateUrl: './championships.component.html',
  styleUrls: ['./championships.component.css']
})

export class ChampionshipsComponent
{
  private readonly http!: HttpClient;
  private readonly serviceUrl!: string;
  championships: Set<ChampionshipViewData> = new Set();
  selectedChampionship: ChampionshipViewData | undefined;
  preSelectedChampionshipId: number = 0;
  championshipsCount: any;

  csColumns: string[] = ['track', 'race', 'quali', 'sprint', 'sprintShootout', 'difficulty', 'points'];
  displayedColumns: string[] = this.csColumns.slice();
  dataSource = new MatTableDataSource();

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string, public dialog: MatDialog, private readonly route: ActivatedRoute, private readonly changeDetector: ChangeDetectorRef)
  {
    this.http = http;
    this.serviceUrl = baseUrl;

    this.route.queryParamMap.subscribe(params =>
    {
      const championshipIdString = params.get('championshipId');

      this.preSelectedChampionshipId = championshipIdString ? Number(championshipIdString) : 0;
    });

    this.loadChampionships();
  }

  loadChampionships()
  {
    if (this.http != null)
    {
      this.championships.clear();

      this.http.get<ChampionshipViewApiData[]>(this.serviceUrl + 'api/championship').subscribe(
      {
        next: (result) =>
        {
          if (result.length > 0)
          {
            result.forEach((championship) =>
            {
              let championshipViewData = new ChampionshipViewData(this.http, this.serviceUrl);

              championshipViewData.setChampionshipData(championship);

              this.championships.add(championshipViewData);
            });
          }
        },
        error: (err) => { console.error(err); },
        complete: () =>
        {
          this.championshipsCount = this.championships.size;

          if (this.preSelectedChampionshipId > 0)
          {
            const found = Array.from(this.championships).find(c => c.championshipId === this.preSelectedChampionshipId);

            if (found)
            {
              this.selectedChampionship = found;
              this.onChampionshipChanged();
            }
          }

          this.changeDetector.markForCheck();
        }
      });
    }
  }

  onChampionshipChanged()
  {
    if (this.selectedChampionship != undefined)
    {
      this.selectedChampionship.loadTracks().then(() =>
      {
        console.info("Tracks loaded finished!");

        if (this.selectedChampionship?.tracks != undefined)
        {
          this.sortTracks(this.selectedChampionship);

          this.dataSource.data = this.selectedChampionship.tracks;

          this.changeDetector.markForCheck();
        }
      });
    }
  }

  createChampionship()
  {
    const dialogConfig = new MatDialogConfig();

    dialogConfig.width = '60%';
    dialogConfig.height = '80%';

    let dialogRef = this.dialog.open(CreateChampionshipComponent, dialogConfig);

    dialogRef.afterClosed().subscribe(
    {
      next: (result: boolean) =>
      {
        if (result)
        {
          // reload page, if something was created
          this.loadChampionships();
        }
      },
      error: (err) => { console.error(err); }
    });
  }

  sortTracks(championship: ChampionshipViewData)
  {
    if (championship.tracks != undefined && championship.championshipData != undefined)
    {
      let tracks: number[] = [];

      if (championship.championshipData.gameVersionYear == 2020)
      {
        tracks = [ 0, 3, 21, 2, 22, 4, 5, 20, 6, 1, 17, 7, 9, 10, 11, 12, 18, 13, 15, 19, 16, 14];
      }
      else if (championship.championshipData.gameVersionYear == 2021)
      {
        // Turkey is between Sochi and Texas
        tracks = [ 3, 23, 24, 4, 5, 20, 1, 17, 29, 7, 9, 10, 22, 18, 15, 19, 16, 28, 25, 14];
      }
      else if (championship.championshipData.gameVersionYear == 2022)
      {
        tracks = [ 3, 25, 0, 23, 26, 4, 5, 20, 6, 7, 17, 9, 10, 22, 11, 12, 13, 15, 19, 16, 14];
      }
      else if (championship.championshipData.gameVersionYear == 2023)
      {
        tracks = [ 3, 25, 0, 20, 26, 23, 5, 4, 6, 17, 7, 9, 10, 22, 11, 12, 13, 28, 15, 19, 16, 27, 14 ];
      }
      else if (championship.championshipData.gameVersionYear == 2024)
      {
        tracks = [ 3, 25, 0, 13, 2, 26, 23, 5, 6, 4, 17, 7, 9, 10, 22, 11, 20, 12, 15, 19, 16, 27, 28, 14 ];
      }
      else if (championship.championshipData.gameVersionYear == 2025)
      {
        tracks = [ 0, 2, 13, 3, 25, 26, 23, 5, 4, 6, 17, 7, 10, 9, 22, 11, 20, 12, 15, 19, 16, 27, 28, 14 ];
      }
      else if (championship.championshipData.gameVersionYear == 2026)
      {
        tracks = [ 0, 2, 13, 3, 25, 26, 6, 5, 4, 17, 7, 10, 9, 22, 11, 30, 20, 12, 15, 19, 16, 27, 28, 14 ];
      }

      let sortedTracks: ChampionshipTrackViewData[] = [];

      tracks.forEach(function(idx)
      {
        let trackData = championship.tracks.find(t => t.trackNumber == idx);

        if (trackData != undefined)
        {
          sortedTracks.push(trackData);
        }
      });

      if (sortedTracks.length > 0)
      {
        championship.tracks = sortedTracks;
      }
    }
  }
}
