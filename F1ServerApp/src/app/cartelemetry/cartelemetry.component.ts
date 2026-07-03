import { ChangeDetectorRef, Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { MatDialogTitle, MAT_DIALOG_DATA, MatDialogContent, MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import { CarTelemetryViewData } from '../data/cartelemetryviewdata';
import { Chart, ChartData, ChartOptions, ChartType } from 'chart.js';
import { ParticipantViewApiData } from '../data/participantdata_api';
import { BaseChartDirective } from 'ng2-charts';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { MatButton } from '@angular/material/button';

@Component(
{
  imports: [BaseChartDirective, MatProgressSpinner, MatDialogTitle, MatDialogContent, MatDialogActions, MatButton, MatDialogClose],
  selector: 'app-cartelemetry',
  templateUrl: './cartelemetry.component.html',
  styleUrls: ['./cartelemetry.component.css']
  })

export class CarTelemetryComponent
{
  telemetries: Set<CarTelemetryViewData> = new Set();
  chartLabels: string[] = [];
  speedDatas: number[] = [];
  throttleDatas: number[] = [];
  brakeDatas: number[] = [];
  gearDatas: number[] = [];
  chartSpeedData: ChartData<'line'> = { labels: [], datasets: [{ data: [] }] };
  chartThrottleData: ChartData<'line'> = { labels: [], datasets: [{ data: [] }] };
  chartBrakeData: ChartData<'line'> = { labels: [], datasets: [{ data: [] }] };
  chartType: ChartType = 'line';
  isLoadingData: boolean = false;
  chartSpeedOptions: ChartOptions =
    {
      elements: { point: { radius: 0 } },
      maintainAspectRatio: false,
      plugins: { legend: { display: false } },
      scales:
      {
        y: { beginAtZero: true, type: 'linear', position: 'left' },
        y1: { beginAtZero: true, type: 'linear', position: 'right', grid: { display: false } }
      }
    };
  chartThrottleOptions: ChartOptions =
    {
      elements: { point: { radius: 0 } },
      maintainAspectRatio: false,
      plugins: { legend: { display: false } },
      scales:
      {
        y: { beginAtZero: true, type: 'linear', position: 'left' }
      }
    };
  chartBrakeOptions: ChartOptions =
    {
      elements:
      {
        point: { radius: 0, hitRadius: 10, hoverRadius: 10, backgroundColor: 'rgb(232,76,94)' }
      },
      maintainAspectRatio: false,
      plugins: { legend: { display: false } },
      scales:
      {
        y: { beginAtZero: true, type: 'linear', position: 'left' }
      }
    };

  // Constructor
  constructor(@Inject(MAT_DIALOG_DATA) public data: any, public http: HttpClient, @Inject('BASE_URL') public baseUrl: string, private readonly changeDetector: ChangeDetectorRef)
  {
    Chart.defaults.font.size = 15;

    this.isLoadingData = true;

    console.info("Session id: " + data.sessionId);

    this.loadParticipants(data.sessionId);
  }

  // Load participants of session
  loadParticipants(sessionId: number)
  {
    if (sessionId > 0 && this.http != null)
    {
      let humanParticipantId = 0;

      this.http.get<ParticipantViewApiData[]>(this.baseUrl + "api/participants/participantsofsession/" + sessionId).subscribe(
      {
        next: (result) =>
        {
          result.forEach((participant) =>
          {
            if (participant.isHumanControlled)
            {
              humanParticipantId = participant.participantDbId;
            }
          })
        },
        error: (err) => { console.error(err); },
        complete: () => { this.loadTelemetryData(humanParticipantId); }
      });
    }
  }

  // Load telemetry data
  loadTelemetryData(humanParticipantId: number)
  {
    if (humanParticipantId > 0 && this.http != null)
    {
      this.http.get<CarTelemetryViewData[]>(this.baseUrl + 'api/cartelemetry/telemetrybyparticipantfastestlap/' + humanParticipantId).subscribe(
      {
        next: (result) =>
        {
          console.info("Telemetry data points: " + result.length);

          result.forEach((entry) =>
          {
            this.telemetries.add(entry);
          });
        },
        error: (err) => { console.error(err); },
        complete: () => { this.fillChart(); }
      });
    }
  }

  // Fill the chart
  fillChart()
  {
    console.log("fillChart - start");

    if (this.telemetries != null && this.telemetries.size > 0)
    {
      this.speedDatas.length = 0;
      this.throttleDatas.length = 0;
      this.brakeDatas.length = 0;
      this.gearDatas.length = 0;

      this.chartLabels.length = 0;

      this.telemetries.forEach(telemetry =>
      {
        this.speedDatas.push(telemetry.speed);
        this.throttleDatas.push(telemetry.throttle);
        this.brakeDatas.push(telemetry.brake);
        this.gearDatas.push(telemetry.gear);
        
        this.chartLabels.push(telemetry.distance.toString(10));
      })

      this.chartSpeedData =
      {
        labels: this.chartLabels,
        datasets: [
          {
            label: 'Speed',
            borderColor: 'rgb(71,164,233)',
            borderWidth: 3.5,
            data: this.speedDatas,
            tension: 0.2,
            yAxisID: 'y'
          },
          {
            label: 'Gear',
            borderColor: 'rgb(128,128,128,0.5)',
            borderWidth: 2.5,
            data: this.gearDatas,
            tension: 0.2,
            yAxisID: 'y1'
          }
        ]
      };

      this.chartThrottleData =
      {
        labels: this.chartLabels,
        datasets: [
          {
            label: 'Throttle',
            borderColor: 'rgb(0,153,84)',
            data: this.throttleDatas,
            tension: 0.2,
            yAxisID: 'y',
            borderWidth: 3.5
          }
        ]
      };

      this.chartBrakeData =
      {
        labels: this.chartLabels,
        datasets: [
          {
            label: 'Brake',
            borderColor: 'rgb(232,76,94)',
            data: this.brakeDatas,
            tension: 0.2,
            yAxisID: 'y',
            borderWidth: 3.5
          }
        ]
      };
    }

    this.isLoadingData = false;

    this.changeDetector.markForCheck();

    console.log("fillChart - end");
  }

  // Chart is clicked
  public chartSpeedClicked(e: any): void
  {
    console.log(e);
  }

  // Chart is hovered
  public chartSpeedHovered(e: any): void
  {
    console.log(e);
  }

  // Chart is clicked
  public chartThrottleClicked(e: any): void
  {
    console.log(e);
  }

  // Chart is hovered
  public chartThrottleHovered(e: any): void
  {
    console.log(e);
  }

  // Chart is clicked
  public chartBrakeClicked(e: any): void
  {
    console.log(e);
  }

  // Chart is hovered
  public chartBrakeHovered(e: any): void
  {
    console.log(e);
  }
}
