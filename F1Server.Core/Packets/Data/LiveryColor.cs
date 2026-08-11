using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Livery color of a car
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