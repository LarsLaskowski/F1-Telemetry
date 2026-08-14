import { ChangeDetectorRef, Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { SignalrService } from '../services/signalr.service';
import { SessionViewApiData } from '../data/sessiondata_api';
import { SessionLiveViewApiData } from '../data/livesessiondata_api';
import { LiveSessionViewData } from '../data/livesessionviewdata';
import { DriverViewData } from '../data/driverviewdata';
import { LoggerService } from '../services/logger.service';
import { NotificationService } from '../services/notification.service';

@Component(
{
  imports: [CommonModule],
  selector: 'app-livesession',
  templateUrl: './livesession.component.html'
})

export class LiveSessionComponent implements OnInit, OnDestroy
{
  public liveSession!: LiveSessionViewData;
  public timeTable: DriverViewData[] = [];
  private readonly http!: HttpClient;
  private readonly serviceUrl!: string;
  private readonly onLiveSessionDataUpdated = (liveSessionApiData: SessionLiveViewApiData) => { this.handleLiveSessionDataUpdated(liveSessionApiData); };

  // Constructor
  constructor(http: HttpClient, public liveSessionService: SignalrService , @Inject('BASE_URL') baseUrl: string, private readonly changeDetector: ChangeDetectorRef, private readonly logger: LoggerService, private readonly notificationService: NotificationService)
  {
    this.http = http;
    this.serviceUrl = baseUrl;
    this.liveSession = new LiveSessionViewData(http, baseUrl);
  }

  // Initialization
  ngOnInit()
  {
    this.liveSessionService.addLiveSessionDataListener(this.onLiveSessionDataUpdated);

    this.http.get<SessionLiveViewApiData>(this.serviceUrl + 'api/livesessiondata/').subscribe(
    {
      next: (liveSessionApiData) => { this.handleLiveSessionDataUpdated(liveSessionApiData); },
      error: (err) =>
      {
        console.error(err);
        this.notificationService.showError('Failed to load live session data.');
      }
    });
  }

  // Deinitialization
  ngOnDestroy()
  {
    this.liveSessionService.removeLiveSessionDataListener(this.onLiveSessionDataUpdated);
  }

  // Recompute the time table and the fastest-time highlight classes from the current live session state
  private refreshTimeTable(): void
  {
    let timeTable: DriverViewData[] = [];
    let position = 1;

    if (this.liveSession.timeTable && this.liveSession.timeTable.size > 0)
    {
      this.liveSession.timeTable.forEach((driverNum) =>
      {
        if (this.liveSession.drivers.has(driverNum))
        {
          let driverData = this.liveSession.drivers.get(driverNum);

          if (driverData)
          {
            driverData.setPosition(position);
            driverData.setTimeClasses(this.liveSession.fastestSector1DriverId,
                                       this.liveSession.fastestSector2DriverId,
                                       this.liveSession.fastestSector3DriverId,
                                       this.liveSession.fastestLapDriverId);

            timeTable.push(driverData);

            position += 1;
          }
        }
      });
    }
    else
    {
      this.logger.log("No timetable data available!");
    }

    this.timeTable = timeTable;
  }

  // Handle a live session data update pushed by the SignalR hub
  private handleLiveSessionDataUpdated(liveSessionApiData: SessionLiveViewApiData)
  {
    if (liveSessionApiData)
    {
      const previousSessionDbId = this.liveSession.sessionDbId;
      const previousIsFinished = this.liveSession.isFinished;

      this.liveSession.setLiveSessionApiData(liveSessionApiData);
      this.refreshTimeTable();
      this.changeDetector.markForCheck();

      const sessionChanged = this.liveSession.sessionDbId > 0 && this.liveSession.sessionDbId != previousSessionDbId;
      const sessionJustFinished = this.liveSession.isFinished && this.liveSession.isFinished != previousIsFinished;

      if (this.liveSession.sessionDbId > 0 && (sessionChanged || sessionJustFinished))
      {
        this.logger.log("Live session db id " + this.liveSession.sessionDbId);

        this.loadSessionDetails(this.liveSession.sessionDbId);
      }
    }
  }

  // Load session details for the given session once, e.g. when the live session id changes
  private loadSessionDetails(sessionDbId: number)
  {
    this.http.get<SessionViewApiData>(this.serviceUrl + 'api/sessions/session/' + sessionDbId).subscribe(
    {
      next: (sessionApiData) =>
      {
        if (sessionApiData)
        {
          this.liveSession.setSessionApiData(sessionApiData, () => this.changeDetector.markForCheck());
          this.changeDetector.markForCheck();
        }
      },
      error: (err) =>
      {
        console.error(err);
        this.notificationService.showError('Failed to load session details.');
      }
    });
  }
}
