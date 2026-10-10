using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Implementation of participant data interface 2024
/// </summary>
public class ParticipantData2024 : IParticipantData2024
{
    #region IParticipantDataBase

    /// <inheritdoc/>
    public bool IsAIControlled { get; set; }

    /// <inheritdoc/>
    public ushort DriverId { get; set; }

    /// <inheritdoc/>
    public ushort TeamId { get; set; }

    /// <inheritdoc/>
    public ushort RaceNumber { get; set; }

    /// <inheritdoc/>
    public ushort Nationality { get; set; }

    /// <inheritdoc/>
    public string DriverName { get; set; } = string.Empty;

    /// <inheritdoc/>
    public bool IsPublicTelemetry { get; set; }

    #endregion // IParticipantDataBase

    #region IParticipantData2021

    /// <inheritdoc/>
    public ushort NetworkId { get; set; }

    /// <inheritdoc/>
    public bool IsMyTeam { get; set; }

    #endregion // IParticipantData2021

    #region IParticipantData2023

    /// <inheritdoc/>
    public bool IsShowOnlineNames { get; set; }

    /// <inheritdoc/>
    public Platforms Platform { get; set; }

    #endregion // IParticipantData2023

    #region IParticipantData2024

    /// <inheritdoc/>
    public ushort TechLevel { get; set; }

    #endregion // IParticipantData2024
}