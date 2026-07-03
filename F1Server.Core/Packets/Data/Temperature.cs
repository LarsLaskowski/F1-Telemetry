namespace F1Server.Core.Packets.Data;

/// <summary>
/// Interface for temperature on brakes or wheels
/// </summary>
public class Temperature
{
    #region Properties

    /// <summary>
    /// Rear left
    /// </summary>
    public ushort RearLeft { get; set; }

    /// <summary>
    /// Rear right
    /// </summary>
    public ushort RearRight { get; set; }

    /// <summary>
    /// Front left
    /// </summary>
    public ushort FrontLeft { get; set; }

    /// <summary>
    /// Front right
    /// </summary>
    public ushort FrontRight { get; set; }

    #endregion // Properties
}