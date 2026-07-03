using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Car status data - F1 2021
/// </summary>
public class CarStatus2021 : ICarStatus2021
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public CarStatus2021()
    {
        CarStatusData = new CarStatusData2021[22];

        for (int carStatus = 0; carStatus < CarStatusData.Length; ++carStatus)
        {
            var carStatusData2021 = new CarStatusData2021();

            CarStatusData[carStatus] = carStatusData2021;
        }
    }

    #endregion // Constructors

    #region ICarStatusBase

    /// <summary>
    /// Car Status of all cars
    /// </summary>
    public ICarStatusDataBase[] CarStatusData { get; }

    #endregion // ICarStatusBase
}