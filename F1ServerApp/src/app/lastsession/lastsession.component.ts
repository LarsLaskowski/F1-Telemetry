import { ChangeDetectorRef, Component, Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { MatTableModule, MatTableDataSource, MatHeaderCell, MatHeaderCellDef, MatColumnDef, MatCell, MatCellDef, MatRow, MatRowDef } from '@angular/material/table';
import { SessionViewApiData } from '../data/sessiondata_api';
import { LastSessionViewData } from '../data/lastsessionviewdata';
import { FinalClassificationViewApiData } from '../data/finalclassificationviewdata_api';
import { ActivatedRoute } from '@angular/router';

@Component(
{
  imports: [CommonModule, MatTableModule, MatHeaderCell, MatCell, MatCellDef, MatHeaderCellDef, MatColumnDef, MatRow, MatRowDef],
  selector: 'app-lastsession',
  templateUrl: './lastsession.component.html',
  styleUrls: ['./lastsession.component.css']
})

export class LastSessionComponent implements OnInit
{
  private readonly http!: HttpClient;
  private readonly serviceUrl!: string;
  public lastSession!: LastSessionViewData;
  isRace: boolean = false;
  lastSessionId: number = 0;
  finalClassifications: FinalClassificationViewApiData[] = [];

  raceColumns: string[] = ['position', 'driver', 'team', 'gridPosition', 'fastestLaptime', 'pitStops', 'lapsDriven', 'totalRacetime'];
  practiceColumns: string[] = ['position', 'driver', 'team', 'fastestLaptime', 'fastestLaptimeDiff', 'lapsDriven'];
  qualifyingColumns: string[] = ['position', 'driver', 'team', 'fastestLaptime', 'fastestLaptimeDiff'];
  displayedColumns: string[] = this.raceColumns.slice();
  dataSource = new MatTableDataSource();

  // Constructor
  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string, private route: ActivatedRoute, private readonly changeDetector: ChangeDetectorRef)
  {
    this.http = http;
    this.serviceUrl = baseUrl;
    this.lastSession = new LastSessionViewData(http, baseUrl);
  }

  ngOnInit()
  {
    this.route.queryParamMap.subscribe(
    {
      next: (params) =>
      {
        if (params.has('session'))
        {
          this.lastSessionId = Number(params.get('session'));

          console.log("Loading session data (param): " + this.lastSessionId);
        }

        this.checkSessionId();
      },
      error: (err) => { console.error(err); }
    });
  }

  checkSessionId()
  {
    // No session id? Search last finished session id
    if (this.lastSessionId == 0)
    {
      console.info("No session number - loading last finished session");

      if (this.http != null)
      {
        this.http.get<number>(this.serviceUrl + 'api/sessions/lastfinishedsession').subscribe(
        {
          next: (result) =>
          {
            this.lastSessionId = result;
          },
          error: (err) => { console.error(err); },
          complete: () => { this.loadSessionData(); }
        });
      }
    }
    else
    {
      console.log("Session number found - load session");

      // Load session data
      this.loadSessionData();
    }
  }

  loadSessionData()
  {
    console.log("Load data for session: " + this.lastSessionId);

    if (this.http != null && this.lastSessionId > 0 && this.lastSession != null)
    {
      this.http.get<SessionViewApiData>(this.serviceUrl + 'api/sessions/session/' + this.lastSessionId).subscribe(
      {
        next: (sessionApiData) =>
        {
          if (sessionApiData)
          {
            this.lastSession.setSessionApiData(sessionApiData);
            this.changeDetector.markForCheck();
          }
        },
        error: (err) => { console.error(err); },
        complete: () => { this.loadTimeTable(); }
      });
    }
  }

  loadTimeTable()
  {
    if (this.http != null && this.lastSessionId > 0 && this.lastSession != null)
    {
      if (this.lastSession.sessionRawType >= 10)
      {
        this.displayedColumns = this.raceColumns.slice();
      }
      else if (this.lastSession.sessionRawType >= 5)
      {
        this.displayedColumns = this.qualifyingColumns.slice();
      }
      else
      {
        this.displayedColumns = this.practiceColumns.slice();
      }

      this.http.get<FinalClassificationViewApiData[]>(this.serviceUrl + 'api/finalclassification/' + this.lastSessionId).subscribe(
      {
        next: (finalClassifications) =>
        {
          if (finalClassifications)
          {
            this.lastSession.setFinalClassificationApiData(finalClassifications);
          }
        },
        error: (err) => { console.error(err); },
        complete: () => { this.prepareFinalClassificationTable(); }
      });
    }
  }

  prepareFinalClassificationTable()
  {
    this.finalClassifications = [];

    if (this.lastSession?.timeTable != null)
    {
      for (let i = 1; i <= this.lastSession.timeTable.size; i++)
      {
        let finalData = this.lastSession.timeTable.get(i);

        if (finalData != null)
        {
          this.finalClassifications.push(finalData);
        }
      }

      this.dataSource.data = this.finalClassifications;

      this.changeDetector.markForCheck();
    };
  }

  public getTimeColor(finalData: FinalClassificationViewApiData): string
  {
    let classType = 'normalTime';

    if (finalData?.isFastestSessionLapTime == true)
    {
      classType = 'fastestTime';
    }

    return classType;
  }

  public getPositionChangeIndicator(finalData: FinalClassificationViewApiData): string
  {
    let imageName = '';

    if (finalData)
    {
      if (finalData.finishPosition > finalData.startingPosition)
      {
        imageName = 'assets/arrow_down.png';
      }
      else if (finalData.finishPosition < finalData.startingPosition)
      {
        imageName = 'assets/arrow_up.png';
      }
      else
      {
        imageName = 'assets/equal.png';
      }
    }

    return imageName;
  }

  public getRaceTime(driverData: FinalClassificationViewApiData): string
  {
    let raceTime = '';

    if (driverData)
    {
      if (driverData.finishPosition == 1)
      {
        raceTime = driverData.totalRaceTime;
      }
      else
      {
        raceTime = driverData.raceTimeDifference;
      }
    }

    return raceTime;
  }

  public isRaceSession(): boolean
  {
    if (this.lastSession != null)
    {
      return this.lastSession.sessionRawType > 9;
    }

    return false;
  }
}
