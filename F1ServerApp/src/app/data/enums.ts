// TS enums mirrored from F1Server.Core.Enumerations, plus the display-string
// decoders that used to be duplicated (and drifting) across the viewdata
// models. Keep the numeric values in sync with the backend enums.

export enum Formula
{
  F1Modern = 0,
  F1Classic,
  F2,
  F1Generic,
  Beta,
  SuperCars,
  ESports,
  F2TwentyOne,
  F1World,
  F1Elimination,
  F12026 = 13
}

export enum SessionType
{
  Unknown = 0,
  Practice1,
  Practice2,
  Practice3,
  ShortPractice,
  Qualifying1,
  Qualifying2,
  Qualifying3,
  ShortQualifying,
  OneShotQualifying,
  Race,
  Race2,
  Race3,
  TimeTrial,
  SprintShootout1,
  SprintShootout2,
  SprintShootout3,
  ShortSprintShootout,
  OneShotSprintShootout,
  Sprint = 100
}

export enum WeatherCondition
{
  Clear = 0,
  LightClouds,
  Overcast,
  LightRain,
  HeavyRain,
  Storm
}

export enum DriverStatus
{
  Unknown = 0,
  InGarage,
  FlyingLap,
  InLap,
  OutLap,
  OnTrack
}

export enum VisualTyreCompound
{
  Unknown = 0,
  HyperSoft,
  SuperSoft,
  Soft,
  Medium,
  Hard,
  SuperHard,
  Inter,
  Wet
}

export enum FastestLapSessionType
{
  Practice = 0,
  Qualifying,
  Race
}

// Formula display name
export function matchFormulaType(formula: Formula): string
{
  switch (formula)
  {
    case Formula.F1Modern:
      return "F1";

    case Formula.F1Classic:
      return "F1 Classic";

    case Formula.F2:
      return "F2";

    case Formula.F1Generic:
      return "F1 Generic";

    case Formula.Beta:
      return "Beta";

    case Formula.SuperCars:
      return "Supercars";

    case Formula.ESports:
      return "E-Sports";

    case Formula.F2TwentyOne:
      return "F2 2021";

    case Formula.F1World:
      return "F1 World";

    case Formula.F1Elimination:
      return "F1 Elimination";

    case Formula.F12026:
      return "F1";

    default:
      return "Unknown";
  }
}

// Session type display name
export function matchSessionType(sessionType: SessionType): string
{
  switch (sessionType)
  {
    case SessionType.Unknown:
      return "Not set";

    case SessionType.Practice1:
      return "Practice session 1";

    case SessionType.Practice2:
      return "Practice session 2";

    case SessionType.Practice3:
      return "Practice session 3";

    case SessionType.ShortPractice:
      return "Short practice";

    case SessionType.Qualifying1:
      return "Qualifying session 1";

    case SessionType.Qualifying2:
      return "Qualifying session 2";

    case SessionType.Qualifying3:
      return "Qualifying session 3";

    case SessionType.ShortQualifying:
      return "Short qualifying";

    case SessionType.OneShotQualifying:
      return "One shot qualifying";

    case SessionType.Race:
    case SessionType.Race2:
    case SessionType.Race3:
      return "Race";

    case SessionType.TimeTrial:
      return "Time trial";

    case SessionType.SprintShootout1:
      return "Sprint shootout 1";

    case SessionType.SprintShootout2:
      return "Sprint shootout 2";

    case SessionType.SprintShootout3:
      return "Sprint shootout 3";

    case SessionType.ShortSprintShootout:
      return "Short sprint shootout";

    case SessionType.OneShotSprintShootout:
      return "One shot sprint shootout";

    case SessionType.Sprint:
      return "Sprint";

    default:
      return "Unknown";
  }
}

// Weather icon token, e.g. for material icon font names
export function matchWeatherIcon(weather: WeatherCondition): string
{
  switch (weather)
  {
    case WeatherCondition.Clear:
      return "sunny";

    case WeatherCondition.LightClouds:
      return "partly_cloudy_day";

    case WeatherCondition.Overcast:
      return "cloudy";

    case WeatherCondition.LightRain:
      return "rainy";

    case WeatherCondition.HeavyRain:
      return "rainy_heavy";

    case WeatherCondition.Storm:
      return "thunderstorm";

    default:
      return "sunny";
  }
}

// Weather display label
export function matchWeatherLabel(weather: WeatherCondition): string
{
  switch (weather)
  {
    case WeatherCondition.Clear:
      return "Clear";

    case WeatherCondition.LightClouds:
      return "Light clouds";

    case WeatherCondition.Overcast:
      return "Overcast";

    case WeatherCondition.LightRain:
      return "Light rain";

    case WeatherCondition.HeavyRain:
      return "Heavy rain";

    case WeatherCondition.Storm:
      return "Storm";

    default:
      return "Unknown";
  }
}

// Driver status display label
export function matchDriverStatus(driverStatus: DriverStatus): string
{
  switch (driverStatus)
  {
    case DriverStatus.Unknown:
      return "Unknown";

    case DriverStatus.InGarage:
      return "In garage";

    case DriverStatus.FlyingLap:
      return "Flying lap";

    case DriverStatus.InLap:
      return "In lap";

    case DriverStatus.OutLap:
      return "Out lap";

    case DriverStatus.OnTrack:
      return "On track";

    default:
      return "???";
  }
}

// Tyre compound image path
export function matchTyreImagePath(tyre: VisualTyreCompound): string
{
  switch (tyre)
  {
    case VisualTyreCompound.HyperSoft:
      return "assets/tyre_hypersoft.png";

    case VisualTyreCompound.SuperSoft:
      return "assets/tyre_supersoft.png";

    case VisualTyreCompound.Soft:
      return "assets/tyre_soft.png";

    case VisualTyreCompound.Medium:
      return "assets/tyre_medium.png";

    case VisualTyreCompound.Hard:
      return "assets/tyre_hard.png";

    case VisualTyreCompound.SuperHard:
      return "assets/tyre_superhard.png";

    case VisualTyreCompound.Inter:
      return "assets/tyre_inter.png";

    case VisualTyreCompound.Wet:
      return "assets/tyre_wet.png";

    default:
      return "";
  }
}

// Fastest lap session type display name
export function matchFastestLapSessionType(sessionType: FastestLapSessionType): string
{
  switch (sessionType)
  {
    case FastestLapSessionType.Practice:
      return "Practice";

    case FastestLapSessionType.Qualifying:
      return "Qualifying";

    case FastestLapSessionType.Race:
      return "Race";

    default:
      return "";
  }
}
