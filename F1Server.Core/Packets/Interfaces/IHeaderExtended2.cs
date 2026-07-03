namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Basic interface for extended data since F1 2023
/// </summary>
public interface IHeaderExtended2
{
    #region Properties

    /// <summary>
    /// Game year - last two digits
    /// </summary>
    ushort GameYear { get; }

    /// <summary>
    /// Overall identifier for the frame the data was received (doesn't go back after falshbacks)
    /// </summary>
    uint OverallFrameIdentifier { get; }

    #endregion // Properties
}