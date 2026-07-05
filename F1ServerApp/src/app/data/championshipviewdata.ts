import { HttpClient } from "@angular/common/http";
import { ChampionshipViewApiData } from "./championshipdata_api";
import { TrackViewApiData } from "./trackviewdata_api";
import { ChampionshipTrackViewData } from "./championshiptrackviewdata";
import { ChampionshipTrackViewApiData } from "./championshiptrackdata_api";

export class ChampionshipViewData
{
  championshipData: ChampionshipViewApiData | undefined;
  championshipName: string = "";
  championshipId: number = 0;
  isLoadingTracks: boolean = false;
  isTracksLoaded: boolean = false;
  tracks: ChampionshipTrackViewData[] = [];
  httpClient: HttpClient | undefined;
  serviceUrl: string = "";

  // Constructor
  constructor(http: HttpClient, serviceUrl: string)
  {
    this.httpClient = http;
    this.serviceUrl = serviceUrl;
  }

  setChampionshipData(championshipData: ChampionshipViewApiData)
  {
    if (championshipData)
    {
      this.championshipData = championshipData;
      this.championshipId = championshipData.championshipId;

      // Build display name of championship
      this.championshipName = championshipData.gameVersionName + ' - Season ' + championshipData.number;
    }
  }

  loadTracks(): Promise<any>
  {
    return new Promise((resolve) =>
    {
      if (this.isTracksLoaded == false)
      {
        if (this.championshipData)
        {
          this.isLoadingTracks = true;

          this.loadTracksInternal(this.championshipData.tracks).then(() =>
          {
            this.isTracksLoaded = true;
            this.isLoadingTracks = false;

            resolve("tracks loaded!")
          });
        }
        else
        {
          resolve("no data!");
        }
      }
      else
      {
        resolve("tracks already loaded");
      }
    });
  }

  private loadTracksInternal(tracks: ChampionshipTrackViewApiData[]): Promise<any>
  {
    this.tracks = [];

    return new Promise((resolve) =>
    {
      let countTracks = tracks.length;
      let tracksLoaded = 0;

      tracks.forEach((track) =>
      {
        if (this.httpClient != null)
        {
          this.httpClient.get<TrackViewApiData>(this.serviceUrl + 'api/tracks/track/' + track.championshipTrackId).subscribe(
          {
            next: (result) =>
            {
              let championshipTrack = new ChampionshipTrackViewData();

              championshipTrack.trackId = result.trackId;
              championshipTrack.trackName = result.trackName;
              championshipTrack.trackNumber = result.trackNumber;
              championshipTrack.qualifyingPosition = track.qualifyingPosition;
              championshipTrack.sprintQualifyingPosition = track.sprintQualifyingPosition;
              championshipTrack.sprintPosition = track.sprintPosition;
              championshipTrack.racePosition = track.racePosition;
              championshipTrack.racePoints = track.racePoints;
              championshipTrack.sprintPoints = track.sprintPoints;
              championshipTrack.qualifyingDifficulty = track.qualifyingDifficulty;
              championshipTrack.sprintQualifyingDifficulty = track.sprintQualifyingDifficulty;
              championshipTrack.sprintDifficulty = track.sprintDifficulty;
              championshipTrack.raceDifficulty = track.raceDifficulty;

              this.tracks.push(championshipTrack);
            },
            complete: () =>
            {
              console.info("Track " + track.championshipTrackId + " loaded!");

              ++tracksLoaded;

              if (tracksLoaded == countTracks)
              {
                resolve("all tracks loaded!");
              }
            },
            error: (err) => { console.error(err); }
          });
        }
        else
        {
          resolve("no http client!");
        }
      });
    });
  }
}
