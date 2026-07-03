namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Interface for basic participant data of session
/// </summary>
public interface IParticipantDataBase
{
    #region Properties

    /// <summary>
    /// Is AI controlled or human
    /// </summary>
    bool IsAIControlled { get; set; }

    /// <summary>
    /// Id of driver
    /// </summary>
    ushort DriverId { get; set; }

    /// <summary>
    /// Id of team
    /// </summary>
    ushort TeamId { get; set; }

    /// <summary>
    /// Race number of the car
    /// </summary>
    ushort RaceNumber { get; set; }

    /// <summary>
    /// Nationality of the driver
    /// </summary>
    ushort Nationality { get; set; }

    /// <summary>
    /// Name of the driver
    /// </summary>
    string DriverName { get; set; }

    /// <summary>
    /// Is telemetry restricted or public?
    /// </summary>
    bool IsPublicTelemetry { get; set; }

    #endregion // Properties
}