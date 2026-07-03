namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Livery color interface
/// </summary>
public interface ILiveryColor
{
    #region Properties

    /// <summary>
    /// Red
    /// </summary>
    ushort Red { get; set; }

    /// <summary>
    /// Green
    /// </summary>
    ushort Green { get; set; }

    /// <summary>
    /// Blue
    /// </summary>
    ushort Blue { get; set; }

    #endregion // Properties
}