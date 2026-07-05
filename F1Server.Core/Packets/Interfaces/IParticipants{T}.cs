namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Interface for public participants data class
/// </summary>
/// <typeparam name="T">Game version dependent interface</typeparam>
public interface IParticipants<out T>
{
    #region Properties

    /// <summary>
    /// Active cars
    /// </summary>
    ushort ActiveCars { get; set; }

    /// <summary>
    /// Data of participants in session
    /// </summary>
    T[] ParticipantData { get; }

    #endregion // Properties
}