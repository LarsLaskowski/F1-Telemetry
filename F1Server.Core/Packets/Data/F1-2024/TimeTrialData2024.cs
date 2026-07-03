using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Time trial data (F1 2024)
/// </summary>
public class TimeTrialData2024 : ITimeTrialDataBase
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public TimeTrialData2024()
    {
        PlayerSessionBestDataSet = new TimeTrialDataSet2024();
        PersonalBestDataSet = new TimeTrialDataSet2024();
        RivalDataSet = new TimeTrialDataSet2024();
    }

    #endregion // Constructors

    #region ITimeTrialDataBase

    /// <summary>
    /// Player session best data set
    /// </summary>
    public ITimeTrialDataSetBase PlayerSessionBestDataSet { get; }

    /// <summary>
    /// Personal best data set
    /// </summary>
    public ITimeTrialDataSetBase PersonalBestDataSet { get; }

    /// <summary>
    /// Rival data set
    /// </summary>
    public ITimeTrialDataSetBase RivalDataSet { get; }

    #endregion // ITimeTrialDataBase
}