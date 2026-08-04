using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.PacketToObject;

namespace F1Server.Core;

/// <summary>
/// Analyze new packets. The analyzer keeps one reusable transformation instance per packet type to avoid
/// an allocation per received packet, so an instance is stateful and must only be used from a single
/// thread at a time. Every concurrent processing path needs its own <see cref="PacketAnalyzer"/> instance
/// </summary>
public class PacketAnalyzer
{
    #region Fields

    /// <summary>
    /// Reusable transformation for session packets
    /// </summary>
    private readonly PacketToSessionData _sessionTransformation = new();

    /// <summary>
    /// Reusable transformation for lap data packets
    /// </summary>
    private readonly PacketToLapData _lapDataTransformation = new();

    /// <summary>
    /// Reusable transformation for event packets
    /// </summary>
    private readonly PacketToEventData _eventDataTransformation = new();

    /// <summary>
    /// Reusable transformation for participants packets
    /// </summary>
    private readonly PacketToParticipants _participantsTransformation = new();

    /// <summary>
    /// Reusable transformation for car telemetry packets
    /// </summary>
    private readonly PacketToCarTelemetry _carTelemetryTransformation = new();

    /// <summary>
    /// Reusable transformation for car status packets
    /// </summary>
    private readonly PacketToCarStatus _carStatusTransformation = new();

    /// <summary>
    /// Reusable transformation for additional car telemetry packets
    /// </summary>
    private readonly PacketToCarTelemetry2 _carTelemetry2Transformation = new();

    /// <summary>
    /// Reusable transformation for time trial packets
    /// </summary>
    private readonly PacketToTimeTrialData _timeTrialTransformation = new();

    /// <summary>
    /// Reusable transformation for lap positions packets
    /// </summary>
    private readonly PacketToLapPositions _lapPositionsTransformation = new();

    /// <summary>
    /// Reusable transformation for session history packets
    /// </summary>
    private readonly PacketToSessionHistoryData _sessionHistoryTransformation = new();

    /// <summary>
    /// Reusable transformation for final classification packets
    /// </summary>
    private readonly PacketToFinalClassification _finalClassificationTransformation = new();

    #endregion // Fields

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
            sessionData = _sessionTransformation.ExtractSessionDataPacket(packetHeader, dataPacket);

            UpdateLastError(sessionData, _sessionTransformation);
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
            packetLapData = _lapDataTransformation.ExtractLapData(packetHeader, dataPacket);

            UpdateLastError(packetLapData, _lapDataTransformation);
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
            eventData = _eventDataTransformation.ExtractEventData(packetHeader, dataPacket);

            UpdateLastError(eventData, _eventDataTransformation);
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
            participantsData = _participantsTransformation.ExtractParticipantsDataPacket(packetHeader, dataPacket);

            UpdateLastError(participantsData, _participantsTransformation);
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
            carTelemetry = _carTelemetryTransformation.ExtractCarTelemetryData(packetHeader, dataPacket);

            UpdateLastError(carTelemetry, _carTelemetryTransformation);
        }

        return carTelemetry;
    }

    /// <summary>
    /// Get car status
    /// </summary>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="dataPacket">Received packet data</param>
    /// <returns>Return car status data object, depending on game version</returns>
    public object? GetCarStatus(PacketHeader packetHeader, ReadOnlySpan<byte> dataPacket)
    {
        object? carStatus = null;

        LastError = string.Empty;

        if (dataPacket.Length > 0 && packetHeader != null)
        {
            carStatus = _carStatusTransformation.ExtractCarStatusData(packetHeader, dataPacket);

            UpdateLastError(carStatus, _carStatusTransformation);
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
            carTelemetry2 = _carTelemetry2Transformation.ExtractCarTelemetry2Data(packetHeader, dataPacket);

            UpdateLastError(carTelemetry2, _carTelemetry2Transformation);
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
            timeTrialData = _timeTrialTransformation.ExtractTimeTrialDataPacket(packetHeader, dataPacket);

            UpdateLastError(timeTrialData, _timeTrialTransformation);
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
            lapPositionsData = _lapPositionsTransformation.ExtractLapPositionsPacket(packetHeader, dataPacket);

            UpdateLastError(lapPositionsData, _lapPositionsTransformation);
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
            sessionHistoryData = _sessionHistoryTransformation.ExtractSessionHistoryDataPacket(packetHeader, dataPacket);

            UpdateLastError(sessionHistoryData, _sessionHistoryTransformation);
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
            finalClassificationData = _finalClassificationTransformation.ExtractFinalClassificationData(packetHeader, dataPacket);

            UpdateLastError(finalClassificationData, _finalClassificationTransformation);
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