using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Car status data - F1 2025
/// </summary>
public class CarStatus2025 : ICarStatus2025
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public CarStatus2025()
    {
        CarStatusData = new CarStatusData2025[22];

        for (int carStatus = 0; carStatus < CarStatusData.Length; ++carStatus)
        {
            var carStatusData = new CarStatusData2025();

            CarStatusData[carStatus] = carStatusData;
        }
    }

    #endregion // Constructors

    #region ICarStatusBase

    /// <inheritdoc/>
    public ICarStatusDataBase[] CarStatusData { get; }

    #endregion // ICarStatusBase
}