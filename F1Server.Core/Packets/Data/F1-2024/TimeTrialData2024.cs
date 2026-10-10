using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Time trial data (F1 2024)
/// </summary>
public class TimeTrialData2024 : ITimeTrialData2024
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

    /// <inheritdoc/>
    public ITimeTrialDataSetBase PlayerSessionBestDataSet { get; }

    /// <inheritdoc/>
    public ITimeTrialDataSetBase PersonalBestDataSet { get; }

    /// <inheritdoc/>
    public ITimeTrialDataSetBase RivalDataSet { get; }

    #endregion // ITimeTrialDataBase
}