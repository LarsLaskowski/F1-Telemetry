import { ParticipantViewApiData } from "./participantdata_api";

export class ParticipantViewData
{
  // Fields
  participantDbId: number = 0;
  driverName: string = "";
  teamName: string = "";
  driverNationality: string = "";
  isHumanControlled: boolean = false;
  isMyTeam: boolean = false;
  carRaceNumber: number = 0;

  // Methods
  setParticipantApiData(participantData: ParticipantViewApiData)
  {
    if (participantData) {
      this.participantDbId = participantData.participantDbId;
      this.driverName = participantData.driverName;
      this.teamName = participantData.teamName;
      this.driverNationality = participantData.driverNationality;
      this.isHumanControlled = participantData.isHumanControlled;
      this.isMyTeam = participantData.isMyTeam;
      this.carRaceNumber = participantData.carRaceNumber;
    }
  }
}
