using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Car status data - F1 2023
/// </summary>
public class CarStatus2023 : ICarStatus2023
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public CarStatus2023()
    {
        CarStatusData = new CarStatusData2023[22];

        for (int carData = 0; carData < CarStatusData.Length; ++carData)
        {
            var carStatusData = new CarStatusData2023();

            CarStatusData[carData] = carStatusData;
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