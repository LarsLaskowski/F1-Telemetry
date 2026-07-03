export interface LiveDriverApiData
{
  arrayIndex: number;
  driverName: string;
  carNumber: number;
  gridPosition: number;
  carPosition: number;
  nationality: string;
  teamName: string;
  currentDriverStatus: number;
  currentLapTime: number;
  fastestSector1: number;
  fastestSector2: number;
  fastestSector3: number;
  fastestLapTime: number;
  lapsDriven: number;
  currentUsedTyre: number;
}

export interface SessionLiveViewApiData
{
  dbId: number;
  sessionGameId: number;
  isFinished: boolean;
  currentCarsOnTrack: number;
  sessionType: number;
  sessionDuration: number;
  sessionTimeLeft: number;
  airTemperature: number;
  trackTemperature: number;
  isSafetyCar: boolean;
  weather: number;
  fastestSector1: number;
  fastestSector1Driver: number;
  fastestSector2: number;
  fastestSector2Driver: number;
  fastestSector3: number;
  fastestSector3Driver: number;
  fastestLap: number;
  fastestLapDriver: number;
  drivers: Array<LiveDriverApiData>;
  timeTable: Array<number>;
}
