using F1Server.Core.Enumerations;

namespace F1Server.Core.Utils;

/// <summary>
/// Visual tyre compound mapper class
/// </summary>
public static class TyreCompoundMapper
{
    #region Methods

    /// <summary>
    /// Maps game visual tyre compound value to enum
    /// </summary>
    /// <param name="gameTyreCompound">Game value</param>
    /// <returns>Enum-Value</returns>
    public static VisualTyreCompound MapVisualTyreCompoundToEnum(ushort gameTyreCompound)
    {
        var visualTyreCompound = VisualTyreCompound.Unknown;

        // Value ranges per the F1 2019-2026 telemetry specs: F1 Modern/Classic use
        // 7, 8, 16, 17, 18; F2 uses 15, 19, 20, 21, 22. No spec documents values above 22.
        switch (gameTyreCompound)
        {
            case 7:
                {
                    visualTyreCompound = VisualTyreCompound.Inter;
                }
                break;

            case 8:
            case 15:
                {
                    visualTyreCompound = VisualTyreCompound.Wet;
                }
                break;

            case 19:
                {
                    visualTyreCompound = VisualTyreCompound.SuperSoft;
                }
                break;

            case 16:
            case 20:
                {
                    visualTyreCompound = VisualTyreCompound.Soft;
                }
                break;

            case 17:
            case 21:
                {
                    visualTyreCompound = VisualTyreCompound.Medium;
                }
                break;

            case 18:
            case 22:
                {
                    visualTyreCompound = VisualTyreCompound.Hard;
                }
                break;
        }

        return visualTyreCompound;
    }

    #endregion // Methods
}