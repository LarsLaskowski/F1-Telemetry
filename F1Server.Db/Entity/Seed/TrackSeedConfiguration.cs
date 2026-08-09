using F1Server.Core.Data;
using F1Server.Db.Entity.Tables;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace F1Server.Db.Entity.Seed;

/// <summary>
/// Seeds the default set of tracks
/// </summary>
public sealed class TrackSeedConfiguration : IEntityTypeConfiguration<TrackEntity>
{
    #region IEntityTypeConfiguration

    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<TrackEntity> builder)
    {
        try
        {
            builder.HasData(new TrackEntity
                            {
                                Id = 1,
                                TrackNumber = 0,
                                Name = SeedNames.Melbourne,
                                LapReferenceTime = 76086,
                                Sector1ReferenceTime = 26213,
                                Sector2ReferenceTime = 17547,
                                Sector3ReferenceTime = 32326
                            },
                            new TrackEntity
                            {
                                Id = 2,
                                TrackNumber = 1,
                                Name = SeedNames.PaulRicard,
                                LapReferenceTime = 88384,
                                Sector1ReferenceTime = 21725,
                                Sector2ReferenceTime = 27336,
                                Sector3ReferenceTime = 39323
                            },
                            new TrackEntity
                            {
                                Id = 3,
                                TrackNumber = 2,
                                Name = SeedNames.Shanghai,
                                LapReferenceTime = 90098,
                                Sector1ReferenceTime = 23633,
                                Sector2ReferenceTime = 27041,
                                Sector3ReferenceTime = 39424
                            },
                            new TrackEntity
                            {
                                Id = 4,
                                TrackNumber = 3,
                                Name = SeedNames.SakhirBahrain,
                                LapReferenceTime = 87550,
                                Sector1ReferenceTime = 28228,
                                Sector2ReferenceTime = 37605,
                                Sector3ReferenceTime = 21717
                            },
                            new TrackEntity
                            {
                                Id = 5,
                                TrackNumber = 4,
                                Name = SeedNames.Catalunya,
                                LapReferenceTime = 70490,
                                Sector1ReferenceTime = 21268,
                                Sector2ReferenceTime = 28388,
                                Sector3ReferenceTime = 20834
                            },
                            new TrackEntity
                            {
                                Id = 6,
                                TrackNumber = 5,
                                Name = SeedNames.Monaco,
                                LapReferenceTime = 69897,
                                Sector1ReferenceTime = 18286,
                                Sector2ReferenceTime = 33560,
                                Sector3ReferenceTime = 18051
                            },
                            new TrackEntity
                            {
                                Id = 7,
                                TrackNumber = 6,
                                Name = SeedNames.Montreal,
                                LapReferenceTime = 68744,
                                Sector1ReferenceTime = 19103,
                                Sector2ReferenceTime = 21748,
                                Sector3ReferenceTime = 27893
                            },
                            new TrackEntity
                            {
                                Id = 8,
                                TrackNumber = 7,
                                Name = SeedNames.Silverstone,
                                LapReferenceTime = 84532,
                                Sector1ReferenceTime = 26940,
                                Sector2ReferenceTime = 34630,
                                Sector3ReferenceTime = 22962
                            },
                            new TrackEntity
                            {
                                Id = 9,
                                TrackNumber = 8,
                                Name = SeedNames.Hockenheim,
                                LapReferenceTime = 70963,
                                Sector1ReferenceTime = 15279,
                                Sector2ReferenceTime = 34159,
                                Sector3ReferenceTime = 21525
                            },
                            new TrackEntity
                            {
                                Id = 10,
                                TrackNumber = 9,
                                Name = SeedNames.Hungaroring,
                                LapReferenceTime = 74551,
                                Sector1ReferenceTime = 26892,
                                Sector2ReferenceTime = 26319,
                                Sector3ReferenceTime = 21340
                            },
                            new TrackEntity
                            {
                                Id = 11,
                                TrackNumber = 10,
                                Name = SeedNames.Spa,
                                LapReferenceTime = 100331,
                                Sector1ReferenceTime = 29592,
                                Sector2ReferenceTime = 42794,
                                Sector3ReferenceTime = 27945
                            },
                            new TrackEntity
                            {
                                Id = 12,
                                TrackNumber = 11,
                                Name = SeedNames.Monza,
                                LapReferenceTime = 78401,
                                Sector1ReferenceTime = 26003,
                                Sector2ReferenceTime = 26278,
                                Sector3ReferenceTime = 26120
                            },
                            new TrackEntity
                            {
                                Id = 13,
                                TrackNumber = 12,
                                Name = SeedNames.Singapore,
                                LapReferenceTime = 94904,
                                Sector1ReferenceTime = 26392,
                                Sector2ReferenceTime = 36524,
                                Sector3ReferenceTime = 31988
                            },
                            new TrackEntity
                            {
                                Id = 14,
                                TrackNumber = 13,
                                Name = SeedNames.Suzuka,
                                LapReferenceTime = 86051,
                                Sector1ReferenceTime = 30889,
                                Sector2ReferenceTime = 38756,
                                Sector3ReferenceTime = 16406
                            },
                            new TrackEntity
                            {
                                Id = 15,
                                TrackNumber = 14,
                                Name = SeedNames.AbuDhabi,
                                LapReferenceTime = 81379,
                                Sector1ReferenceTime = 17113,
                                Sector2ReferenceTime = 35001,
                                Sector3ReferenceTime = 29265
                            },
                            new TrackEntity
                            {
                                Id = 16,
                                TrackNumber = 15,
                                Name = SeedNames.Texas,
                                LapReferenceTime = 91376,
                                Sector1ReferenceTime = 25031,
                                Sector2ReferenceTime = 36621,
                                Sector3ReferenceTime = 29724
                            },
                            new TrackEntity
                            {
                                Id = 17,
                                TrackNumber = 16,
                                Name = SeedNames.Brazil,
                                LapReferenceTime = 67039,
                                Sector1ReferenceTime = 16654,
                                Sector2ReferenceTime = 33806,
                                Sector3ReferenceTime = 16579
                            },
                            new TrackEntity
                            {
                                Id = 18,
                                TrackNumber = 17,
                                Name = SeedNames.Austria,
                                LapReferenceTime = 62994,
                                Sector1ReferenceTime = 15716,
                                Sector2ReferenceTime = 28174,
                                Sector3ReferenceTime = 19054
                            },
                            new TrackEntity
                            {
                                Id = 19,
                                TrackNumber = 18,
                                Name = SeedNames.Sochi,
                                LapReferenceTime = 89867,
                                Sector1ReferenceTime = 32606,
                                Sector2ReferenceTime = 31066,
                                Sector3ReferenceTime = 26195
                            },
                            new TrackEntity
                            {
                                Id = 20,
                                TrackNumber = 19,
                                Name = SeedNames.Mexico,
                                LapReferenceTime = 75181,
                                Sector1ReferenceTime = 28221,
                                Sector2ReferenceTime = 27944,
                                Sector3ReferenceTime = 19016
                            },
                            new TrackEntity
                            {
                                Id = 21,
                                TrackNumber = 20,
                                Name = SeedNames.BakuAzerbaijan,
                                LapReferenceTime = 99352,
                                Sector1ReferenceTime = 35288,
                                Sector2ReferenceTime = 40019,
                                Sector3ReferenceTime = 24045
                            },
                            new TrackEntity
                            {
                                Id = 22,
                                TrackNumber = 21,
                                Name = SeedNames.SakhirShort,
                                LapReferenceTime = 53252,
                                Sector1ReferenceTime = 18566,
                                Sector2ReferenceTime = 18474,
                                Sector3ReferenceTime = 16212
                            },
                            new TrackEntity
                            {
                                Id = 23,
                                TrackNumber = 22,
                                Name = SeedNames.SilverstoneShort,
                                LapReferenceTime = 51812,
                                Sector1ReferenceTime = 10960,
                                Sector2ReferenceTime = 16373,
                                Sector3ReferenceTime = 24479
                            },
                            new TrackEntity
                            {
                                Id = 24,
                                TrackNumber = 23,
                                Name = SeedNames.TexasShort,
                                LapReferenceTime = 30000,
                                Sector1ReferenceTime = 10000,
                                Sector2ReferenceTime = 10000,
                                Sector3ReferenceTime = 10000
                            },
                            new TrackEntity
                            {
                                Id = 25,
                                TrackNumber = 24,
                                Name = SeedNames.SuzukaShort,
                                LapReferenceTime = 30000,
                                Sector1ReferenceTime = 10000,
                                Sector2ReferenceTime = 10000,
                                Sector3ReferenceTime = 10000
                            },
                            new TrackEntity
                            {
                                Id = 26,
                                TrackNumber = 25,
                                Name = SeedNames.Hanoi,
                                LapReferenceTime = 93454,
                                Sector1ReferenceTime = 25342,
                                Sector2ReferenceTime = 40367,
                                Sector3ReferenceTime = 27745
                            },
                            new TrackEntity
                            {
                                Id = 27,
                                TrackNumber = 26,
                                Name = SeedNames.Zandvoort,
                                LapReferenceTime = 67834,
                                Sector1ReferenceTime = 23711,
                                Sector2ReferenceTime = 23428,
                                Sector3ReferenceTime = 20695
                            },
                            new TrackEntity
                            {
                                Id = 28,
                                TrackNumber = 27,
                                Name = SeedNames.Imola,
                                LapReferenceTime = 73311,
                                Sector1ReferenceTime = 23564,
                                Sector2ReferenceTime = 25323,
                                Sector3ReferenceTime = 24515
                            },
                            new TrackEntity
                            {
                                Id = 29,
                                TrackNumber = 28,
                                Name = SeedNames.Portimao,
                                LapReferenceTime = 75588,
                                Sector1ReferenceTime = 21567,
                                Sector2ReferenceTime = 29255,
                                Sector3ReferenceTime = 24766
                            },
                            new TrackEntity
                            {
                                Id = 30,
                                TrackNumber = 29,
                                Name = SeedNames.Jeddah,
                                LapReferenceTime = 85870,
                                Sector1ReferenceTime = 31244,
                                Sector2ReferenceTime = 27721,
                                Sector3ReferenceTime = 26905
                            },
                            new TrackEntity
                            {
                                Id = 31,
                                TrackNumber = 30,
                                Name = SeedNames.Miami,
                                LapReferenceTime = 85890,
                                Sector1ReferenceTime = 29419,
                                Sector2ReferenceTime = 31444,
                                Sector3ReferenceTime = 25027
                            },
                            new TrackEntity
                            {
                                Id = 32,
                                TrackNumber = 31,
                                Name = SeedNames.LasVegas,
                                LapReferenceTime = 90406,
                                Sector1ReferenceTime = 43317,
                                Sector2ReferenceTime = 25530,
                                Sector3ReferenceTime = 21559
                            },
                            new TrackEntity
                            {
                                Id = 33,
                                TrackNumber = 32,
                                Name = SeedNames.Losail,
                                LapReferenceTime = 79850,
                                Sector1ReferenceTime = 26190,
                                Sector2ReferenceTime = 26501,
                                Sector3ReferenceTime = 27159
                            },
                            new TrackEntity
                            {
                                Id = 34,
                                TrackNumber = 39,
                                Name = SeedNames.SilverstoneReverse,
                                LapReferenceTime = 0,
                                Sector1ReferenceTime = 0,
                                Sector2ReferenceTime = 0,
                                Sector3ReferenceTime = 0
                            },
                            new TrackEntity
                            {
                                Id = 35,
                                TrackNumber = 40,
                                Name = SeedNames.AustriaReverse,
                                LapReferenceTime = 0,
                                Sector1ReferenceTime = 0,
                                Sector2ReferenceTime = 0,
                                Sector3ReferenceTime = 0
                            },
                            new TrackEntity
                            {
                                Id = 36,
                                TrackNumber = 41,
                                Name = SeedNames.ZandvoortReverse,
                                LapReferenceTime = 0,
                                Sector1ReferenceTime = 0,
                                Sector2ReferenceTime = 0,
                                Sector3ReferenceTime = 0
                            },
                            new TrackEntity
                            {
                                Id = 37,
                                TrackNumber = 42,
                                Name = SeedNames.Madrid,
                                LapReferenceTime = 0,
                                Sector1ReferenceTime = 0,
                                Sector2ReferenceTime = 0,
                                Sector3ReferenceTime = 0
                            });
        }
        catch
        {
            // Ignore exceptions in this step
        }
    }

    #endregion // IEntityTypeConfiguration
}