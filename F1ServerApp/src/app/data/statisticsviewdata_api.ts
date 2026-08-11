import { SessionPacketMetricsViewApiData } from "./packetmetrics_api";

export interface StatisticsViewApiData
{
  currentSessionId: number;
  currentSessionGameVersion: number;
  lastSessionId: number;
  lastSessionGameVersion: number;
  packetsReceivedTotal: number;
  packetsReceivedCurrentSession: number;
  packetsReceivedLastSession: number;
  packetsInQueue: number;
  packetsInProcessorQueue: number;
  totalPacketProcessingTime: number;
  totalPacketsProcessed: number;
  currentSessionMetrics: SessionPacketMetricsViewApiData;
  lastSessionMetrics: SessionPacketMetricsViewApiData;
  totalPacketLogTime: number;
  totalPacketsLogged: number;
}
