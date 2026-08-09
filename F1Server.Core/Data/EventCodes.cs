namespace F1Server.Core.Data;

/// <summary>
/// Event code constants shared across event packet extraction and evaluation
/// </summary>
public static class EventCodes
{
    #region Constants

    /// <summary>
    /// Event code constant for "Session Started"
    /// </summary>
    public const string SessionStart = "SSTA";

    /// <summary>
    /// Event code constant for "Session Ended"
    /// </summary>
    public const string SessionEnd = "SEND";

    /// <summary>
    /// Event code constant for "Flashback Activated"
    /// </summary>
    public const string Flashback = "FLBK";

    /// <summary>
    /// Event code constant for "Fastest Lap"
    /// </summary>
    public const string FastestLap = "FTLP";

    /// <summary>
    /// Event code constant for "Retirement"
    /// </summary>
    public const string Retirement = "RTMT";

    /// <summary>
    /// Event code constant for "DRS Enabled"
    /// </summary>
    public const string DRSEnabled = "DRSE";

    /// <summary>
    /// Event code constant for "DRS Disabled"
    /// </summary>
    public const string DRSDisabled = "DRSD";

    /// <summary>
    /// Event code constant for "Team Mate in Pits"
    /// </summary>
    public const string TeamMateInPits = "TMPT";

    /// <summary>
    /// Event code constant for "Chequered Flag"
    /// </summary>
    public const string ChequeredFlag = "CHQF";

    /// <summary>
    /// Event code constant for "Race Winner"
    /// </summary>
    public const string RaceWinner = "RCWN";

    /// <summary>
    /// Event code constant for "Penalty Issued"
    /// </summary>
    public const string PenaltyIssued = "PENA";

    /// <summary>
    /// Event code constant for "Speed Trap Triggered"
    /// </summary>
    public const string SpeedTrapTriggered = "SPTP";

    /// <summary>
    /// Event code constant for "Start Lights"
    /// </summary>
    public const string StartLights = "STLG";

    /// <summary>
    /// Event code constant for "Lights Out"
    /// </summary>
    public const string LightsOut = "LGOT";

    /// <summary>
    /// Event code constant for "Drive Through Served"
    /// </summary>
    public const string DriveThroughServed = "DTSV";

    /// <summary>
    /// Event code constant for "Stop Go Served"
    /// </summary>
    public const string StopGoServed = "SGSV";

    /// <summary>
    /// Event code constant for "Button Status"
    /// </summary>
    public const string ButtonStatus = "BUTN";

    /// <summary>
    /// Event code constant for "Red Flag"
    /// </summary>
    public const string RedFlag = "RDFL";

    /// <summary>
    /// Event code constant for "Overtake"
    /// </summary>
    public const string Overtake = "OVTK";

    /// <summary>
    /// Event code constant for "Safety Car"
    /// </summary>
    public const string SafetyCar = "SCAR";

    /// <summary>
    /// Event code constant for "Collision"
    /// </summary>
    public const string Collision = "COLL";

    #endregion // Constants
}