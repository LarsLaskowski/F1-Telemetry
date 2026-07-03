namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Complete time trial data
/// </summary>
public interface ITimeTrialDataBase
{
    #region Properties

    /// <summary>
    /// Player session best data set
    /// </summary>
    ITimeTrialDataSetBase PlayerSessionBestDataSet { get; }

    /// <summary>
    /// Personal best data set
    /// </summary>
    ITimeTrialDataSetBase PersonalBestDataSet { get; }

    /// <summary>
    /// Rival data set
    /// </summary>
    ITimeTrialDataSetBase RivalDataSet { get; }

    #endregion // Properties
}