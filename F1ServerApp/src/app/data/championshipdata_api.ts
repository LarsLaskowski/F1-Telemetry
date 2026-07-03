import { ChampionshipTrackViewApiData } from "./championshiptrackdata_api";

export interface ChampionshipViewApiData
{
  championshipId: number;
  gameVersionId: number;
  gameVersionName: string;
  gameVersionYear: number;
  number: number;
  tracks: ChampionshipTrackViewApiData[];
}
