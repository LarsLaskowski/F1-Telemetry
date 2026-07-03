import { Injectable } from '@angular/core';
import *  as signalR from '@microsoft/signalr';

@Injectable({ providedIn: 'root' })

export class SignalrService
{
  public isLiveSession: boolean = false;
  public liveSessionId: number = 0;

  private hubConnection!: signalR.HubConnection;

  public startConnection = (baseUrl: string) =>
  {
    this.hubConnection = new signalR.HubConnectionBuilder().withUrl(baseUrl + 'live').build();
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
