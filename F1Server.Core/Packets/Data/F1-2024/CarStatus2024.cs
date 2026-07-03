using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Car status data - F1 2024
/// </summary>
public class CarStatus2024 : ICarStatus2024
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public CarStatus2024()
    {
        CarStatusData = new CarStatusData2024[22];

        for (int carStatus = 0; carStatus < CarStatusData.Length; ++carStatus)
        {
            var carStatusData = new CarStatusData2024();

            CarStatusData[carStatus] = carStatusData;
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