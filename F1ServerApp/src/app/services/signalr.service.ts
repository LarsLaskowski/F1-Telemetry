import { Injectable } from '@angular/core';
import *  as signalR from '@microsoft/signalr';

import { SessionLiveViewApiData } from '../data/livesessiondata_api';

@Injectable({ providedIn: 'root' })

export class SignalrService
{
  public isLiveSession: boolean = false;
  public liveSessionId: number = 0;

  private hubConnection!: signalR.HubConnection;

  public startConnection = (baseUrl: string) =>
  {
    this.hubConnection = new signalR.HubConnectionBuilder().withUrl(baseUrl + 'live').withAutomaticReconnect().build();
    this.hubConnection.start().then(() => console.log('Connection started'))
    .catch(err => console.log('Error while start connection: ' + err))
  }

  public addLiveSessionListener = () =>
  {
    this.hubConnection.on('islivesession', (isLiveSession, liveSessionId) =>
    {
      if (isLiveSession != this.isLiveSession || liveSessionId != this.liveSessionId)
      {
        console.log('Live session information: ' + isLiveSession + ' id: ' + liveSessionId);
      }

      this.isLiveSession = isLiveSession;
      this.liveSessionId = liveSessionId;
    })
  }

  public addLiveSessionDataListener = (callback: (liveSessionData: SessionLiveViewApiData) => void) =>
  {
    this.hubConnection.on('livesessiondataupdated', callback);
  }

  public removeLiveSessionDataListener = (callback: (liveSessionData: SessionLiveViewApiData) => void) =>
  {
    this.hubConnection.off('livesessiondataupdated', callback);
  }

  public isConnected(): boolean
  {
    let isConnected = false;

    if (this.hubConnection != null)
    {
      isConnected = this.hubConnection.state == signalR.HubConnectionState.Connected;
    }

    return isConnected;
  }
}
