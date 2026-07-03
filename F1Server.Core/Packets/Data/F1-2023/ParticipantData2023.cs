using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Implementation of participant data interface 2023
/// </summary>
public class ParticipantData2023 : IParticipantData2023
{
    #region IParticipantData2023

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

    /// <summary>
    /// Players show online name setting
    /// </summary>
    public bool IsShowOnlineNames { get; set; }

    /// <summary>
    /// Platform of player
    /// </summary>
    public Platforms Platform { get; set; }

    #endregion // IParticipantData2023
}