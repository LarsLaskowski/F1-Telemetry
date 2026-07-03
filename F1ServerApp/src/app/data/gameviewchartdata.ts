import { GameViewApiData } from "./gameviewdata_api";

export class GameViewChartData
{
  gameId: number = 0;
  gameVersion: string = "";
  gameSessions: number = 0;

  setGameData(gameData: GameViewApiData)
  {
    if (gameData)
    {
      this.gameId = gameData.id;
      this.gameVersion = gameData.gameVersion;
      this.gameSessions = gameData.sessions;
    }
  }
}
