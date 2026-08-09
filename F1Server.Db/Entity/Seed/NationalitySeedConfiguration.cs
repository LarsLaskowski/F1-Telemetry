using F1Server.Core.Data;
using F1Server.Db.Entity.Tables;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace F1Server.Db.Entity.Seed;

/// <summary>
/// Seeds the default set of nationalities
/// </summary>
public sealed class NationalitySeedConfiguration : IEntityTypeConfiguration<NationalityEntity>
{
    #region IEntityTypeConfiguration

    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<NationalityEntity> builder)
    {
        try
        {
            builder.HasData(new NationalityEntity
                            {
                                Id = 1,
                                NationalityGameId = 0,
                                Name = SeedNames.Unknown
                            },
                            new NationalityEntity
                            {
                                Id = 2,
                                NationalityGameId = 1,
                                Name = SeedNames.American
                            },
                            new NationalityEntity
                            {
                                Id = 3,
                                NationalityGameId = 2,
                                Name = SeedNames.Argentinean
                            },
                            new NationalityEntity
                            {
                                Id = 4,
                                NationalityGameId = 3,
                                Name = SeedNames.Australian
                            },
                            new NationalityEntity
                            {
                                Id = 5,
                                NationalityGameId = 4,
                                Name = SeedNames.Austrian
                            },
                            new NationalityEntity
                            {
                                Id = 6,
                                NationalityGameId = 5,
                                Name = SeedNames.Azerbaijani
                            },
                            new NationalityEntity
                            {
                                Id = 7,
                                NationalityGameId = 6,
                                Name = SeedNames.Bahraini
                            },
                            new NationalityEntity
                            {
                                Id = 8,
                                NationalityGameId = 7,
                                Name = SeedNames.Belgian
                            },
                            new NationalityEntity
                            {
                                Id = 9,
                                NationalityGameId = 8,
                                Name = SeedNames.Bolivian
                            },
                            new NationalityEntity
                            {
                                Id = 10,
                                NationalityGameId = 9,
                                Name = SeedNames.Brazilian
                            },
                            new NationalityEntity
                            {
                                Id = 11,
                                NationalityGameId = 10,
                                Name = SeedNames.British
                            },
                            new NationalityEntity
                            {
                                Id = 12,
                                NationalityGameId = 11,
                                Name = SeedNames.Bulgarian
                            },
                            new NationalityEntity
                            {
                                Id = 13,
                                NationalityGameId = 12,
                                Name = SeedNames.Cameroonian
                            },
                            new NationalityEntity
                            {
                                Id = 14,
                                NationalityGameId = 13,
                                Name = SeedNames.Canadian
                            },
                            new NationalityEntity
                            {
                                Id = 15,
                                NationalityGameId = 14,
                                Name = SeedNames.Chilean
                            },
                            new NationalityEntity
                            {
                                Id = 16,
                                NationalityGameId = 15,
                                Name = SeedNames.Chinese
                            },
                            new NationalityEntity
                            {
                                Id = 17,
                                NationalityGameId = 16,
                                Name = SeedNames.Colombian
                            },
                            new NationalityEntity
                            {
                                Id = 18,
                                NationalityGameId = 17,
                                Name = SeedNames.CostaRican
                            },
                            new NationalityEntity
                            {
                                Id = 19,
                                NationalityGameId = 18,
                                Name = SeedNames.Croatian
                            },
                            new NationalityEntity
                            {
                                Id = 20,
                                NationalityGameId = 19,
                                Name = SeedNames.Cypriot
                            },
                            new NationalityEntity
                            {
                                Id = 21,
                                NationalityGameId = 20,
                                Name = SeedNames.Czech
                            },
                            new NationalityEntity
                            {
                                Id = 22,
                                NationalityGameId = 21,
                                Name = SeedNames.Danish
                            },
                            new NationalityEntity
                            {
                                Id = 23,
                                NationalityGameId = 22,
                                Name = SeedNames.Dutch
                            },
                            new NationalityEntity
                            {
                                Id = 24,
                                NationalityGameId = 23,
                                Name = SeedNames.Ecuadorian
                            },
                            new NationalityEntity
                            {
                                Id = 25,
                                NationalityGameId = 24,
                                Name = SeedNames.English
                            },
                            new NationalityEntity
                            {
                                Id = 26,
                                NationalityGameId = 25,
                                Name = SeedNames.Emirian
                            },
                            new NationalityEntity
                            {
                                Id = 27,
                                NationalityGameId = 26,
                                Name = SeedNames.Estonian
                            },
                            new NationalityEntity
                            {
                                Id = 28,
                                NationalityGameId = 27,
                                Name = SeedNames.Finnish
                            },
                            new NationalityEntity
                            {
                                Id = 29,
                                NationalityGameId = 28,
                                Name = SeedNames.French
                            },
                            new NationalityEntity
                            {
                                Id = 30,
                                NationalityGameId = 29,
                                Name = SeedNames.German
                            },
                            new NationalityEntity
                            {
                                Id = 31,
                                NationalityGameId = 30,
                                Name = SeedNames.Ghanaian
                            },
                            new NationalityEntity
                            {
                                Id = 32,
                                NationalityGameId = 31,
                                Name = SeedNames.Greek
                            },
                            new NationalityEntity
                            {
                                Id = 33,
                                NationalityGameId = 32,
                                Name = SeedNames.Guatemalan
                            },
                            new NationalityEntity
                            {
                                Id = 34,
                                NationalityGameId = 33,
                                Name = SeedNames.Honduran
                            },
                            new NationalityEntity
                            {
                                Id = 35,
                                NationalityGameId = 34,
                                Name = SeedNames.HongKonger
                            },
                            new NationalityEntity
                            {
                                Id = 36,
                                NationalityGameId = 35,
                                Name = SeedNames.Hungarian
                            },
                            new NationalityEntity
                            {
                                Id = 37,
                                NationalityGameId = 36,
                                Name = SeedNames.Icelander
                            },
                            new NationalityEntity
                            {
                                Id = 38,
                                NationalityGameId = 37,
                                Name = SeedNames.Indian
                            },
                            new NationalityEntity
                            {
                                Id = 39,
                                NationalityGameId = 38,
                                Name = SeedNames.Indonesian
                            },
                            new NationalityEntity
                            {
                                Id = 40,
                                NationalityGameId = 39,
                                Name = SeedNames.Irish
                            },
                            new NationalityEntity
                            {
                                Id = 41,
                                NationalityGameId = 40,
                                Name = SeedNames.Israeli
                            },
                            new NationalityEntity
                            {
                                Id = 42,
                                NationalityGameId = 41,
                                Name = SeedNames.Italian
                            },
                            new NationalityEntity
                            {
                                Id = 43,
                                NationalityGameId = 42,
                                Name = SeedNames.Jamaican
                            },
                            new NationalityEntity
                            {
                                Id = 44,
                                NationalityGameId = 43,
                                Name = SeedNames.Japanese
                            },
                            new NationalityEntity
                            {
                                Id = 45,
                                NationalityGameId = 44,
                                Name = SeedNames.Jordanian
                            },
                            new NationalityEntity
                            {
                                Id = 46,
                                NationalityGameId = 45,
                                Name = SeedNames.Kuwaiti
                            },
                            new NationalityEntity
                            {
                                Id = 47,
                                NationalityGameId = 46,
                                Name = SeedNames.Latvian
                            },
                            new NationalityEntity
                            {
                                Id = 48,
                                NationalityGameId = 47,
                                Name = SeedNames.Lebanese
                            },
                            new NationalityEntity
                            {
                                Id = 49,
                                NationalityGameId = 48,
                                Name = SeedNames.Lithuanian
                            },
                            new NationalityEntity
                            {
                                Id = 50,
                                NationalityGameId = 49,
                                Name = SeedNames.Luxembourger
                            },
                            new NationalityEntity
                            {
                                Id = 51,
                                NationalityGameId = 50,
                                Name = SeedNames.Malaysian
                            },
                            new NationalityEntity
                            {
                                Id = 52,
                                NationalityGameId = 51,
                                Name = SeedNames.Maltese
                            },
                            new NationalityEntity
                            {
                                Id = 53,
                                NationalityGameId = 52,
                                Name = SeedNames.Mexican
                            },
                            new NationalityEntity
                            {
                                Id = 54,
                                NationalityGameId = 53,
                                Name = SeedNames.Monegasque
                            },
                            new NationalityEntity
                            {
                                Id = 55,
                                NationalityGameId = 54,
                                Name = SeedNames.NewZealander
                            },
                            new NationalityEntity
                            {
                                Id = 56,
                                NationalityGameId = 55,
                                Name = SeedNames.Nicaraguan
                            },
                            new NationalityEntity
                            {
                                Id = 57,
                                NationalityGameId = 56,
                                Name = SeedNames.NorthernIrish
                            },
                            new NationalityEntity
                            {
                                Id = 58,
                                NationalityGameId = 57,
                                Name = SeedNames.Norwegian
                            },
                            new NationalityEntity
                            {
                                Id = 59,
                                NationalityGameId = 58,
                                Name = SeedNames.Omani
                            },
                            new NationalityEntity
                            {
                                Id = 60,
                                NationalityGameId = 59,
                                Name = SeedNames.Pakistani
                            },
                            new NationalityEntity
                            {
                                Id = 61,
                                NationalityGameId = 60,
                                Name = SeedNames.Panamanian
                            },
                            new NationalityEntity
                            {
                                Id = 62,
                                NationalityGameId = 61,
                                Name = SeedNames.Paraguayan
                            },
                            new NationalityEntity
                            {
                                Id = 63,
                                NationalityGameId = 62,
                                Name = SeedNames.Peruvian
                            },
                            new NationalityEntity
                            {
                                Id = 64,
                                NationalityGameId = 63,
                                Name = SeedNames.Polish
                            },
                            new NationalityEntity
                            {
                                Id = 65,
                                NationalityGameId = 64,
                                Name = SeedNames.Portuguese
                            },
                            new NationalityEntity
                            {
                                Id = 66,
                                NationalityGameId = 65,
                                Name = SeedNames.Qatari
                            },
                            new NationalityEntity
                            {
                                Id = 67,
                                NationalityGameId = 66,
                                Name = SeedNames.Romanian
                            },
                            new NationalityEntity
                            {
                                Id = 68,
                                NationalityGameId = 67,
                                Name = SeedNames.Russian
                            },
                            new NationalityEntity
                            {
                                Id = 69,
                                NationalityGameId = 68,
                                Name = SeedNames.Salvadoran
                            },
                            new NationalityEntity
                            {
                                Id = 70,
                                NationalityGameId = 69,
                                Name = SeedNames.Saudi
                            },
                            new NationalityEntity
                            {
                                Id = 71,
                                NationalityGameId = 70,
                                Name = SeedNames.Scottish
                            },
                            new NationalityEntity
                            {
                                Id = 72,
                                NationalityGameId = 71,
                                Name = SeedNames.Serbian
                            },
                            new NationalityEntity
                            {
                                Id = 73,
                                NationalityGameId = 72,
                                Name = SeedNames.Singaporean
                            },
                            new NationalityEntity
                            {
                                Id = 74,
                                NationalityGameId = 73,
                                Name = SeedNames.Slovakian
                            },
                            new NationalityEntity
                            {
                                Id = 75,
                                NationalityGameId = 74,
                                Name = SeedNames.Slovenian
                            },
                            new NationalityEntity
                            {
                                Id = 76,
                                NationalityGameId = 75,
                                Name = SeedNames.SouthKorean
                            },
                            new NationalityEntity
                            {
                                Id = 77,
                                NationalityGameId = 76,
                                Name = SeedNames.SouthAfrican
                            },
                            new NationalityEntity
                            {
                                Id = 78,
                                NationalityGameId = 77,
                                Name = SeedNames.Spanish
                            },
                            new NationalityEntity
                            {
                                Id = 79,
                                NationalityGameId = 78,
                                Name = SeedNames.Swedish
                            },
                            new NationalityEntity
                            {
                                Id = 80,
                                NationalityGameId = 79,
                                Name = SeedNames.Swiss
                            },
                            new NationalityEntity
                            {
                                Id = 81,
                                NationalityGameId = 80,
                                Name = SeedNames.Thai
                            },
                            new NationalityEntity
                            {
                                Id = 82,
                                NationalityGameId = 81,
                                Name = SeedNames.Turkish
                            },
                            new NationalityEntity
                            {
                                Id = 83,
                                NationalityGameId = 82,
                                Name = SeedNames.Uruguayan
                            },
                            new NationalityEntity
                            {
                                Id = 84,
                                NationalityGameId = 83,
                                Name = SeedNames.Ukrainian
                            },
                            new NationalityEntity
                            {
                                Id = 85,
                                NationalityGameId = 84,
                                Name = SeedNames.Venezuelan
                            },
                            new NationalityEntity
                            {
                                Id = 86,
                                NationalityGameId = 85,
                                Name = SeedNames.Barbadian
                            },
                            new NationalityEntity
                            {
                                Id = 87,
                                NationalityGameId = 86,
                                Name = SeedNames.Welsh
                            },
                            new NationalityEntity
                            {
                                Id = 88,
                                NationalityGameId = 87,
                                Name = SeedNames.Vietnamese
                            },
                            new NationalityEntity
                            {
                                Id = 89,
                                NationalityGameId = 88,
                                Name = SeedNames.Algerian
                            },
                            new NationalityEntity
                            {
                                Id = 90,
                                NationalityGameId = 89,
                                Name = SeedNames.Bosnian
                            },
                            new NationalityEntity
                            {
                                Id = 91,
                                NationalityGameId = 90,
                                Name = SeedNames.Filipino
                            });
        }
        catch
        {
            // Ignore exceptions in this step
        }
    }

    #endregion // IEntityTypeConfiguration
}