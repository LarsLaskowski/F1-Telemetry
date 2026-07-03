using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace F1Server.Db.Entity.Tables;

/// <summary>
/// Entity of car telemetry datas
/// </summary>
[Table("CarTelemetries")]
[Comment("Car telemetry data for each lap in a session.")]
[Index(nameof(PacketNumber))]
[Index(nameof(LapNumberId))]
public class CarTelemetryEntity
{
    #region Properties

    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Number of packet
    /// </summary>
    public int PacketNumber { get; set; }

    /// <summary>
    /// Lap id in the database
    /// </summary>
    public long LapNumberId { get; set; }

    /// <summary>
    /// Current lap distance
    /// </summary>
    public float LapDistance { get; set; }

    /// <summary>
    /// Amount of throttle applied (0.0 - 1.0)
    /// </summary>
    public float Throttle { get; set; }

    /// <summary>
    /// Amount of brake applied (0.0 - 1.0)
    /// </summary>
    public float Brake { get; set; }

    /// <summary>
    /// Amount of clutch applied (0 - 100)
    /// </summary>
    public ushort Clutch { get; set; }

    /// <summary>
    /// Amount of steering applied (-1.0 [left] - 1.0 [right])
    /// </summary>
    public float Steer { get; set; }

    /// <summary>
    /// Engine RPM
    /// </summary>
    public ushort EngineRPM { get; set; }

    /// <summary>
    /// Engine Temperature
    /// </summary>
    public ushort EngineTemperature { get; set; }

    /// <summary>
    /// Current gear (0 - 8, -1 R)
    /// </summary>
    public short Gear { get; set; }

    /// <summary>
    /// Speed in kilometers per hour
    /// </summary>
    public int Speed { get; set; }

    /// <summary>
    /// Flag, if DRS is active or not
    /// </summary>
    [Column("IsDRS")]
    public int DbIsDRS { get; set; }

    /// <summary>
    /// Flag, if DRS is active or not
    /// </summary>
    [NotMapped]
    public bool IsDRS
    {
        get => DbIsDRS != 0;
        set => DbIsDRS = value ? 1 : 0;
    }

    /// <summary>
    /// Rev ligthts indicator in percent
    /// </summary>
    public ushort RevLightsIndicator { get; set; }

    /// <summary>
    /// Brakes temperature front left
    /// </summary>
    public ushort BrakesTempFrontLeft { get; set; }

    /// <summary>
    /// Brakes temperature front right
    /// </summary>
    public ushort BrakesTempFrontRight { get; set; }

    /// <summary>
    /// Brakes temperature rear left
    /// </summary>
    public ushort BrakesTempRearLeft { get; set; }

    /// <summary>
    /// Brakes temperature rear right
    /// </summary>
    public ushort BrakesTempRearRight { get; set; }

    /// <summary>
    /// Tyres surface temperature front left
    /// </summary>
    public ushort TyresSurfaceTempFrontLeft { get; set; }

    /// <summary>
    /// Tyres surface temperature front right
    /// </summary>
    public ushort TyresSurfaceTempFrontRight { get; set; }

    /// <summary>
    /// Tyres surface temperature rear left
    /// </summary>
    public ushort TyresSurfaceTempRearLeft { get; set; }

    /// <summary>
    /// Tyres surface temperature rear right
    /// </summary>
    public ushort TyresSurfaceTempRearRight { get; set; }

    /// <summary>
    /// Tyres inner temperature front left
    /// </summary>
    public ushort TyresInnerTempFrontLeft { get; set; }

    /// <summary>
    /// Tyres inner temperature front right
    /// </summary>
    public ushort TyresInnerTempFrontRight { get; set; }

    /// <summary>
    /// Tyres inner temperature rear left
    /// </summary>
    public ushort TyresInnerTempRearLeft { get; set; }

    /// <summary>
    /// Tyres inner temperature rear right
    /// </summary>
    public ushort TyresInnerTempRearRight { get; set; }

    /// <summary>
    /// Tyres pressure front left
    /// </summary>
    public float TyresPressureFrontLeft { get; set; }

    /// <summary>
    /// Tyres pressure front right
    /// </summary>
    public float TyresPressureFrontRight { get; set; }

    /// <summary>
    /// Tyres pressure rear left
    /// </summary>
    public float TyresPressureRearLeft { get; set; }

    /// <summary>
    /// Tyres pressure rear right
    /// </summary>
    public float TyresPressureRearRight { get; set; }

    #endregion // Properties

    #region Navigation properties

    /// <summary>
    /// Lap
    /// </summary>
    [ForeignKey(nameof(LapNumberId))]
    public virtual LapEntity Lap { get; set; }

    #endregion // Navigation properties
}