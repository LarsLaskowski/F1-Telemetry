namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Interface participant data of session (F1 2025)
/// </summary>
public interface IParticipantData2025 : IParticipantData2024
{
    #region Properties

    /// <summary>
    /// Number of colors valid for this car
    /// </summary>
    ushort NumColors { get; set; }

    /// <summary>
    /// Colors of the car
    /// </summary>
    ILiveryColor[] LiveryColors { get; set; }

    #endregion // Properties
}