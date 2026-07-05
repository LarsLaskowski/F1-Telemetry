using F1Server.Core.Enumerations;

namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Interface for lap data packets across all F1 game versions
/// </summary>
public interface ILapDataBase
{
    #region Properties

    /// <summary>
    /// Flag if there is no car available (is nothing from the game)
    /// </summary>
    bool IsEmpty { get; }

    /// <summary>
    /// Distance vehicle is around current lap in meters, negative is finish line not crossed yet
    /// </summary>
    float LapDistance { get; }

    /// <summary>
    /// Total distance travelled in session in meters, can be negative like <see cref="LapDistance"/>
    /// </summary>
    float TotalDistance { get; }

    /// <summary>
    /// Delta in seconds for safety car
    /// </summary>
    float SafetyCarDelta { get; }

    /// <summary>
    /// Actual race position
    /// </summary>
    ushort CarPosition { get; }

    /// <summary>
    /// Current lap number
    /// </summary>
    ushort CurrentLapNumber { get; }

    /// <summary>
    /// Current pit status
    /// </summary>
    PitStatus CurrentPitStatus { get; }

    /// <summary>
    /// Current sector
    /// </summary>
    Sector CurrentSector { get; }

    /// <summary>
    /// Is current lap invalid?
    /// </summary>
    bool IsCurrentLapInvalid { get; }

    /// <summary>
    /// Accumulated time penalties in seconds to be added
    /// </summary>
    ushort TimePenalties { get; }

    /// <summary>
    /// Grid start position
    /// </summary>
    ushort GridPosition { get; }

    /// <summary>
    /// Current driver status
    /// </summary>
    DriverStatus CurrentDriverStatus { get; }

    /// <summary>
    /// Current result status
    /// </summary>
    ResultStatus CurrentResultStatus { get; }

    #endregion // Properties
}