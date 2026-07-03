using F1Server.Core.Enumerations;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Wheel surface type
/// </summary>
public class WheelSurface
{
    #region Properties

    /// <summary>
    /// Surface type rear left wheel
    /// </summary>
    public SurfaceType RearLeft { get; set; }

    /// <summary>
    /// Surface type rear right wheel
    /// </summary>
    public SurfaceType RearRight { get; set; }

    /// <summary>
    /// Surface type front left wheel
    /// </summary>
    public SurfaceType FrontLeft { get; set; }

    /// <summary>
    /// Surface type front right wheel
    /// </summary>
    public SurfaceType FrontRight { get; set; }

    #endregion // Properties
}