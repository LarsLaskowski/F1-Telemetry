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

        switch (gameTyreCompound)
        {
            case 7:
                visualTyreCompound = VisualTyreCompound.Inter;
                break;

            case 8:
            case 15:
                visualTyreCompound = VisualTyreCompound.Wet;
                break;

            case 23:
                visualTyreCompound = VisualTyreCompound.SuperSoft;
                break;

            case 16:
            case 20:
            case 24:
                visualTyreCompound = VisualTyreCompound.Soft;
                break;

            case 17:
            case 21:
            case 25:
                visualTyreCompound = VisualTyreCompound.Medium;
                break;

            case 18:
            case 22:
            case 26:
                visualTyreCompound = VisualTyreCompound.Hard;
                break;

            case 19:
                visualTyreCompound = VisualTyreCompound.SuperSoft;
                break;
        }

        return visualTyreCompound;
    }

    #endregion // Methods
}