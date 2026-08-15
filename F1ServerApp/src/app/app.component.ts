import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';

import { SignalrService } from './services/signalr.service';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { RouterOutlet } from '@angular/router';

@Component(
{
  imports: [NavMenuComponent, RouterOutlet],
  selector: 'app-root',
  templateUrl: './app.component.html'
})

export class AppComponent implements OnInit
{
  private readonly url: string;

  constructor(public signalRService: SignalrService, private readonly http: HttpClient, @Inject('BASE_URL') baseUrl: string)
  {
    this.url = baseUrl;
  }

  ngOnInit()
  {
    this.signalRService.startConnection(this.url);
    this.signalRService.addLiveSessionListener();
    this.startHttpRequest();
  }

  private startHttpRequest()
  {
    this.http.get(this.url + 'api/livesession').subscribe({ error: (err) => console.error(err) });
  }
}
