using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Implementation of participant data interface 2022
/// </summary>
public class ParticipantData2022 : IParticipantData2022
{
    #region IParticipantData2021

    /// <summary>
    /// Is AI controlled or human
    /// </summary>
    public bool IsAIControlled { get; set; }

    /// <summary>
    /// Id of driver
    /// </summary>
    public ushort DriverId { get; set; }

    /// <summary>
    /// Id of team
    /// </summary>
    public ushort TeamId { get; set; }

    /// <summary>
    /// Race number of the car
    /// </summary>
    public ushort RaceNumber { get; set; }

    /// <summary>
    /// Nationality of the driver
    /// </summary>
    public ushort Nationality { get; set; }

    /// <summary>
    /// Name of the driver
    /// </summary>
    public string DriverName { get; set; }

    /// <summary>
    /// Is telemetry restricted or public?
    /// </summary>
    public bool IsPublicTelemetry { get; set; }

    /// <summary>
    /// Identifier for network players
    /// </summary>
    public ushort NetworkId { get; set; }

    /// <summary>
    /// Is my team?
    /// </summary>
    public bool IsMyTeam { get; set; }

    #endregion // IParticipantData2021
}