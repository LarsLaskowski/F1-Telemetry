using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Time trial data (F1 2025)
/// </summary>
public class TimeTrialData2025 : ITimeTrialData2025
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public TimeTrialData2025()
    {
        PlayerSessionBestDataSet = new TimeTrialDataSet2025();
        PersonalBestDataSet = new TimeTrialDataSet2025();
        RivalDataSet = new TimeTrialDataSet2025();
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