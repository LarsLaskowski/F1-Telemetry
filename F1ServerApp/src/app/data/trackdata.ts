export class TrackData
{
  index: number = 0;
  name: string | undefined;
  flagName: string | undefined;
  selected: boolean = false;
  mapIndex: number = 0;

  constructor(idx: number, name: string, mapIndex: number)
  {
    this.index = idx;
    this.name = name;
    this.mapIndex = mapIndex;
  }
}
