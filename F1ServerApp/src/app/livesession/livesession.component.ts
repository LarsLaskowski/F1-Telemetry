import { ChangeDetectorRef, Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { SignalrService } from '../services/signalr.service';
import { interval, Subscription } from 'rxjs';
import { SessionViewApiData } from '../data/sessiondata_api';
import { SessionLiveViewApiData } from '../data/livesessiondata_api';
import { LiveSessionViewData } from '../data/livesessionviewdata';
import { DriverViewData } from '../data/driverviewdata';

@Component(
{
  imports: [CommonModule],
  selector: 'app-livesession',
  templateUrl: './livesession.component.html'
})

export class LiveSessionComponent
{
  public liveSession!: LiveSessionViewData;
  private updateSubscription!: Subscription;
  private readonly http!: HttpClient;
  private readonly serviceUrl!: string;

  // Constructor
  constructor(http: HttpClient, public liveSessionService: SignalrService , @Inject('BASE_URL') baseUrl: string, private readonly changeDetector: ChangeDetectorRef)
  {
    this.http = http;
    this.serviceUrl = baseUrl;
    this.liveSession = new LiveSessionViewData(http, baseUrl);
  }

  // Initialization
  ngOnInit()
  {
    this.updateSubscription = interval(250).subscribe(() => { this.updateLiveSession() });
  }

  // Deinitialization
  ngOnDestroy()
  {
    this.updateSubscription.unsubscribe();
  }

  public getTimeTable(): DriverViewData[]
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

            timeTable.push(driverData);

            position += 1;
          }
        }
      });
    }
    else
    {
      console.log("No timetable data available!");
    }

    return timeTable;
  }

  public getTimeColor(driver: DriverViewData, timeType: number): string
  {
    let classType = 'normalTime';

    if (driver)
    {
      if (timeType == 1 && this.liveSession.fastestSector1DriverId == driver.arrayIndex)
      {
        classType = 'fastestTime';
      }

      if (timeType == 2 && this.liveSession.fastestSector2DriverId == driver.arrayIndex)
      {
        classType = 'fastestTime';
      }

      if (timeType == 3 && this.liveSession.fastestSector3DriverId == driver.arrayIndex)
      {
        classType = 'fastestTime';
      }

      if (timeType == 4 && this.liveSession.fastestLapDriverId == driver.arrayIndex)
      {
        classType = 'fastestTime';
      }
    }

    return classType;
  }

  // Update live session data
  private updateLiveSession()
  {
    this.http.get<SessionLiveViewApiData>(this.serviceUrl + 'api/livesessiondata/').subscribe(
    {
      next: (liveSessionApiData) =>
      {
        if (liveSessionApiData)
        {
          this.liveSession.setLiveSessionApiData(liveSessionApiData);
          this.changeDetector.markForCheck();
        }
      }, error: (err) => { console.error(err) }
    });

    if (this.liveSession && this.liveSession.sessionDbId > 0)
    {
      console.log("Live session db id " + this.liveSession.sessionDbId);

      this.http.get<SessionViewApiData>(this.serviceUrl + 'api/sessions/session/' + this.liveSession.sessionDbId).subscribe(
      {
        next: (sessionApiData) =>
        {
          if (sessionApiData)
          {
            this.liveSession.setSessionApiData(sessionApiData, () => this.changeDetector.markForCheck());
            this.changeDetector.markForCheck();
          }
        },
        error: (err) => { console.error(err); }
      });
    }
  }
}
