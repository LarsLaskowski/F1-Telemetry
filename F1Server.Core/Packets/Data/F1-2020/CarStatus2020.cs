using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Car status data - F1 2020
/// </summary>
public class CarStatus2020 : ICarStatus2020
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public CarStatus2020()
    {
        CarStatusData = new CarStatusData2020[22];

        for (int carStatusData = 0; carStatusData < CarStatusData.Length; ++carStatusData)
        {
            var carStatusData2020 = new CarStatusData2020
                                    {
                                        TyresWear = new ushort[4],
                                        TyreDamage = new ushort[4]
                                    };

            CarStatusData[carStatusData] = carStatusData2020;
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