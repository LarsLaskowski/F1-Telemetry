using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Implementation of participant data interface 2019
/// </summary>
public class ParticipantData2019 : IParticipantData2019
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
    public string DriverName { get; set; }

    /// <inheritdoc/>
    public bool IsPublicTelemetry { get; set; }

    #endregion // IParticipantDataBase
}