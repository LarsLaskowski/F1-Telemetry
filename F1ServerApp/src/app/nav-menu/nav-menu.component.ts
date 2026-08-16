import { ChangeDetectorRef, Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';
import { SignalrService } from '../services/signalr.service';
import { LoggerService } from '../services/logger.service';

@Component(
{
  imports: [RouterModule, CommonModule],
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})

export class NavMenuComponent implements OnInit, OnDestroy
{
  public isExpanded = false;
  public apiUrl: string;
  private lastHubConnectionState: boolean = false;
  private hubConnectedSub: Subscription | undefined;

  constructor(public liveSessionService: SignalrService, private readonly router: Router, @Inject('BASE_URL') baseUrl: string, private readonly logger: LoggerService, private readonly changeDetector: ChangeDetectorRef)
  {
    this.apiUrl = baseUrl;

    this.logger.log('Base url: ' + this.apiUrl);
  }

  ngOnInit()
  {
    this.startConnectionCheck();
  }

  ngOnDestroy()
  {
    if (this.hubConnectedSub != null)
    {
      this.hubConnectedSub.unsubscribe();
    }
  }

  collapse()
  {
    this.isExpanded = false;
  }

  toggle()
  {
    this.isExpanded = !this.isExpanded;
  }

  isLiveSession(): boolean
  {
    let isLiveSession = false;

    if (this.liveSessionService != null && this.lastHubConnectionState == true)
    {
      isLiveSession = this.liveSessionService.isLiveSession;
    }

    return isLiveSession;
  }

  liveStatusTooltip(): string
  {
    if (this.lastHubConnectionState == false)
    {
      return 'Live status unavailable: not connected to the server';
    }

    if (this.isLiveSession())
    {
      return 'Live session in progress - click to view';
    }

    return 'No live session in progress';
  }

  private startConnectionCheck()
  {
    this.hubConnectedSub = this.liveSessionService.hubConnected$.subscribe(currentHubHealthState =>
    {
      if (this.lastHubConnectionState != currentHubHealthState)
      {
        this.lastHubConnectionState = currentHubHealthState;

        this.reloadCurrentPage();
      }

      this.changeDetector.markForCheck();
    });
  }

  private reloadCurrentPage()
  {
    this.logger.info("Current URL: " + this.router.url);

    this.router.navigateByUrl(this.router.url);
  }
}
