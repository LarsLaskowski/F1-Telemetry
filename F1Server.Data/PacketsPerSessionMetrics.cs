using F1Server.Core.Enumerations;

namespace F1Server.Data;

/// <summary>
/// Metrics for received packets in a session
/// </summary>
public class PacketsPerSessionMetrics
{
    #region Properties

    /// <summary>
    /// Total packets received
    /// </summary>
    public ulong TotalPacketsReceived => Motion.Received + Session.Received + LapData.Received + Event.Received + Participants.Received + CarSetups.Received + CarTelemetry.Received + CarStatus.Received + FinalClassification.Received + LobbyInfo.Received + CarDamage.Received + SessionHistory.Received + TyreSets.Received + MotionEx.Received + TimeTrial.Received + CarTelemetry2.Received + LapPositions.Received;

    /// <summary>
    /// Motion packets
    /// </summary>
    public PacketMetrics Motion { get; private set; } = new();

    /// <summary>
    /// Session packets
    /// </summary>
    public PacketMetrics Session { get; private set; } = new();

    /// <summary>
    /// Lap data packets
    /// </summary>
    public PacketMetrics LapData { get; private set; } = new();

    /// <summary>
    /// Event packets
    /// </summary>
    public PacketMetrics Event { get; private set; } = new();

    /// <summary>
    /// Participants packet
    /// </summary>
    public PacketMetrics Participants { get; private set; } = new();

    /// <summary>
    /// Car setups packet
    /// </summary>
    public PacketMetrics CarSetups { get; private set; } = new();

    /// <summary>
    /// Car telemetry packets
    /// </summary>
    public PacketMetrics CarTelemetry { get; private set; } = new();

    /// <summary>
    /// Car status packets
    /// </summary>
    public PacketMetrics CarStatus { get; private set; } = new();

    /// <summary>
    /// Final classification packets
    /// </summary>
    public PacketMetrics FinalClassification { get; private set; } = new();

    /// <summary>
    /// Lobby info packets
    /// </summary>
    public PacketMetrics LobbyInfo { get; private set; } = new();

    /// <summary>
    /// Car damage packets
    /// </summary>
    public PacketMetrics CarDamage { get; private set; } = new();

    /// <summary>
    /// Session history packets
    /// </summary>
    public PacketMetrics SessionHistory { get; private set; } = new();

    /// <summary>
    /// Tyre sets packets
    /// </summary>
    public PacketMetrics TyreSets { get; private set; } = new();

    /// <summary>
    /// MotionEx packets
    /// </summary>
    public PacketMetrics MotionEx { get; private set; } = new();

    /// <summary>
    /// Time trial packets
    /// </summary>
    public PacketMetrics TimeTrial { get; private set; } = new();

    /// <summary>
    /// Lap positions packets
    /// </summary>
    public PacketMetrics LapPositions { get; private set; } = new();

    /// <summary>
    /// Car telemetry 2 packets
    /// </summary>
    public PacketMetrics CarTelemetry2 { get; private set; } = new();

    /// <summary>
    /// Packets not processed successfully
    /// </summary>
    public long UnsuccessfullyProcessed { get; set; }

    /// <summary>
    /// Packets with errors
    /// </summary>
    public long Errors { get; set; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Copy content from source object
    /// </summary>
    /// <param name="sourceMetrics">Source</param>
    public void CopyFrom(PacketsPerSessionMetrics sourceMetrics)
    {
        Motion = new PacketMetrics(sourceMetrics.Motion);
        Session = new PacketMetrics(sourceMetrics.Session);
        LapData = new PacketMetrics(sourceMetrics.LapData);
        Event = new PacketMetrics(sourceMetrics.Event);
        Participants = new PacketMetrics(sourceMetrics.Participants);
        CarSetups = new PacketMetrics(sourceMetrics.CarSetups);
        CarTelemetry = new PacketMetrics(sourceMetrics.CarTelemetry);
        CarStatus = new PacketMetrics(sourceMetrics.CarStatus);
        FinalClassification = new PacketMetrics(sourceMetrics.FinalClassification);
        LobbyInfo = new PacketMetrics(sourceMetrics.LobbyInfo);
        CarDamage = new PacketMetrics(sourceMetrics.CarDamage);
        SessionHistory = new PacketMetrics(sourceMetrics.SessionHistory);
        TyreSets = new PacketMetrics(sourceMetrics.TyreSets);
        MotionEx = new PacketMetrics(sourceMetrics.MotionEx);
        TimeTrial = new PacketMetrics(sourceMetrics.TimeTrial);
        LapPositions = new PacketMetrics(sourceMetrics.LapPositions);
        CarTelemetry2 = new PacketMetrics(sourceMetrics.CarTelemetry2);

        UnsuccessfullyProcessed = sourceMetrics.UnsuccessfullyProcessed;
        Errors = sourceMetrics.Errors;
    }

    /// <summary>
    /// Reset content
    /// </summary>
    public void Reset()
    {
        Motion.Reset();
        Session.Reset();
        LapData.Reset();
        Event.Reset();
        Participants.Reset();
        CarSetups.Reset();
        CarTelemetry.Reset();
        CarStatus.Reset();
        FinalClassification.Reset();
        LobbyInfo.Reset();
        CarDamage.Reset();
        SessionHistory.Reset();
        TyreSets.Reset();
        MotionEx.Reset();
        TimeTrial.Reset();
        LapPositions.Reset();
        CarTelemetry2.Reset();

        UnsuccessfullyProcessed = 0;
        Errors = 0;
    }

    /// <summary>
    /// Update packet statistics
    /// </summary>
    /// <param name="packetType">Type of packet</param>
    /// <param name="elapsedMilliseconds">Elapsed processing time</param>
    public void UpdatePacketStatistics(PacketTypes? packetType, double elapsedMilliseconds)
    {
        if (packetType != null)
        {
            switch (packetType)
            {
                case PacketTypes.Motion:
                    {
                        Motion.Received++;
                        Motion.TotalProcessingTime += elapsedMilliseconds;
                    }
                    break;

                case PacketTypes.Session:
                    {
                        Session.Received++;
                        Session.TotalProcessingTime += elapsedMilliseconds;
                    }
                    break;

                case PacketTypes.LapData:
                    {
                        LapData.Received++;
                        LapData.TotalProcessingTime += elapsedMilliseconds;
                    }
                    break;

                case PacketTypes.Event:
                    {
                        Event.Received++;
                        Event.TotalProcessingTime += elapsedMilliseconds;
                    }
                    break;

                case PacketTypes.Participants:
                    {
                        Participants.Received++;
                        Participants.TotalProcessingTime += elapsedMilliseconds;
                    }
                    break;

                case PacketTypes.CarSetups:
                    {
                        CarSetups.Received++;
                        CarSetups.TotalProcessingTime += elapsedMilliseconds;
                    }
                    break;

                case PacketTypes.CarTelemetry:
                    {
                        CarTelemetry.Received++;
                        CarTelemetry.TotalProcessingTime += elapsedMilliseconds;
                    }
                    break;

                case PacketTypes.CarStatus:
                    {
                        CarStatus.Received++;
                        CarStatus.TotalProcessingTime += elapsedMilliseconds;
                    }
                    break;

                case PacketTypes.FinalClassification:
                    {
                        FinalClassification.Received++;
                        FinalClassification.TotalProcessingTime += elapsedMilliseconds;
                    }
                    break;

                case PacketTypes.LobbyInfo:
                    {
                        LobbyInfo.Received++;
                        LobbyInfo.TotalProcessingTime += elapsedMilliseconds;
                    }
                    break;

                case PacketTypes.CarDamage:
                    {
                        CarDamage.Received++;
                        CarDamage.TotalProcessingTime += elapsedMilliseconds;
                    }
                    break;

                case PacketTypes.SessionHistory:
                    {
                        SessionHistory.Received++;
                        SessionHistory.TotalProcessingTime += elapsedMilliseconds;
                    }
                    break;

                case PacketTypes.TyreSets:
                    {
                        TyreSets.Received++;
                        TyreSets.TotalProcessingTime += elapsedMilliseconds;
                    }
                    break;

                case PacketTypes.MotionEx:
                    {
                        MotionEx.Received++;
                        MotionEx.TotalProcessingTime += elapsedMilliseconds;
                    }
                    break;

                case PacketTypes.TimeTrial:
                    {
                        TimeTrial.Received++;
                        TimeTrial.TotalProcessingTime += elapsedMilliseconds;
                    }
                    break;

                case PacketTypes.LapPositions:
                    {
                        LapPositions.Received++;
                        LapPositions.TotalProcessingTime += elapsedMilliseconds;
                    }
                    break;

                case PacketTypes.CarTelemetry2:
                    {
                        CarTelemetry2.Received++;
                        CarTelemetry2.TotalProcessingTime += elapsedMilliseconds;
                    }
                    break;
            }
        }
    }

    #endregion // Methods
}