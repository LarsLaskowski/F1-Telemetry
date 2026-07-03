import { FinalClassificationViewApiData } from "./finalclassificationviewdata_api";
import { SessionViewData } from "./sessionviewdata";

export class LastSessionViewData extends SessionViewData
{
  sessionGameId: number = 0;
  fastestSector1: string = "";
  fastestSector2: string = "";
  fastestSector3: string = "";
  fastestSector1Driver: string = "";
  fastestSector1DriverId: number = -1;
  fastestSector2Driver: string = "";
  fastestSector2DriverId: number = -1;
  fastestSector3Driver: string = "";
  fastestSector3DriverId: number = -1;
  fastestLap: string = "";
  fastestLapDriver: string = "";
  fastestLapDriverId: number = -1;
  public timeTable: Map<number, FinalClassificationViewApiData> = new Map();
  public leader: FinalClassificationViewApiData | undefined;

  setFinalClassificationApiData(finalClassifications: FinalClassificationViewApiData[])
  {
    if (finalClassifications != null && finalClassifications.length > 0)
    {
      this.timeTable.clear();

      finalClassifications.forEach((entry) =>
      {
        if (entry.finishPosition == 1)
        {
          this.leader = entry;
        }

        this.timeTable.set(entry.finishPosition, entry);
      });
    }
  }
}
