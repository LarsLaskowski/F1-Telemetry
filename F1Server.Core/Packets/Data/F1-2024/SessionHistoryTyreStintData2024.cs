using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Tyre stint history data of session (F1 2024)
/// </summary>
internal class SessionHistoryTyreStintData2024 : ITyreStintHistoryDataBase
{
    #region ITyreStintHistoryDataBase

    /// <inheritdoc/>
    public ushort EndLap { get; set; }

    /// <inheritdoc/>
    public ushort TyreActualCompound { get; set; }

    /// <inheritdoc/>
    public ushort TyreVisualCompound { get; set; }

    #endregion // ITyreStintHistoryDataBase
}