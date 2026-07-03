using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Car status data - F1 2019
/// </summary>
public class CarStatus2019 : ICarStatus2019
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public CarStatus2019()
    {
        CarStatusData = new CarStatusData2019[20];

        for (int carStatus = 0; carStatus < CarStatusData.Length; ++carStatus)
        {
            var carStatusData2019 = new CarStatusData2019
                                    {
                                        TyreDamage = new ushort[4],
                                        TyresWear = new ushort[4]
                                    };

            CarStatusData[carStatus] = carStatusData2019;
        }
    }

    #endregion // Constructors

    #region ICarStatus2019

    /// <summary>
    /// Car status of all cars
    /// </summary>
    public ICarStatusDataBase[] CarStatusData { get; }

    #endregion // ICarStatus2019
}