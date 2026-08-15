export interface PacketMetricsViewApiData
{
  received: number;
  totalProcessingTime: number;
  avgProcessingTime: number;
}

export interface SessionPacketMetricsViewApiData
{
  motion: PacketMetricsViewApiData;
  session: PacketMetricsViewApiData;
  lapData: PacketMetricsViewApiData;
  event: PacketMetricsViewApiData;
  participants: PacketMetricsViewApiData;
  carSetups: PacketMetricsViewApiData;
  carTelemetry: PacketMetricsViewApiData;
  carStatus: PacketMetricsViewApiData;
  finalClassification: PacketMetricsViewApiData;
  lobbyInfo: PacketMetricsViewApiData;
  carDamage: PacketMetricsViewApiData;
  sessionHistory: PacketMetricsViewApiData;
  tyreSets: PacketMetricsViewApiData;
  motionEx: PacketMetricsViewApiData;
  timeTrial: PacketMetricsViewApiData;
  lapPositions: PacketMetricsViewApiData;
  carTelemetry2: PacketMetricsViewApiData;
  totalPacketsReceived: number;
  unsuccessfullyProcessed: number;
  errors: number;
}
