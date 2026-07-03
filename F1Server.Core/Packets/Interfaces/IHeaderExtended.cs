namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Basic interface for extended data since F1 2020
/// </summary>
public interface IHeaderExtended
{
    #region Properties

    /// <summary>
    /// Index of secondary player's car (splitscreen) - 255 if no second car
    /// </summary>
    ushort PlayerCarIndexSecondary { get; }

    #endregion // Properties
}