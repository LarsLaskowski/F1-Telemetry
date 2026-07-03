namespace F1Server.Core.Packets.Data;

/// <summary>
/// Pressure of all tyres
/// </summary>
public class TyresPressure
{
    #region Properties

    /// <summary>
    /// Rear left
    /// </summary>
    public float RearLeft { get; set; }

    /// <summary>
    /// Rear right
    /// </summary>
    public float RearRight { get; set; }

    /// <summary>
    /// Front left
    /// </summary>
    public float FrontLeft { get; set; }

    /// <summary>
    /// Front right
    /// </summary>
    public float FrontRight { get; set; }

    #endregion // Properties
}