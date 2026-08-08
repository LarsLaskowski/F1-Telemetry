import { LiveDriverApiData } from "./livesessiondata_api";

export class DriverViewData
{
  arrayIndex: number = 0;
  position: number = 0;
  name: string = "";
  carNumber: number = 0;
  gridPosition: number = 0;
  carPosition: number = 0;
  currentLapTime: string = "";
  nationality: string = "";
  teamName: string = "";
  currentDriverStatus: string = "";
  fastestSector1: string = "";
  fastestSector2: string = "";
  fastestSector3: string = "";
  fastestLaptime: string = "";
  lapsDriven: number = 0;
  currentTyre: string = "";
  sector1Class: string = "normalTime";
  sector2Class: string = "normalTime";
  sector3Class: string = "normalTime";
  fastestLapClass: string = "normalTime";

  // Constructor
  constructor(driverApiData: LiveDriverApiData)
  {
    this.setDriverApiData(driverApiData);
  }

  // Method to set data
  setDriverApiData(driverApiData: LiveDriverApiData)
  {
    if (driverApiData)
    {
      this.arrayIndex = driverApiData.arrayIndex;
      this.name = driverApiData.driverName;
      this.carNumber = driverApiData.carNumber;
      this.nationality = driverApiData.nationality;
      this.teamName = driverApiData.teamName;
      this.carPosition = driverApiData.carPosition;
      this.gridPosition = driverApiData.gridPosition;
      this.currentDriverStatus = this.matchDriverStatus(driverApiData.currentDriverStatus);
      this.fastestSector1 = this.matchTimeOutput(driverApiData.fastestSector1);
      this.fastestSector2 = this.matchTimeOutput(driverApiData.fastestSector2);
      this.fastestSector3 = this.matchTimeOutput(driverApiData.fastestSector3);
      this.fastestLaptime = this.matchTimeOutput(driverApiData.fastestLapTime);
      this.lapsDriven = driverApiData.lapsDriven;
      this.currentTyre = this.matchCurrentTyre(driverApiData.currentUsedTyre);

      if (driverApiData.currentDriverStatus > 1 && driverApiData.currentLapTime > 0)
      {
        this.currentLapTime = this.matchTimeOutput(driverApiData.currentLapTime);
      }
      else
      {
        this.currentLapTime = "";
      }
    }    
  }

  // Postion of driver in current time table
  setPosition(position: number)
  {
    this.position = position;
  }

  // Precompute the fastest-time highlight classes for the current session state
  setTimeClasses(fastestSector1DriverId: number, fastestSector2DriverId: number, fastestSector3DriverId: number, fastestLapDriverId: number)
  {
    this.sector1Class = this.arrayIndex == fastestSector1DriverId ? "fastestTime" : "normalTime";
    this.sector2Class = this.arrayIndex == fastestSector2DriverId ? "fastestTime" : "normalTime";
    this.sector3Class = this.arrayIndex == fastestSector3DriverId ? "fastestTime" : "normalTime";
    this.fastestLapClass = this.arrayIndex == fastestLapDriverId ? "fastestTime" : "normalTime";
  }

  // Generate time output
  private matchTimeOutput(timeInMilliseconds: number): string
  {
    let output = "";

    if (timeInMilliseconds >= 60000)
    {
      let minutes = Math.floor(timeInMilliseconds / 1000 / 60);

      output += this.padLeadingZeros(minutes, 2) + ":";

      timeInMilliseconds %= 60000;
    }

    let seconds = Math.floor(timeInMilliseconds / 1000);

    output += this.padLeadingZeros(seconds, 2) + ".";

    let milliseconds = timeInMilliseconds % 1000;

    output += this.padLeadingZeros(milliseconds, 3);

    return output;
  }

  padLeadingZeros(num: number, size: number)
  {
    let s = num + "";
    while (s.length < size) s = "0" + s;
    return s;
  }

  // Match driver status
  private matchDriverStatus(driverStatusNum: number): string
  {
    let output = "";

    switch (driverStatusNum) {
      case 0:
        output = "Unknown";
        break;

      case 1:
        output = "In garage";
        break;

      case 2:
        output = "Flying lap";
        break;

      case 3:
        output = "In lap";
        break;

      case 4:
        output = "Out lap";
        break;

      case 5:
        output = "On track";
        break;

      default:
        output = "???";
        break;
    }

    return output;
  }

  // Match current tyre to picture
  private matchCurrentTyre(currentTyre: number): string
  {
    let output = "";

    switch (currentTyre)
    {
      case 1:
        output = "assets/tyre_hypersoft.png"
        break;

      case 2:
        output = "assets/tyre_supersoft.png"
        break;

      case 3:
        output = "assets/tyre_soft.png"
        break;

      case 4:
        output = "assets/tyre_medium.png"
        break;

      case 5:
        output = "assets/tyre_hard.png"
        break;

      case 6:
        output = "assets/tyre_superhard.png"
        break;

      case 7:
        output = "assets/tyre_inter.png"
        break;

      case 8:
        output = "assets/tyre_wet.png"
        break;
    }

    return output;
  }
}
