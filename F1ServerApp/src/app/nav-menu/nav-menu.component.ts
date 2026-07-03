import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Subscription, timer } from 'rxjs';
import { SignalrService } from '../services/signalr.service';

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
  private timerSubscription: Subscription | undefined;

  constructor(public liveSessionService: SignalrService, private readonly router: Router, @Inject('BASE_URL') baseUrl: string)
  {
    this.apiUrl = baseUrl;

    console.log('Base url: ' + this.apiUrl);
  }

  ngOnInit()
  {
    this.startConnectionCheck();
  }

  ngOnDestroy()
  {
    if (this.timerSubscription != null)
    {
      this.timerSubscription.unsubscribe();
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

  private startConnectionCheck()
  {
    this.timerSubscription = timer(1, 5000).subscribe(() =>
    {
      let currentHubHealthState = this.liveSessionService.isConnected();

      if (this.lastHubConnectionState != currentHubHealthState)
      {
        this.lastHubConnectionState = currentHubHealthState;

        this.reloadCurrentPage();
      }
    });
  }

  private reloadCurrentPage()
  {
    console.info("Current URL: " + this.router.url);

    this.router.navigateByUrl(this.router.url);
  }
}
