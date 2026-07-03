import { ParticipantViewData } from "./participantviewdata";
import { SessionViewApiData } from "./sessiondata_api";
import { ParticipantViewApiData } from '../data/participantdata_api';
import { HttpClient } from "@angular/common/http";

export interface ISessionViewData
{
  // Fields
  sessionId: number;
  gameVersion: string;
  track: string;
  formulaType: string;
  sessionType: string;
  sessionRawType: number;
  cars: number;
  aiDifficulty: number;
  weatherType: string;
  hasCarTelemetry: boolean;
  isTelemetryChecking: boolean;
  isActiveChampionship: boolean;
  isInChampionship: boolean;
  isChampionshipChecking: boolean;
  isExpanded: boolean;
  participants: Set<ParticipantViewData>;
  httpClient: HttpClient | undefined;
  baseUrl: string;
}

export class SessionViewData implements ISessionViewData
{
  // Fields
  sessionId: number = 0;
  gameVersion: string = "";
  track: string = "";
  formulaType: string = "";
  sessionType: string = "";
  sessionRawType: number = 0;
  cars: number = 0;
  aiDifficulty: number = 0;
  weatherType: string = "";
  hasCarTelemetry: boolean = false;
  isTelemetryChecking: boolean = false;
  isActiveChampionship: boolean = false;
  championshipId: number = 0;
  isInChampionship: boolean = false;
  isChampionshipChecking: boolean = false;
  isExpanded: boolean = false;
  participants: Set<ParticipantViewData> = new Set();
  httpClient: HttpClient | undefined;
  baseUrl: string = "";
  hideImageButtons: boolean = true

  // Constructor
  constructor(http: HttpClient, baseUrl: string)
  {
    this.httpClient = http;
    this.baseUrl = baseUrl;
  }

  // Methods
  setSessionApiData(sessionData: SessionViewApiData, onUpdate?: () => void)
  {
    if (sessionData)
    {
      this.sessionId = sessionData.sessionDbId;
      this.gameVersion = sessionData.gameVersion;
      this.track = sessionData.track;
      this.cars = sessionData.cars;
      this.aiDifficulty = sessionData.aiDifficulty;
      this.sessionRawType = sessionData.sessionType;

      this.matchFormulaType(sessionData.formulaType);
      this.matchSessionType(sessionData.sessionType);
      this.matchWeatherType(sessionData.weather);

      this.isTelemetryChecking = true;
      this.checkCarTelemetry(onUpdate);

      this.isChampionshipChecking = true;
      this.checkChampionship(onUpdate);
    }
  }

  expandSession(expand: boolean, onUpdate?: () => void)
  {
    this.isExpanded = expand;

    if (expand)
    {
      if (this.httpClient != null)
      {
        this.httpClient.get<ParticipantViewApiData[]>(this.baseUrl + 'api/participants/participantsofsession/' + this.sessionId).subscribe(
        {
          next: (result) =>
          {
            result.forEach((participantData) =>
            {
              let participantViewData = new ParticipantViewData();

              participantViewData.setParticipantApiData(participantData);

              this.participants.add(participantViewData);
            });

            onUpdate?.();
          },
          error: (err) => { console.error(err); }
        });
      }
    }
    else
    {
      this.participants.clear();
    }
  }

  // Telemetry available and loading finished?
  isCarTelemetry(): boolean
  {
    let hasTelemetry = false;

    if (this.hasCarTelemetry && this.isTelemetryChecking == false)
    {
      hasTelemetry = this.hasCarTelemetry;
    }

    return hasTelemetry;
  }

  hasActiveChampionship(): boolean
  {
    return this.isActiveChampionship && this.isChampionshipChecking == false;
  }

  isSessionInChampionship(): boolean
  {
    let isInChampionship = false;

    if (this.isActiveChampionship && this.isInChampionship && this.isChampionshipChecking == false)
    {
      isInChampionship = true;
    }

    return isInChampionship;
  }

  // Match formula type
  private matchFormulaType(formulaNumType: number)
  {
    switch (formulaNumType)
    {
      case 0:
        this.formulaType = "F1";
        break;

      case 1:
        this.formulaType = "F1 Classic";
        break;

      case 2:
        this.formulaType = "F2";
        break;

      case 3:
        this.formulaType = "F1 Generic";
        break;

      case 4:
        this.formulaType = "Beta";
        break;

      case 5:
        this.formulaType = "Supercars";
        break;

      case 6:
        this.formulaType = "E-Sports";
        break;

      case 7:
        this.formulaType = "F2 2021";
        break;

      case 8:
        this.formulaType = "F1 World";
        break;

      case 9:
        this.formulaType = "F1 Elimination";
        break;

      case 13:
        this.formulaType = "F1";
        break;

      default:
        this.formulaType = "Unknown";
        break;
    }
  }

  // Match session type
  private matchSessionType(sessionNumType: number)
  {
    switch (sessionNumType)
    {
      case 0:
        this.sessionType = "Not set";
        break;

      case 1:
        this.sessionType = "Practice session 1";
        break;

      case 2:
        this.sessionType = "Practice session 2";
        break;

      case 3:
        this.sessionType = "Practice session 3";
        break;

      case 4:
        this.sessionType = "Short practice";
        break;

      case 5:
        this.sessionType = "Qualifying session 1";
        break;

      case 6:
        this.sessionType = "Qualifying session 2";
        break;

      case 7:
        this.sessionType = "Qualifying session 3";
        break;

      case 8:
        this.sessionType = "Short qualifying";
        break;

      case 9:
        this.sessionType = "One shot qualifying";
        break;

      case 10:
        // Race
        this.sessionType = "Race";
        break;

      case 11:
        // Race 2
        this.sessionType = "Race";
        break;

      case 12:
        this.sessionType = "Race";
        break;

      case 13:
        this.sessionType = "Time trial";
        break;

      case 14:
        this.sessionType = "Sprint shootout 1";
        break;

      case 15:
        this.sessionType = "Sprint shootout 2";
        break;

      case 16:
        this.sessionType = "Sprint shootout 3";
        break;

      case 17:
        this.sessionType = "Short sprint shootout";
        break;

      case 18:
        this.sessionType = "One shot sprint shootout";
        break;

      case 100:
        this.sessionType = "Sprint";
        break;

      default:
        this.sessionType = "Unknown";
        break;
    }
  }

  // Match weather type
  private matchWeatherType(weather: number)
  {
    switch (weather)
    {
      case 0:
        this.weatherType = "sunny";
        break;

      case 1:
        this.weatherType = "partly_cloudy_day";
        break;

      case 2:
        this.weatherType = "cloudy";
        break;

      case 3:
        this.weatherType = "rainy";
        break;

      case 4:
        this.weatherType = "rainy_heavy";
        break;

      case 5:
        this.weatherType = "thunderstorm";
        break;

      default:
        this.weatherType = "sunny";
        break;
    }
  }

  // Car telemetry available?
  private checkCarTelemetry(onUpdate?: () => void)
  {
    if (this.httpClient != null)
    {
      this.httpClient.get<boolean>(this.baseUrl + 'api/cartelemetry/hasusertelemetry/' + this.sessionId).subscribe(
      {
        next: (result) =>
        {
          this.hasCarTelemetry = result;
        },
        error: (err) => { console.error(err); },
        complete: () =>
        {
          this.isTelemetryChecking = false;

          onUpdate?.();
        }
      });
    }
  }

  // Championship active?
  private checkChampionship(onUpdate?: () => void)
  {
    if (this.httpClient != null)
    {
      this.httpClient.get<number>(this.baseUrl + 'api/championship/isactivechampionship/' + this.sessionId).subscribe(
      {
        next: (championshipId) =>
        {
          if (championshipId > 0 && this.httpClient != null)
          {
            this.httpClient.get<boolean>(this.baseUrl + 'api/championship/issessioninchampionshipactive/' + championshipId + '/' + this.sessionId).subscribe(
            {
              next: (result) =>
              {
                this.championshipId = championshipId;
                this.isActiveChampionship = championshipId > 0;
                this.isInChampionship = result;

                onUpdate?.();
              },
              error: (err) => { console.error(err); }
            });
          }
        },
        error: (err) => { console.error(err); },
        complete: () =>
        {
          this.isChampionshipChecking = false;

          onUpdate?.();
        }
      });
    }
  }
}
