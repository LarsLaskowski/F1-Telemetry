using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Time trial data (F1 2026)
/// </summary>
public class TimeTrialData2026 : ITimeTrialData2026
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public TimeTrialData2026()
    {
        PlayerSessionBestDataSet = new TimeTrialDataSet2026();
        PersonalBestDataSet = new TimeTrialDataSet2026();
        RivalDataSet = new TimeTrialDataSet2026();
    }

    #endregion // Constructors

    #region ITimeTrialData2026

    /// <inheritdoc/>
    public ITimeTrialDataSetBase PlayerSessionBestDataSet { get; }

    /// <inheritdoc/>
    public ITimeTrialDataSetBase PersonalBestDataSet { get; }

    /// <inheritdoc/>
    public ITimeTrialDataSetBase RivalDataSet { get; }

    #endregion // ITimeTrialData2026
}