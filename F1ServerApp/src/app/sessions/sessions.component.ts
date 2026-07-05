import { ChangeDetectorRef, Component, Inject, ViewChild, AfterViewInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { SessionViewApiData } from '../data/sessiondata_api';
import { SessionPageResultApi } from '../data/sessionpageresult_api';
import { SessionViewDataEx } from '../data/sessionviewdataex';
import { MatTableDataSource, MatTableModule, MatHeaderCell, MatHeaderCellDef, MatRow, MatRowDef, MatCell, MatCellDef, MatColumnDef } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { CarTelemetryComponent } from '../cartelemetry/cartelemetry.component';
import { ShowSessionComponent } from '../showsession/showsession.component';
import { DeleteSessionComponent } from './deletesession.component';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon'
import { MatButtonModule } from '@angular/material/button'
import { MatTooltipModule } from '@angular/material/tooltip'
import { Router } from '@angular/router';

@Component(
{
  imports: [
    MatPaginator,
    MatProgressSpinner,
    MatFormField,
    MatTableModule,
    MatLabel,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatTooltipModule,
    MatHeaderCell,
    MatHeaderCellDef,
    MatRow,
    MatRowDef,
    MatCell,
    MatCellDef,
    MatColumnDef,
    CommonModule
  ],
  selector: 'app-sessions',
  templateUrl: './sessions.component.html',
  styleUrls: ['./sessions.component.css']
})

export class SessionsComponent implements AfterViewInit
{
  displayedColumns: string[] = ['gameInfo', 'track', 'sessionType', 'fastestLap', 'aiDifficulty', 'weather', 'laps', 'telemetry', 'championship', 'delete'];
  sessions: SessionViewDataEx[] = [];
  dataSource = new MatTableDataSource();
  sessionsCount = 0;
  isLoading = false;
  games = new Set<string>();
  expandedSession: SessionViewDataEx | undefined;

  @ViewChild(MatPaginator) paginator!: MatPaginator;


  constructor(public http: HttpClient, @Inject('BASE_URL') public baseUrl: string, public dialog: MatDialog, private readonly router: Router, private readonly changeDetector: ChangeDetectorRef)
  {
  }

  // After view initialization
  ngAfterViewInit()
  {
    this.paginator._intl.itemsPerPageLabel = 'Sessions per page';

    this.loadSessions(0, 15)

    this.paginator.page.subscribe(event =>
    {
      this.loadSessions(event.pageIndex, event.pageSize);
    })
  }

  // Apply filter
  applyFilter(event: Event)
  {
    const filterValue = (event.target as HTMLInputElement).value;

    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  onPageChange(event: PageEvent)
  {
    const pageIndex = event.pageIndex;
    const pageSize = event.pageSize;

    console.log("Page change - pageIndex: " + pageIndex + " pageSize: " + pageSize);

    this.loadSessions(pageIndex, pageSize)
  }

  // Gets class name for fastest times symbol
  public getTimesIconClass(session: SessionViewDataEx): string
  {
    let classType = 'icon-times-not-expanded-human-fastest';

    // Not expanded
    if (this.expandedSession != session)
    {
      // AI player has fastest lap
      if (session.fastestLapDriverIsHuman == false)
      {
        classType = 'icon-times-not-expanded-ai-fastest';

        // AI player drives the fastest lap
        if (session.isFastestLapSector1 && session.isFastestLapSector2 && session.isFastestLapSector3)
        {
          classType = 'icon-times-not-expanded-ai-fastest-overall';
        }
      }
      else
      {
        // Human player drives the fastest lap
        if (session.isFastestLapSector1 && session.isFastestLapSector2 && session.isFastestLapSector3)
        {
          classType = 'icon-times-not-expanded-human-fastest-overall';
        }
      }
    }
    else
    {
      // Session is expanded
      classType = 'icon-times-expanded';
    }

    return classType;
  }

  // Load sessions
  loadSessions(pageIndex: number, pageSize: number)
  {
    this.isLoading = true;

    const params =
    {
      pageIndex: pageIndex.toString(),
      pageSize: pageSize.toString()
    };

    if (this.http != null)
    {
      console.log("Load sessions - pageIndex: " + pageIndex + " pageSize: " + pageSize)

      this.http.get<SessionPageResultApi<SessionViewApiData>>(this.baseUrl + 'api/sessions', { params }).subscribe(
      {
        next: (result) =>
        {
          this.sessionsCount = result.totalCount;
          this.sessions.length = 0;

          this.sessions = result.items.map(item =>
          {
            const session = new SessionViewDataEx(this.http, this.baseUrl);

            session.setSessionApiData(item, () => this.changeDetector.markForCheck());
            session.loadFastestTimes(() => this.changeDetector.markForCheck()); // optional

            return session;
          });

          this.changeDetector.markForCheck();
        },
        error: (err) => { console.error(err); },
        complete: () =>
        {
          setTimeout(() =>
          {
            this.isLoading = false;
            this.changeDetector.markForCheck();
          });

          console.log("Total sessions: " + this.sessionsCount)
        }
      });
    }
  }

  // Show telemetry data dialog
  showTelemetryOverlay(sessionData: SessionViewDataEx)
  {
    if (sessionData.hasCarTelemetry)
    {
      const dialogConfig = new MatDialogConfig();

      dialogConfig.width = '90%';
      dialogConfig.data = { sessionId: sessionData.sessionId, sessionData: sessionData };

      this.dialog.open(CarTelemetryComponent, dialogConfig);
    }
  }

  // Add session to championship
  addToChampionship(sessionData: SessionViewDataEx)
  {
    if (!sessionData.isInChampionship && this.http != null)
    {
      this.http.post(this.baseUrl + 'api/championship/addToChampionship/' + sessionData.sessionId, {}).subscribe(
      {
        next: () =>
        {
          console.log("Session " + sessionData.sessionId + " added to championship");

          sessionData.isInChampionship = true;

          this.changeDetector.markForCheck();
        },
        error: (err) => { console.error(err); }
      });
    }
  }

  showChampionship(sessionData: SessionViewDataEx)
  {
    this.router.navigate(['/championships'], { queryParams: { championshipId: sessionData.championshipId }, skipLocationChange: false }).then(() => { });
  }

  showSessionOverlay(sessionData: SessionViewDataEx)
  {
    const dialogConfig = new MatDialogConfig();

    dialogConfig.width = '90%';
    dialogConfig.data = { sessionId: sessionData.sessionId, sessionData: sessionData };

    this.dialog.open(ShowSessionComponent, dialogConfig);
  }

  deleteSession(sessionData: SessionViewDataEx)
  {
    const dialogConfig = new MatDialogConfig();

    dialogConfig.width = '50%';
    dialogConfig.data = { sessionId: sessionData.sessionId, sessionData: sessionData };

    let dialogRef = this.dialog.open(DeleteSessionComponent, dialogConfig);

    console.log("Open delete session dialog for session: " + sessionData.sessionId);

    dialogRef.afterClosed().subscribe(
    {
      next: (result: boolean) =>
      {
        if (result)
        {
          // reload page, if something was deleted
          this.loadSessions(this.paginator.pageIndex, this.paginator.pageSize)
        }
      },
      error: (err) => { console.error(err); }
    });
  }
}
