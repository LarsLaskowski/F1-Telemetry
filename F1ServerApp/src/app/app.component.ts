import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';

import { SignalrService } from './services/signalr.service';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { RouterOutlet } from '@angular/router';

@Component(
{
  imports: [NavMenuComponent, RouterOutlet],
  selector: 'app-root',
  templateUrl: './app.component.html'
})

export class AppComponent
{
  private url!: string;

  title = 'app';

  constructor(public signalRService: SignalrService, private http: HttpClient, @Inject('BASE_URL') baseUrl: string)
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
    this.http.get(this.url + 'api/livesession').subscribe();
  }
}
