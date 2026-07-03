using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Implementation of participant data interface 2025
/// </summary>
public class ParticipantData2025 : IParticipantData2025
{
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="ParticipantData2025"/> class
    /// </summary>
    public ParticipantData2025()
    {
        LiveryColors = new ILiveryColor[4];
    }

    #endregion // Constructor

    #region IParticipantData2025

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
    public string DriverName { get; set; }

    /// <inheritdoc/>
    public bool IsPublicTelemetry { get; set; }

    /// <inheritdoc/>
    public ushort NetworkId { get; set; }

    /// <inheritdoc/>
    public bool IsMyTeam { get; set; }

    /// <inheritdoc/>
    public bool IsShowOnlineNames { get; set; }

    /// <inheritdoc/>
    public Platforms Platform { get; set; }

    /// <inheritdoc/>
    public ushort TechLevel { get; set; }

    /// <inheritdoc/>
    public ushort NumColors { get; set; }

    /// <inheritdoc/>
    public ILiveryColor[] LiveryColors { get; set; }

    #endregion // IParticipantData2025
}