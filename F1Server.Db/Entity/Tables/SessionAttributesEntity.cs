using System.ComponentModel.DataAnnotations.Schema;

using F1Server.Core.Enumerations;

using Microsoft.EntityFrameworkCore;

namespace F1Server.Db.Entity.Tables;

/// <summary>
/// Attributes of a session
/// </summary>
[Table("SessionAttributes")]
[Comment("Attributes of a session, like weather, assists and so on.")]
[Index(nameof(SessionId))]
public class SessionAttributesEntity
{
    #region Properties

    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Id of session entry in table <see cref="SessionEntity"/>
    /// </summary>
    public long SessionId { get; set; }

    /// <summary>
    /// Number of flashbacks used
    /// </summary>
    public int FlashbacksUsed { get; set; }

    /// <summary>
    /// Flag, to some more parameters (AiDifficulty, SteeringAssist and so on) - database only value
    /// </summary>
    [Column("IsExtendedSession")]
    public int DbIsExtendedSession { get; set; }

    /// <summary>
    /// Flag, to some more parameters (AiDifficulty, SteeringAssist and so on)
    /// </summary>
    [NotMapped]
    public bool IsExtendedSession
    {
        get => DbIsExtendedSession != 0;
        set => DbIsExtendedSession = value ? 1 : 0;
    }

    /// <summary>
    /// First setting of steering assist in session - database only value
    /// </summary>
    [Column("SteeringAssistFirst")]
    public int DbSteeringAssistFirst { get; set; }

    /// <summary>
    /// First setting of steering assist in session
    /// </summary>
    [NotMapped]
    public bool SteeringAssistFirst
    {
        get => DbSteeringAssistFirst != 0;
        set => DbSteeringAssistChanged = value ? 1 : 0;
    }

    /// <summary>
    /// Last value of steering assist in session - database only value
    /// </summary>
    [Column("SteeringAssistLast")]
    public int DbSteeringAssistLast { get; set; }

    /// <summary>
    /// Last value of steering assist in session
    /// </summary>
    [NotMapped]
    public bool SteeringAssistLast
    {
        get => DbSteeringAssistLast != 0;
        set => DbSteeringAssistLast = value ? 1 : 0;
    }

    /// <summary>
    /// Flag, if steering assist was changed in session - database only value
    /// </summary>
    [Column("SteeringAssistChanged")]
    public int DbSteeringAssistChanged { get; set; }

    /// <summary>
    /// Flag, if steering assist was changed in session
    /// </summary>
    [NotMapped]
    public bool SteeringAssistChanged
    {
        get => DbSteeringAssistChanged != 0;
        set => DbSteeringAssistChanged = value ? 1 : 0;
    }

    /// <summary>
    /// First setting of braking assist in session
    /// </summary>
    public BrakingAssist BrakingAssistFirst { get; set; }

    /// <summary>
    /// Last value of braking assist
    /// </summary>
    public BrakingAssist BrakingAssistLast { get; set; }

    /// <summary>
    /// Braking assist changed in session - database only value
    /// </summary>
    [Column("BrakingAssistChanged")]
    public int DbBrakingAssistChanged { get; set; }

    /// <summary>
    /// Braking assist changed in session
    /// </summary>
    [NotMapped]
    public bool BrakingAssistChanged
    {
        get => DbBrakingAssistChanged != 0;
        set => DbBrakingAssistChanged = value ? 1 : 0;
    }

    /// <summary>
    /// First setting of gear box assist in session
    /// </summary>
    public GearboxAssist GearBoxAssistFirst { get; set; }

    /// <summary>
    /// Last value of gear box assist
    /// </summary>
    public GearboxAssist GearBoxAssistLast { get; set; }

    /// <summary>
    /// Gear box assist changed in session - database only value
    /// </summary>
    [Column("GearBoxAssistChanged")]
    public int DbGearBoxAssistChanged { get; set; }

    /// <summary>
    /// Gear box assist changed in session
    /// </summary>
    [NotMapped]
    public bool GearBoxAssistChanged
    {
        get => DbGearBoxAssistChanged != 0;
        set => DbGearBoxAssistChanged = value ? 1 : 0;
    }

    /// <summary>
    /// Game mode
    /// </summary>
    public GameMode GameMode { get; set; }

    /// <summary>
    /// Ruleset
    /// </summary>
    public RuleSet RuleSet { get; set; }

    /// <summary>
    /// Weather condition at session start
    /// </summary>
    public WeatherCondition WeatherStart { get; set; }

    /// <summary>
    /// Weather condition at session end
    /// </summary>
    public WeatherCondition WeatherEnd { get; set; }

    /// <summary>
    /// Number virtual safety car stages
    /// </summary>
    public uint VirtualSafetyCarStages { get; set; }

    /// <summary>
    /// Number of safety car stages
    /// </summary>
    public uint SafetyCarStages { get; set; }

    /// <summary>
    /// Number of red flags
    /// </summary>
    public uint RedFlags { get; set; }

    #endregion // Properties

    #region Navigation properties

    /// <summary>
    /// Session data
    /// </summary>
    [ForeignKey(nameof(SessionId))]
    public virtual SessionEntity Session { get; set; }

    #endregion // Navigation properties
}