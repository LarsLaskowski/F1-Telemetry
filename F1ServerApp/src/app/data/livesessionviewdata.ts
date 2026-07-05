import { CalculateTimes } from "../utils/calculatetimes";
import { DriverViewData } from "./driverviewdata";
import { SessionLiveViewApiData } from "./livesessiondata_api";
import { SessionViewData } from "./sessionviewdata";

export class LiveSessionViewData extends SessionViewData
{
  sessionGameId: number = 0;
  sessionDbId: number = 0;
  isFinished: boolean = false;
  currentCarsOnTrack: number = 0;
  sessionDuration: string = "";
  sessionTimeLeft: string = "";
  airTemperature: number = 0;
  trackTemperature: number = 0;
  isSafetyCar: boolean = false;
  weatherCondition: string = "";
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
  drivers: Map<number, DriverViewData> = new Map();
  timeTable: Set<number> = new Set();

  setLiveSessionApiData(liveSessionApiData: SessionLiveViewApiData)
  {
    if (liveSessionApiData)
    {
      this.sessionGameId = liveSessionApiData.sessionGameId;
      this.sessionDbId = liveSessionApiData.dbId;
      this.isFinished = liveSessionApiData.isFinished;
      this.currentCarsOnTrack = liveSessionApiData.currentCarsOnTrack;
      this.sessionDuration = CalculateTimes.timeSecondsToString(liveSessionApiData.sessionDuration);
      this.sessionTimeLeft = CalculateTimes.timeSecondsToString(liveSessionApiData.sessionTimeLeft);
      this.airTemperature = liveSessionApiData.airTemperature;
      this.trackTemperature = liveSessionApiData.trackTemperature;
      this.isSafetyCar = liveSessionApiData.isSafetyCar;
      this.weatherCondition = this.matchWeatherCondition(liveSessionApiData.weather);
      this.fastestSector1 = CalculateTimes.matchTimeOutput(liveSessionApiData.fastestSector1);
      this.fastestSector2 = CalculateTimes.matchTimeOutput(liveSessionApiData.fastestSector2);
      this.fastestSector3 = CalculateTimes.matchTimeOutput(liveSessionApiData.fastestSector3);
      this.fastestLap = CalculateTimes.matchTimeOutput(liveSessionApiData.fastestLap);

      this.drivers.clear();

      if (liveSessionApiData.drivers && liveSessionApiData.drivers.length > 0)
      {
        this.setDrivers(liveSessionApiData);
      }

      this.timeTable.clear();

      this.setTimeTable(liveSessionApiData);

      this.fastestSector1Driver = "";
      this.fastestSector1DriverId = -1;
      this.fastestSector2Driver = "";
      this.fastestSector2DriverId = -1;
      this.fastestSector3Driver = "";
      this.fastestSector3DriverId = -1;
      this.fastestLapDriver = "";
      this.fastestLapDriverId = -1;

      this.setFastestSectorDriver(liveSessionApiData);

      this.setFastestLapDriver(liveSessionApiData);
    }
  }

  private setFastestLapDriver(liveSessionApiData: SessionLiveViewApiData)
  {
    if (liveSessionApiData.fastestLapDriver > 0)
    {
      let driverData = this.drivers.get(liveSessionApiData.fastestLapDriver);

      if (driverData)
      {
        this.fastestLapDriver = driverData.name;
        this.fastestLapDriverId = driverData.arrayIndex;
      }
    }
  }

  private setFastestSectorDriver(liveSessionApiData: SessionLiveViewApiData)
  {
    if (liveSessionApiData.fastestSector1Driver > 0)
    {
      let driverData = this.drivers.get(liveSessionApiData.fastestSector1Driver);

      if (driverData)
      {
        this.fastestSector1Driver = driverData.name;
        this.fastestSector1DriverId = driverData.arrayIndex;
      }
    }

    if (liveSessionApiData.fastestSector2Driver > 0)
    {
      let driverData = this.drivers.get(liveSessionApiData.fastestSector2Driver);

      if (driverData)
      {
        this.fastestSector2Driver = driverData.name;
        this.fastestSector2DriverId = driverData.arrayIndex;
      }
    }

    if (liveSessionApiData.fastestSector3Driver > 0)
    {
      let driverData = this.drivers.get(liveSessionApiData.fastestSector3Driver);

      if (driverData)
      {
        this.fastestSector3Driver = driverData.name;
        this.fastestSector3DriverId = driverData.arrayIndex;
      }
    }
  }

  private setTimeTable(liveSessionApiData: SessionLiveViewApiData)
  {
    if (liveSessionApiData.timeTable && liveSessionApiData.timeTable.length > 0)
    {
      liveSessionApiData.timeTable.forEach((driverNumber) =>
      {
        this.timeTable.add(driverNumber);
      });
    }

    if (this.timeTable.size < this.drivers.size)
    {
      this.drivers.forEach((value: DriverViewData, key: number) =>
      {
        if (this.timeTable.has(key) == false)
        {
          this.timeTable.add(key);
        }
      });
    }
  }

  private setDrivers(liveSessionApiData: SessionLiveViewApiData)
  {
    liveSessionApiData.drivers.forEach((driver) =>
    {
      if (this.drivers.has(driver.arrayIndex))
      {
        // Update data
        let driverData = this.drivers.get(driver.arrayIndex);

        driverData?.setDriverApiData(driver);
      }
      else
      {
        // New data
        let driverData = new DriverViewData(driver);

        this.drivers.set(driver.arrayIndex, driverData);
      }
    });
  }

  // Match weather condition
  private matchWeatherCondition(weather: number): string
  {
    let output = "";

    switch (weather)
    {
      case 0:
        output = "Clear";
        break;

      case 1:
        output = "Light clouds";
        break;

      case 2:
        output = "Overcast";
        break;

      case 3:
        output = "Light rain";
        break;

      case 4:
        output = "Heavy rain";
        break;

      case 5:
        output = "Storm";
        break;

      default:
        output = "Unknown";
        break;
    }

    return output;
  }
}
