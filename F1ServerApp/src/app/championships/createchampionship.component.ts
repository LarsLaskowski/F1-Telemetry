import { ChangeDetectorRef, Component, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ChampionshipCreateData } from '../data/championshipcreatedata';
import { GameViewApiData } from '../data/gameviewdata_api';
import { TrackData } from '../data/trackdata';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIcon } from '@angular/material/icon';
import { MatCheckbox } from '@angular/material/checkbox';
import { MatOption } from '@angular/material/core';
import { MatFormField, MatLabel, MatSelect } from '@angular/material/select';
import { MatButton } from '@angular/material/button';

@Component(
  {
    imports: [MatProgressSpinnerModule, MatIcon, MatCheckbox, MatOption, MatSelect, MatLabel, MatFormField, MatButton],
    selector: 'app-createchampionship',
    templateUrl: './createchampionship.component.html'
  })

export class CreateChampionshipComponent
{
  games: Set<GameViewApiData> = new Set();
  tracks: Map<number, TrackData> = new Map();
  gameTracks: Set<TrackData> = new Set();
  selectedGame: GameViewApiData | undefined;
  careerMode: number = -1;
  isCreating: boolean = false;
  isCreated: boolean = false;
  isCreateable: boolean = false;

  constructor(public http: HttpClient, @Inject('BASE_URL') public serviceUrl: string, @Inject(MAT_DIALOG_DATA) public data: any, public dialogRef: MatDialogRef<CreateChampionshipComponent>, private readonly changeDetector: ChangeDetectorRef)
  {
    this.loadGames();

    // insert all available tracks
    this.tracks.set(0, new TrackData(0, "Melbourne", 0));
    this.tracks.set(1, new TrackData(1, "Paul Ricard", 1));
    this.tracks.set(2, new TrackData(2, "Shanghai", 2));
    this.tracks.set(3, new TrackData(3, "Sakhir (Bahrain)", 3));
    this.tracks.set(4, new TrackData(4, "Catalunya", 4));
    this.tracks.set(5, new TrackData(5, "Monaco", 5));
    this.tracks.set(6, new TrackData(6, "Montreal", 6));
    this.tracks.set(7, new TrackData(7, "Silverstone", 7));
    this.tracks.set(8, new TrackData(8, "Hockenheim", 8));
    this.tracks.set(9, new TrackData(9, "Hungaroring", 9));
    this.tracks.set(10, new TrackData(10, "Spa", 10));
    this.tracks.set(11, new TrackData(11, "Monza", 11));
    this.tracks.set(12, new TrackData(12, "Singapore", 12));
    this.tracks.set(13, new TrackData(13, "Suzuka", 13));
    this.tracks.set(14, new TrackData(14, "Abu Dhabi", 14));
    this.tracks.set(15, new TrackData(15, "Texas", 15));
    this.tracks.set(16, new TrackData(16, "Brazil", 16));
    this.tracks.set(17, new TrackData(17, "Austria", 17));
    this.tracks.set(18, new TrackData(18, "Sochi", 18));
    this.tracks.set(19, new TrackData(19, "Mexico", 19));
    this.tracks.set(20, new TrackData(20, "Baku (Azerbaijan)", 20));
    this.tracks.set(21, new TrackData(25, "Hanoi", 21));
    this.tracks.set(22, new TrackData(26, "Zandvoort", 22));
    this.tracks.set(23, new TrackData(27, "Imola", 23));
    this.tracks.set(24, new TrackData(28, "Portimão", 24));
    this.tracks.set(25, new TrackData(29, "Jeddah", 25));
    this.tracks.set(26, new TrackData(30, "Miami", 26));
    this.tracks.set(27, new TrackData(31, "Las Vegas", 27));
    this.tracks.set(28, new TrackData(32, "Losail", 28));
    this.tracks.set(29, new TrackData(17, "Austria (Steyr)", 29));
    this.tracks.set(30, new TrackData(37, "Madrid", 30));

    this.adjustTracks();

    this.checkIsCreateable();
  }

  loadGames()
  {
    if (this.http != null)
    {
      this.games.clear();

      this.http.get<GameViewApiData[]>(this.serviceUrl + 'api/games').subscribe(
      {
        next: (result) =>
        {
          console.log("Games found: " + result.length)

          result.forEach((gameData) =>
          {
            this.games.add(gameData);
          });

          this.changeDetector.markForCheck();
        },
        error: (err) =>
        {
          console.error(err);
         }
      });
    }
  }

  onGameChanged(event: any)
  {
    this.adjustTracks();

    this.checkIsCreateable();
  }

  onCareerModeChanged(event: any)
  {
    this.checkIsCreateable();
  }

  onTrackSelected(checked: boolean, idx?: number)
  {
    if (idx != null)
    {
      let track = this.tracks.get(idx);

      console.log("selected track index: " + idx);

      if (track != undefined)
      {
        track.selected = checked;

        console.log("selected track: " + track.name);
      }

      this.checkIsCreateable();
    }
  }

  adjustTracks()
  {
    this.gameTracks.clear();

    if (this.selectedGame != undefined)
    {
      if (this.selectedGame.gameVersion == "F1 2020")
      {
        let tracks = [ 0, 3, 21, 2, 22, 4, 5, 20, 6, 1, 17, 7, 9, 10, 11, 12, 18, 13, 15, 19, 16, 14 ];

        this.addTracksToGame(tracks);
      }
      else if (this.selectedGame.gameVersion == "F1 2021")
      {
        // Turkey is between Sochi and Texas
        let tracks = [ 3, 23, 24, 4, 5, 20, 1, 17, 29, 7, 9, 10, 22, 18, 15, 19, 16, 28, 25, 14 ];

        this.addTracksToGame(tracks);
      }
      else if (this.selectedGame.gameVersion == "F1 2022")
      {
        let tracks = [ 3, 25, 0, 23, 26, 4, 5, 20, 6, 7, 17, 9, 10, 22, 11, 12, 13, 15, 19, 16, 14 ];

        this.addTracksToGame(tracks);
      }
      else if (this.selectedGame.gameVersion == "F1 2023")
      {
        let tracks = [ 3, 25, 0, 20, 26, 23, 5, 4, 6, 17, 7, 9, 10, 22, 11, 12, 13, 28, 15, 19, 16, 27, 14 ];

        this.addTracksToGame(tracks);
      }
      else if (this.selectedGame.gameVersion == "F1 2024")
      {
        let tracks = [ 3, 25, 0, 13, 2, 26, 23, 5, 6, 4, 17, 7, 9, 10, 22, 11, 20, 12, 15, 19, 16, 27, 28, 14 ];

        this.addTracksToGame(tracks);
      }
      else if (this.selectedGame.gameVersion == "F1 2025")
      {
        let tracks = [ 0, 2, 13, 3, 25, 26, 23, 5, 4, 6, 17, 7, 10, 9, 22, 11, 20, 12, 15, 19, 16, 27, 28, 14 ];

        this.addTracksToGame(tracks);
      }
      else if (this.selectedGame.gameVersion == "F1 2026")
      {
        let tracks = [ 0, 2, 13, 3, 25, 26, 6, 5, 4, 17, 7, 10, 9, 22, 11, 30, 20, 12, 15, 19, 16, 27, 28, 14 ];

        this.addTracksToGame(tracks);
      }
    }
  }

  addTracksToGame(tracks: number[])
  {
    tracks.forEach(trackId =>
    {
      let track = this.tracks.get(trackId);

      if (track != undefined)
      {
        this.gameTracks.add(track);
      }
    });
  }

  checkIsCreateable()
  {
    let isCreateable = false;

    if (this.selectedGame != undefined && this.careerMode > -1)
    {
      let tracksSelected = 0;

      this.gameTracks.forEach(track =>
      {
        if (track.selected)
        {
          tracksSelected += 1;
        }
      });

      if (tracksSelected >= 6)
      {
        isCreateable = true;
      }
    }

    this.isCreateable = isCreateable;
  }

  createChampionship()
  {
    if (this.http != null && this.selectedGame != null)
    {
      this.isCreating = true;

      let createData = new ChampionshipCreateData();

      createData.gameVersionId = this.selectedGame.id;
      createData.mode = this.careerMode;

      this.tracks.forEach(track =>
      {
        if (track.selected == true)
        {
          createData.tracks.push(track.index);
        }
      })

      const headers = new HttpHeaders().set('Content-Type','application/json')

      this.http.post<boolean>(this.serviceUrl + 'api/championship/createchampionship', JSON.stringify(createData), { headers }).subscribe(
      {
        next: (result) =>
        {
          this.isCreated = result;
          this.isCreating = false;

          this.changeDetector.markForCheck();
        },
        error: (err) => { console.error(err); },
        complete: () =>
        {
          if (this.isCreated)
          {
            this.dialogRef.close(true);
          }
        }
      });
    }
  }
}
