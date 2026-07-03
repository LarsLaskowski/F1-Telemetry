namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Interface participant data of session (F1 2024)
/// </summary>
public interface IParticipantData2024 : IParticipantData2023
{
    #region Properties

    /// <summary>
    /// F1 World tech level
    /// </summary>
    ushort TechLevel { get; set; }

    #endregion // Properties
}