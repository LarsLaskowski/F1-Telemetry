using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.PacketToObject;

namespace F1Server.Core;

/// <summary>
/// Analyze new packets
/// </summary>
public class PacketAnalyzer
{
    #region Properties

    /// <summary>
    /// Last error of the most recent transformation, <see cref="string.Empty"/> when the transformation succeeded
    /// </summary>
    public string LastError { get; private set; } = string.Empty;

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Analyze the complete packet and return the correct data class
    /// </summary>
    /// <param name="receivedData">Complete received packet data</param>
    /// <returns>Object</returns>
    public object? GetPacketData(ReceivedPacketData receivedData)
    {
        object? packetData = null;

        LastError = string.Empty;

        if (receivedData.PacketHeader != null)
        {
            switch (receivedData.PacketHeader.PacketType)
            {
                case PacketTypes.Session:
                    {
                        packetData = GetSessionData(receivedData.PacketHeader, receivedData.PacketRawData);
                    }
                    break;

                case PacketTypes.LapData:
                    {
                        packetData = GetLapData(receivedData.PacketHeader, receivedData.PacketRawData);
                    }
                    break;

                case PacketTypes.Event:
                    {
                        packetData = GetEventData(receivedData.PacketHeader, receivedData.PacketRawData);
                    }
                    break;

                case PacketTypes.Participants:
                    {
                        packetData = GetParticipantsData(receivedData.PacketHeader, receivedData.PacketRawData);
                    }
                    break;

                case PacketTypes.CarTelemetry:
                    {
                        packetData = GetCarTelemetry(receivedData.PacketHeader, receivedData.PacketRawData);
                    }
                    break;

                case PacketTypes.CarStatus:
                    {
                        packetData = GetCarStatus(receivedData.PacketHeader, receivedData.PacketRawData);
                    }
                    break;

                case PacketTypes.CarTelemetry2:
                    {
                        packetData = GetCarTelemetry2(receivedData.PacketHeader, receivedData.PacketRawData);
                    }
                    break;

                case PacketTypes.CarDamage:
                case PacketTypes.Motion:
                case PacketTypes.LobbyInfo:
                case PacketTypes.CarSetups:
                case PacketTypes.TyreSets:
                case PacketTypes.MotionEx:
                    {
                        packetData = new UnknownData(receivedData.PacketHeader);
                    }
                    break;

                case PacketTypes.TimeTrial:
                    {
                        packetData = GetTimeTrialData(receivedData.PacketHeader, receivedData.PacketRawData);
                    }
                    break;

                case PacketTypes.LapPositions:
                    {
                        packetData = GetLapPositionsData(receivedData.PacketHeader, receivedData.PacketRawData);
                    }
                    break;

                case PacketTypes.SessionHistory:
                    {
                        packetData = GetSessionHistoryData(receivedData.PacketHeader, receivedData.PacketRawData);
                    }
                    break;

                case PacketTypes.FinalClassification:
                    {
                        packetData = GetFinalClassificationData(receivedData.PacketHeader, receivedData.PacketRawData);
                    }
                    break;
            }
        }

        return packetData;
    }

    /// <summary>
    /// Get session information
    /// </summary>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="dataPacket">Received data</param>
    /// <returns>Returns session data object, depending of game version</returns>
    public object? GetSessionData(PacketHeader packetHeader, ReadOnlySpan<byte> dataPacket)
    {
        object? sessionData = null;

        LastError = string.Empty;

        if (dataPacket.Length > 0 && packetHeader != null)
        {
            var sessionTransformation = new PacketToSessionData(packetHeader);

            sessionData = sessionTransformation.ExtractSessionDataPacket(dataPacket);

            UpdateLastError(sessionData, sessionTransformation);
        }

        return sessionData;
    }

    /// <summary>
    /// Get lap data
    /// </summary>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="dataPacket">Received packet data</param>
    /// <returns>Returns object from type LapData20xx, depending of game version</returns>
    public object? GetLapData(PacketHeader packetHeader, ReadOnlySpan<byte> dataPacket)
    {
        object? packetLapData = null;

        LastError = string.Empty;

        if (dataPacket.Length > 0 && packetHeader != null)
        {
            var lapDataTransformation = new PacketToLapData(packetHeader);

            packetLapData = lapDataTransformation.ExtractLapData(dataPacket);

            UpdateLastError(packetLapData, lapDataTransformation);
        }

        return packetLapData;
    }

    /// <summary>
    /// Get event data
    /// </summary>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="dataPacket">Received packet data</param>
    /// <returns>Returns event data object, depending on game version</returns>
    public object? GetEventData(PacketHeader packetHeader, ReadOnlySpan<byte> dataPacket)
    {
        object? eventData = null;

        LastError = string.Empty;

        if (dataPacket.Length > 0 && packetHeader != null)
        {
            var eventDataTransformation = new PacketToEventData(packetHeader);

            eventData = eventDataTransformation.ExtractEventData(dataPacket);

            UpdateLastError(eventData, eventDataTransformation);
        }

        return eventData;
    }

    /// <summary>
    /// Get participant data
    /// </summary>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="dataPacket">Received packet data</param>
    /// <returns>Returns participant data object, depending on game version</returns>
    public object? GetParticipantsData(PacketHeader packetHeader, ReadOnlySpan<byte> dataPacket)
    {
        object? participantsData = null;

        LastError = string.Empty;

        if (dataPacket.Length > 0 && packetHeader != null)
        {
            var participantsTransformation = new PacketToParticipants(packetHeader);

            participantsData = participantsTransformation.ExtractParticipantsDataPacket(dataPacket);

            UpdateLastError(participantsData, participantsTransformation);
        }

        return participantsData;
    }

    /// <summary>
    /// Get car telemetry
    /// </summary>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="dataPacket">Received packet data</param>
    /// <returns>Return car telemetry data object, depending on game version</returns>
    public object? GetCarTelemetry(PacketHeader packetHeader, ReadOnlySpan<byte> dataPacket)
    {
        object? carTelemetry = null;

        LastError = string.Empty;

        if (dataPacket.Length > 0 && packetHeader != null)
        {
            var carTelemetryTransformation = new PacketToCarTelemetry(packetHeader);

            carTelemetry = carTelemetryTransformation.ExtractCarTelemetryData(dataPacket);

            UpdateLastError(carTelemetry, carTelemetryTransformation);
        }

        return carTelemetry;
    }

    /// <summary>
    /// Get car status
    /// </summary>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="dataPacket">Received packet data</param>
    /// <returns>Return car telemetry data object, depending on game version</returns>
    public object? GetCarStatus(PacketHeader packetHeader, ReadOnlySpan<byte> dataPacket)
    {
        object? carStatus = null;

        LastError = string.Empty;

        if (dataPacket.Length > 0 && packetHeader != null)
        {
            var carStatusTransformation = new PacketToCarStatus(packetHeader);

            carStatus = carStatusTransformation.ExtractCarStatusData(dataPacket);

            UpdateLastError(carStatus, carStatusTransformation);
        }

        return carStatus;
    }

    /// <summary>
    /// Get additional car telemetry (CarTelemetry2) data
    /// </summary>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="dataPacket">Received packet data</param>
    /// <returns>Return additional car telemetry data object, depending on game version</returns>
    public object? GetCarTelemetry2(PacketHeader packetHeader, ReadOnlySpan<byte> dataPacket)
    {
        object? carTelemetry2 = null;

        LastError = string.Empty;

        if (dataPacket.Length > 0 && packetHeader != null)
        {
            var carTelemetry2Transformation = new PacketToCarTelemetry2(packetHeader);

            carTelemetry2 = carTelemetry2Transformation.ExtractCarTelemetry2Data(dataPacket);

            UpdateLastError(carTelemetry2, carTelemetry2Transformation);
        }

        return carTelemetry2;
    }

    /// <summary>
    /// Get time trial data sets
    /// </summary>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="dataPacket">Received data packet</param>
    /// <returns>Return time trial data</returns>
    public object? GetTimeTrialData(PacketHeader packetHeader, ReadOnlySpan<byte> dataPacket)
    {
        object? timeTrialData = null;

        LastError = string.Empty;

        if (dataPacket.Length > 0 && packetHeader != null)
        {
            var timeTrialTransformation = new PacketToTimeTrialData(packetHeader);

            timeTrialData = timeTrialTransformation.ExtractTimeTrialDataPacket(dataPacket);

            UpdateLastError(timeTrialData, timeTrialTransformation);
        }

        return timeTrialData;
    }

    /// <summary>
    /// Get lap positions data
    /// </summary>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="dataPacket">Received data packet</param>
    /// <returns>Return lap positions data</returns>
    public object? GetLapPositionsData(PacketHeader packetHeader, ReadOnlySpan<byte> dataPacket)
    {
        object? lapPositionsData = null;

        LastError = string.Empty;

        if (dataPacket.Length > 0 && packetHeader != null)
        {
            var lapPositionsTransformation = new PacketToLapPositions(packetHeader);

            lapPositionsData = lapPositionsTransformation.ExtractLapPositionsPacket(dataPacket);

            UpdateLastError(lapPositionsData, lapPositionsTransformation);
        }

        return lapPositionsData;
    }

    /// <summary>
    /// Get session history
    /// </summary>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="dataPacket">Received data packet</param>
    /// <returns>Return session history data</returns>
    public object? GetSessionHistoryData(PacketHeader packetHeader, ReadOnlySpan<byte> dataPacket)
    {
        object? sessionHistoryData = null;

        LastError = string.Empty;

        if (dataPacket.Length > 0 && packetHeader != null)
        {
            var sessionHistoryTransformation = new PacketToSessionHistoryData(packetHeader);

            sessionHistoryData = sessionHistoryTransformation.ExtractSessionHistoryDataPacket(dataPacket);

            UpdateLastError(sessionHistoryData, sessionHistoryTransformation);
        }

        return sessionHistoryData;
    }

    /// <summary>
    /// Get final classification
    /// </summary>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="dataPacket">Received data packet</param>
    /// <returns>Return final classification data</returns>
    public object? GetFinalClassificationData(PacketHeader packetHeader, ReadOnlySpan<byte> dataPacket)
    {
        object? finalClassificationData = null;

        LastError = string.Empty;

        if (dataPacket.Length > 0 && packetHeader != null)
        {
            var finalClassificationTransformation = new PacketToFinalClassification(packetHeader);

            finalClassificationData = finalClassificationTransformation.ExtractFinalClassificationData(dataPacket);

            UpdateLastError(finalClassificationData, finalClassificationTransformation);
        }

        return finalClassificationData;
    }

    #endregion // Methods

    #region Private methods

    /// <summary>
    /// Takes over the error of a transformation when the transformation returned no data object,
    /// so that the reason for a rejected packet is not lost
    /// </summary>
    /// <param name="packetData">Data object returned by the transformation</param>
    /// <param name="transformation">Transformation that converted the packet</param>
    private void UpdateLastError(object? packetData, PacketToXBase transformation)
    {
        if (packetData is null && string.IsNullOrWhiteSpace(transformation.LastError) == false)
        {
            LastError = transformation.LastError;
        }
    }

    #endregion // Private methods
}