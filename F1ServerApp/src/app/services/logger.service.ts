import { Injectable } from '@angular/core';

import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })

export class LoggerService
{
  public log(message?: unknown, ...optionalParams: unknown[]): void
  {
    if (environment.production == false)
    {
      console.log(message, ...optionalParams);
    }
  }

  public info(message?: unknown, ...optionalParams: unknown[]): void
  {
    if (environment.production == false)
    {
      console.info(message, ...optionalParams);
    }
  }
}
