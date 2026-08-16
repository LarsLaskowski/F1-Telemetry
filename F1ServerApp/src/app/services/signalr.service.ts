import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, Observable } from 'rxjs';

import { SessionLiveViewApiData } from '../data/livesessiondata_api';
import { LoggerService } from './logger.service';

@Injectable({ providedIn: 'root' })

export class SignalrService
{
  private readonly isLiveSessionSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  private readonly liveSessionIdSubject: BehaviorSubject<number> = new BehaviorSubject<number>(0);
  private readonly hubConnectedSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  public readonly isLiveSession$: Observable<boolean> = this.isLiveSessionSubject.asObservable();
  public readonly liveSessionId$: Observable<number> = this.liveSessionIdSubject.asObservable();
  public readonly hubConnected$: Observable<boolean> = this.hubConnectedSubject.asObservable();

  private hubConnection!: signalR.HubConnection;

  constructor(private readonly logger: LoggerService)
  {
  }

  public get isLiveSession(): boolean
  {
    return this.isLiveSessionSubject.value;
  }

  public get liveSessionId(): number
  {
    return this.liveSessionIdSubject.value;
  }

  public startConnection(baseUrl: string): void
  {
    this.hubConnection = new signalR.HubConnectionBuilder().withUrl(baseUrl + 'live', { withCredentials: false }).withAutomaticReconnect().build();

    this.hubConnection.onreconnecting(() => this.hubConnectedSubject.next(false));
    this.hubConnection.onreconnected(() => this.hubConnectedSubject.next(true));
    this.hubConnection.onclose(() => this.hubConnectedSubject.next(false));

    this.hubConnection.start().then(() =>
    {
      this.logger.log('Connection started');

      this.hubConnectedSubject.next(true);
    })
    .catch(err => console.error('Error while start connection: ' + err))
  }

  public addLiveSessionListener(): void
  {
    this.hubConnection.on('islivesession', (isLiveSession, liveSessionId) =>
    {
      if (isLiveSession !== this.isLiveSession || liveSessionId !== this.liveSessionId)
      {
        this.logger.log('Live session information: ' + isLiveSession + ' id: ' + liveSessionId);
      }

      this.isLiveSessionSubject.next(isLiveSession);
      this.liveSessionIdSubject.next(liveSessionId);
    })
  }

  public addLiveSessionDataListener(callback: (liveSessionData: SessionLiveViewApiData) => void): void
  {
    this.hubConnection.on('livesessiondataupdated', callback);
  }

  public removeLiveSessionDataListener(callback: (liveSessionData: SessionLiveViewApiData) => void): void
  {
    this.hubConnection.off('livesessiondataupdated', callback);
  }
}
