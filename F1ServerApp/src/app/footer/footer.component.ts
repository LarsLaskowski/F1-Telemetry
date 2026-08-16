import { Component } from '@angular/core';

import { version } from '../../../package.json';

@Component(
{
  imports: [],
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.css']
})

export class FooterComponent
{
  public readonly appVersion: string = version;
}
