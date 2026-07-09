export interface PacketMetricsViewApiData
{
  received: 0;
  totalProcessingTime: 0;
  avgProcessingTime: 0;
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
  unsuccessfullyProcessed: 0;
  errors: 0;
}
