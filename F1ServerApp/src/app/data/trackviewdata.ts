import { TrackViewApiData } from "./trackviewdata_api";
import { HttpClient } from "@angular/common/http";
import { FastestLapOfTrackViewApiData } from "./fastestlapoftrackviewdata_api";
import { FastestLapOfTrackViewData } from "./fastestlapoftrackviewdata";
import { CalculateTimes } from "../utils/calculatetimes";

export class TrackViewData
{
  // Fields
  trackName: string = "";
  hasSession: boolean = false;
  trackId: number = 0;
  sessions: number = 0;
  referenceLapTime: string = "";
  imagePath: string = "";
  fastestLaps: Set<FastestLapOfTrackViewData> = new Set();
  isLoadingLaps: boolean = false;
  
  private httpClient: HttpClient | undefined;
  private baseUrl: string = "";

  // Constructor
  constructor(http: HttpClient, baseUrl: string)
  {
    this.httpClient = http;
    this.baseUrl = baseUrl;
  }

  // Methods
  setTrackApiData(trackData: TrackViewApiData, onUpdate?: () => void)
  {
    if (trackData)
    {
      this.trackId = trackData.trackId;
      this.hasSession = trackData.hasSession;
      this.trackName = trackData.trackName;
      this.sessions = trackData.sessions;
      this.referenceLapTime = trackData.referenceLapTime;

      if (this.trackId > 0)
      {
        this.imagePath = "/assets/tracks/track-" + this.trackId + ".png";

        if (this.httpClient != null)
        {
          this.isLoadingLaps = true;

          this.httpClient.get<FastestLapOfTrackViewApiData[]>(this.baseUrl + 'api/fastestlap/fastestlap/' + this.trackId).subscribe(
          {
            next: (result) =>
            {
              result.forEach((lapData) =>
              {
                let viewData = new FastestLapOfTrackViewData();

                viewData.formulaType = this.matchFormulaType(lapData.formulaType);
                viewData.sessionLapType = this.matchLapSessionType(lapData.lapSessionType);
                viewData.driverName = lapData.driverName;
                viewData.gameVersionName = lapData.gameVersionName;
                viewData.lapTime = CalculateTimes.matchTimeOutput(lapData.lapTime);
                viewData.referenceTime = CalculateTimes.matchTimeOutput(lapData.referenceTime);
                viewData.diffReference = CalculateTimes.matchTimeDiffOutput(lapData.diffReference);

                this.fastestLaps.add(viewData);
              });

              onUpdate?.();
            },
            error: (err) => { console.log(err); },
            complete: () =>
            {
              this.isLoadingLaps = false;

              onUpdate?.();
            }
          });
        }
      }
    }
  }

  // Match formula type
  private matchFormulaType(formulaNumType: number): string
  {
    let result = "";

    switch (formulaNumType)
    {
      case 0:
        result = "F1";
        break;

      case 2:
        result = "F2";
        break;

      default:
        result = "Unknown";
        break;
    }

    return result;
  }

  // Match lap session type
  private matchLapSessionType(lapSessionType: number): string
  {
    let result: string = ""

    switch (lapSessionType)
    {
      case 0:
        result = "Practice";
        break;

      case 1:
        result = "Qualifying";
        break;

      case 2:
        result = "Race";
        break;
    }

    return result;
  }
}
