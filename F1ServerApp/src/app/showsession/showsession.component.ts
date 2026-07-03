import { ChangeDetectorRef, Component, Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { MatTableDataSource, MatTableModule, MatRowDef, MatCellDef, MatHeaderRowDef, MatHeaderCellDef } from '@angular/material/table';
import { SessionViewApiData } from '../data/sessiondata_api';
import { LastSessionViewData } from '../data/lastsessionviewdata';
import { FinalClassificationViewApiData } from '../data/finalclassificationviewdata_api';
import { MAT_DIALOG_DATA, MatDialogContent, MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import { MatButton } from '@angular/material/button'

@Component(
{
  imports: [CommonModule, MatTableModule, MatRowDef, MatCellDef, MatHeaderRowDef, MatHeaderCellDef, MatDialogContent, MatDialogActions, MatDialogClose, MatButton],
  selector: 'app-showsession',
  templateUrl: './showsession.component.html',
  styleUrls: ['./showsession.component.css']
  })

export class ShowSessionComponent implements OnInit
{
  private readonly http!: HttpClient;
  private readonly serviceUrl!: string;
  public session!: LastSessionViewData;
  isRace: boolean = false;
  sessionId: number = 0;
  finalClassifications: FinalClassificationViewApiData[] = [];

  raceColumns: string[] = ['position', 'driver', 'team', 'gridPosition', 'fastestLaptime', 'pitStops', 'lapsDriven', 'totalRacetime'];
  practiceColumns: string[] = ['position', 'driver', 'team', 'fastestLaptime', 'lapsDriven'];
  qualifyingColumns: string[] = ['position', 'driver', 'team', 'fastestLaptime'];
  displayedColumns: string[] = this.raceColumns.slice();
  dataSource = new MatTableDataSource();

  // Constructor
  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string, @Inject(MAT_DIALOG_DATA) public data: any, private readonly changeDetector: ChangeDetectorRef)
  {
    this.http = http;
    this.serviceUrl = baseUrl;
    this.session = new LastSessionViewData(http, baseUrl);

    if (data != null)
    {
      this.sessionId = data.sessionId;
    }
  }

  ngOnInit()
  {
    this.checkSessionId();
  }

  checkSessionId()
  {
    // No session id? Search last finished session id
    if (this.sessionId > 0)
    {
      console.log("Session number found - load session: " + this.sessionId);

      // Load session data
      this.loadSessionData();
    }
  }

  loadSessionData()
  {
    console.log("Load data for session: " + this.sessionId);

    if (this.http != null && this.sessionId > 0 && this.session != null)
    {
      this.http.get<SessionViewApiData>(this.serviceUrl + 'api/sessions/session/' + this.sessionId).subscribe(
      {
        next: (sessionApiData) =>
        {
          if (sessionApiData)
          {
            this.session.setSessionApiData(sessionApiData, () => this.changeDetector.markForCheck());
          }
        },
        error: (err) => { console.error(err); },
        complete: () => { this.loadTimeTable(); }
      });
    }
  }

  loadTimeTable()
  {
    if (this.http != null && this.sessionId > 0 && this.session != null)
    {
      if (this.session.sessionRawType >= 10)
      {
        this.displayedColumns = this.raceColumns.slice();
      }
      else if (this.session.sessionRawType >= 5)
      {
        this.displayedColumns = this.qualifyingColumns.slice();
      }
      else
      {
        this.displayedColumns = this.practiceColumns.slice();
      }

      this.http.get<FinalClassificationViewApiData[]>(this.serviceUrl + 'api/finalclassification/' + this.sessionId).subscribe(
      {
        next: (finalClassifications) =>
        {
          if (finalClassifications)
          {
            this.session.setFinalClassificationApiData(finalClassifications);
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

    if (this.session?.timeTable != null)
    {
      for (let i = 1; i <= this.session.timeTable.size; i++)
      {
        let finalData = this.session.timeTable.get(i);

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
    if (this.session != null)
    {
      return this.session.sessionRawType > 9;
    }

    return false;
  }
}
