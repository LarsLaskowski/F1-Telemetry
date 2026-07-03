export interface FinalClassificationViewApiData
{
  finalDbId: number;
  participantDbId: number;
  arrayIndex: number;
  driverName: string;
  carNumber: number;
  teamName: string;
  nationality: string;
  startingPosition: number;
  finishPosition: number;
  lapsDriven: number;
  pitStops: number,
  fastestLapTime: string;
  fastestLapTimeDifference: string;
  isFastestSessionLapTime: boolean;
  totalRaceTime: string;
  raceTimeDifference: string;
  penaltiesTime: number;
  penalties: number;
}
