import { FastestLapSessionViewApiData } from "./fastestlapsessionviewdata_api";
import { SessionViewData } from "./sessionviewdata";

export class SessionViewDataEx extends SessionViewData
{
  fastestSector1: string = "";
  fastestSector1Driver: string = "";
  fastestSector1DriverId: number = -1;
  fastestSector1DriverIsHuman: boolean = false;
  fastestSector2: string = "";
  fastestSector2Driver: string = "";
  fastestSector2DriverId: number = -1;
  fastestSector2DriverIsHuman: boolean = false;
  fastestSector3: string = "";
  fastestSector3Driver: string = "";
  fastestSector3DriverId: number = -1;
  fastestSector3DriverIsHuman: boolean = false;
  fastestLap: string = "";
  fastestLapDriver: string = "";
  fastestLapDriverId: number = -1;
  fastestLapDriverIsHuman: boolean = false;
  fastestLapSector1: string = "";
  fastestLapSector2: string = "";
  fastestLapSector3: string = "";
  isFastestLapSector1: boolean = false;
  isFastestLapSector2: boolean = false;
  isFastestLapSector3: boolean = false;
  theoreticalFastestLap: string = "";
  isLoadingTimes: boolean = true;
  isNonHumanFastestSector: boolean = false;
  humanPlayersFastestLap: string = "";

  loadFastestTimes(onUpdate?: () => void)
  {
    if (this.httpClient != null)
    {
      this.httpClient.get<FastestLapSessionViewApiData>(this.baseUrl + 'api/FastestLap/FastestLapDataOfSession/' + this.sessionId).subscribe(
      {
        next: (result) =>
        {
          this.fastestLap = result.fastestLap;
          this.fastestLapDriver = result.fastestLapDriver;
          this.fastestLapDriverId = result.fastestLapDriverId;
          this.fastestLapDriverIsHuman = result.isFastestLapDriverHuman;
          this.fastestLapSector1 = result.fastestLapSector1;
          this.fastestLapSector2 = result.fastestLapSector2;
          this.fastestLapSector3 = result.fastestLapSector3;
          this.isFastestLapSector1 = result.isFastestLapSector1;
          this.isFastestLapSector2 = result.isFastestLapSector2;
          this.isFastestLapSector3 = result.isFastestLapSector3;
          this.fastestSector1 = result.fastestSector1;
          this.fastestSector1Driver = result.fastestSector1Driver;
          this.fastestSector1DriverId = result.fastestSector1DriverId;
          this.fastestSector1DriverIsHuman = result.isFastestSector1DriverHuman;
          this.fastestSector2 = result.fastestSector2;
          this.fastestSector2Driver = result.fastestSector2Driver;
          this.fastestSector2DriverId = result.fastestSector2DriverId;
          this.fastestSector2DriverIsHuman = result.isFastestSector2DriverHuman;
          this.fastestSector3 = result.fastestSector3;
          this.fastestSector3Driver = result.fastestSector3Driver;
          this.fastestSector3DriverId = result.fastestSector3DriverId;
          this.fastestSector3DriverIsHuman = result.isFastestSector3DriverHuman;
          this.theoreticalFastestLap = result.theoreticalFastestLap;
          this.humanPlayersFastestLap = result.humanPlayersFastestLap;

          this.isNonHumanFastestSector = this.fastestSector1DriverIsHuman == false || this.fastestSector2DriverIsHuman == false || this.fastestSector3DriverIsHuman == false;

          onUpdate?.();
        },
        error: (err) => { console.error(err); },
        complete: () =>
        {
          this.isLoadingTimes = false;

          onUpdate?.();
        }
      });
    }
  }

  public getFastestLapSectorTimeClass(sectorNum: number): string
  {
    let classType = 'fastestSector-time';

    if ((sectorNum == 1 && this.isFastestLapSector1)
      || (sectorNum == 2 && this.isFastestLapSector2)
      || (sectorNum == 3 && this.isFastestLapSector3))
    {
      classType = 'fastestSector-time-fastest';
    }

    return classType;
  }

  public getOverallFastestSectorTimeClass(sectorNum: number): string
  {
    let classType = 'fastestSector-time';

    if ((sectorNum == 1 && this.isFastestLapSector1 == false)
      || (sectorNum == 2 && this.isFastestLapSector2 == false)
      || (sectorNum == 3 && this.isFastestLapSector3 == false))
    {
      classType = 'fastestSector-time-fastest';
    }
    else if ((sectorNum == 1 && this.isFastestLapSector1)
      || (sectorNum == 2 && this.isFastestLapSector2)
      || (sectorNum == 3 && this.isFastestLapSector3))
    {
      classType = 'fastestSector-time-equal';
    }

    return classType;
  }

  public getFastestSectorDriverClass(sectorNum: number): string
  {
    let classType = 'fastestSector-data';

    if ((sectorNum == 1 && this.fastestSector1DriverIsHuman == false)
      || (sectorNum == 2 && this.fastestSector2DriverIsHuman == false)
      || (sectorNum == 3 && this.fastestSector3DriverIsHuman == false))
    {
      classType = 'fastestSector-data-ai';
    }

    return classType;
  }
}
