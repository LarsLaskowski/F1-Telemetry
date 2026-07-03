using F1Server.Core.Data;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Car status data - F1 2026
/// </summary>
public class CarStatus2026 : ICarStatus2026
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public CarStatus2026()
    {
        CarStatusData = new CarStatusData2026[ConstData.F12026MaxCars];

        for (int carStatus = 0; carStatus < CarStatusData.Length; ++carStatus)
        {
            var carStatusData = new CarStatusData2026();

            CarStatusData[carStatus] = carStatusData;
        }
    }

    #endregion // Constructors

    #region ICarStatusBase

    /// <inheritdoc/>
    public ICarStatusDataBase[] CarStatusData { get; }

    #endregion // ICarStatusBase
}