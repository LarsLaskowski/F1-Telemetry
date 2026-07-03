export interface FastestLapOfTrackViewApiData
{
  trackId: number;
  gameVersionId: number;
  gameVersionName: string;
  driverId: number;
  formulaType: number;
  driverName: string;
  lapTime: number;
  referenceTime: number;
  diffReference: number;
  lapSessionType: number;
}
