using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Car status data - F1 2022
/// </summary>
public class CarStatus2022 : ICarStatus2022
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public CarStatus2022()
    {
        CarStatusData = new CarStatusData2022[22];

        for (int catStatusData = 0; catStatusData < CarStatusData.Length; ++catStatusData)
        {
            var carStatusData = new CarStatusData2022();

            CarStatusData[catStatusData] = carStatusData;
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