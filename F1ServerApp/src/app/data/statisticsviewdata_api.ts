import { SessionPacketMetricsViewApiData } from "./packetmetrics_api";

export interface StatisticsViewApiData
{
  currentSessionId: 0;
  currentSessionGameVersion: 0;
  lastSessionId: 0;
  lastSessionGameVersion: 0;
  packetsReceivedTotal: 0;
  packetsReceivedCurrentSession: 0;
  packetsReceivedLastSession: 0;
  packetsInQueue: 0;
  packetsInProcessorQueue: 0;
  totalPacketProcessingTime: 0;
  totalPacketsProcessed: 0;
  currentSessionMetrics: SessionPacketMetricsViewApiData;
  lastSessionMetrics: SessionPacketMetricsViewApiData;
  totalPacketLogTime: 0;
  totalPacketsLogged: 0;
}
