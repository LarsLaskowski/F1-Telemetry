using F1Server.Core.Enumerations;

namespace F1Server.Core.Utils;

/// <summary>
/// Map game result status to our <see cref="F1Server.Core.Enumerations.ResultStatus"/> enumeration
/// </summary>
internal static class ResultStatusMapper
{
    #region Methods

    /// <summary>
    /// Map game (F1 2019) to app ResultStatus enumeration
    /// </summary>
    /// <param name="gameResultStatus">Game value</param>
    /// <returns>Enumeration-Value</returns>
    public static ResultStatus MapResultStatus2019(byte gameResultStatus)
    {
        return (ResultStatus)Enum.ToObject(typeof(ResultStatus), gameResultStatus + 1);
    }

    /// <summary>
    /// Map game (F1 2020) to app ResultStatus enumeration
    /// </summary>
    /// <param name="gameResultStatus">Game value</param>
    /// <returns>Enumeration-Value</returns>
    public static ResultStatus MapResultStatus2020(byte gameResultStatus)
    {
        // No changes since F1 2019
        return MapResultStatus2019(gameResultStatus);
    }

    /// <summary>
    /// Map game (F1 2021) to app ResultStatus enumeration
    /// </summary>
    /// <param name="gameResultStatus">Game value</param>
    /// <returns>Enumeration-Value</returns>
    public static ResultStatus MapResultStatus2021(byte gameResultStatus)
    {
        if (gameResultStatus == 4)
        {
            gameResultStatus = 8;
        }

        if (gameResultStatus < 4)
        {
            ++gameResultStatus;
        }

        return (ResultStatus)Enum.ToObject(typeof(ResultStatus), gameResultStatus);
    }

    /// <summary>
    /// Map game (F1 2022) to app ResultStatus enumeration
    /// </summary>
    /// <param name="gameResultStatus">Game value</param>
    /// <returns>Enumeration-Value</returns>
    public static ResultStatus MapResultStatus2022(byte gameResultStatus)
    {
        // no changes since last version
        return MapResultStatus2021(gameResultStatus);
    }

    /// <summary>
    /// Map game (F1 2023) to app ResultStatus enumeration
    /// </summary>
    /// <param name="gameResultStatus">Game value</param>
    /// <returns>Enumeration-Value</returns>
    public static ResultStatus MapResultStatus2023(byte gameResultStatus)
    {
        // no changes since last version
        return MapResultStatus2021(gameResultStatus);
    }

    /// <summary>
    /// Map game (F1 2024) to app ResultStatus enumeration
    /// </summary>
    /// <param name="gameResultStatus">Game value</param>
    /// <returns>Enumeration-Value</returns>
    public static ResultStatus MapResultStatus2024(byte gameResultStatus)
    {
        // no changes since last version
        return MapResultStatus2021(gameResultStatus);
    }

    /// <summary>
    /// Map game (F1 2025) to app ResultStatus enumeration
    /// </summary>
    /// <param name="gameResultStatus">Game value</param>
    /// <returns>Enumeration-Value</returns>
    public static ResultStatus MapResultStatus2025(byte gameResultStatus)
    {
        // no changes since last version
        return MapResultStatus2021(gameResultStatus);
    }

    #endregion // Methods
}