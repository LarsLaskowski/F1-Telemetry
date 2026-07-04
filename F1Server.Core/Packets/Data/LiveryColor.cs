using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Pressure of all tyres
/// </summary>
public class LiveryColor : ILiveryColor
{
    #region ILiveryColor

    /// <inheritdoc/>
    public ushort Red { get; set; }

    /// <inheritdoc/>
    public ushort Green { get; set; }

    /// <inheritdoc/>
    public ushort Blue { get; set; }

    #endregion // ILiveryColor
}