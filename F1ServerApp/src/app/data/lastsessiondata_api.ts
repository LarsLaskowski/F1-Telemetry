export interface LastDriverApiData
{
  arrayIndex: number;
  driverName: string;
  carNumber: number;
  gridPosition: number;
  carPosition: number;
  nationality: string;
  teamName: string;
  fastestSector1: number;
  fastestSector2: number;
  fastestSector3: number;
  fastestLapTime: number;
}

export interface LastSessionViewApiData
{
  dbId: number;
  sessionGameId: number;
  sessionType: number;
  fastestSector1: number;
  fastestSector1Driver: number;
  fastestSector2: number;
  fastestSector2Driver: number;
  fastestSector3: number;
  fastestSector3Driver: number;
  fastestLap: number;
  fastestLapDriver: number;
  drivers: Array<LastDriverApiData>;
  timeTable: Array<number>;
}
