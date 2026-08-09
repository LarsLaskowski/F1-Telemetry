using F1Server.Core.Data;
using F1Server.Db.Entity.Tables;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace F1Server.Db.Entity.Seed;

/// <summary>
/// Seeds the default set of teams
/// </summary>
public sealed class TeamSeedConfiguration : IEntityTypeConfiguration<TeamEntity>
{
    #region IEntityTypeConfiguration

    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<TeamEntity> builder)
    {
        try
        {
            builder.HasData(new TeamEntity
                            {
                                Id = 1,
                                TeamGameId = 20190,
                                Name = SeedNames.Mercedes
                            },
                            new TeamEntity
                            {
                                Id = 2,
                                TeamGameId = 20191,
                                Name = SeedNames.Ferrari
                            },
                            new TeamEntity
                            {
                                Id = 3,
                                TeamGameId = 20192,
                                Name = SeedNames.RedBullRacing
                            },
                            new TeamEntity
                            {
                                Id = 4,
                                TeamGameId = 20193,
                                Name = SeedNames.Williams
                            },
                            new TeamEntity
                            {
                                Id = 5,
                                TeamGameId = 20194,
                                Name = SeedNames.RacingPoint
                            },
                            new TeamEntity
                            {
                                Id = 6,
                                TeamGameId = 20195,
                                Name = SeedNames.Renault
                            },
                            new TeamEntity
                            {
                                Id = 7,
                                TeamGameId = 20196,
                                Name = SeedNames.ToroRosso
                            },
                            new TeamEntity
                            {
                                Id = 8,
                                TeamGameId = 20197,
                                Name = SeedNames.Haas
                            },
                            new TeamEntity
                            {
                                Id = 9,
                                TeamGameId = 20198,
                                Name = SeedNames.McLaren
                            },
                            new TeamEntity
                            {
                                Id = 10,
                                TeamGameId = 20199,
                                Name = SeedNames.AlfaRomeo
                            },
                            new TeamEntity
                            {
                                Id = 11,
                                TeamGameId = 201910,
                                Name = SeedNames.McLaren1988
                            },
                            new TeamEntity
                            {
                                Id = 12,
                                TeamGameId = 201911,
                                Name = SeedNames.McLaren1991
                            },
                            new TeamEntity
                            {
                                Id = 13,
                                TeamGameId = 201912,
                                Name = SeedNames.Williams1992
                            },
                            new TeamEntity
                            {
                                Id = 14,
                                TeamGameId = 201913,
                                Name = SeedNames.Ferrari1995
                            },
                            new TeamEntity
                            {
                                Id = 15,
                                TeamGameId = 201914,
                                Name = SeedNames.Williams1996
                            },
                            new TeamEntity
                            {
                                Id = 16,
                                TeamGameId = 201915,
                                Name = SeedNames.McLaren1998
                            },
                            new TeamEntity
                            {
                                Id = 17,
                                TeamGameId = 201916,
                                Name = SeedNames.Ferrari2002
                            },
                            new TeamEntity
                            {
                                Id = 18,
                                TeamGameId = 201917,
                                Name = SeedNames.Ferrari2004
                            },
                            new TeamEntity
                            {
                                Id = 19,
                                TeamGameId = 201918,
                                Name = SeedNames.Renault2006
                            },
                            new TeamEntity
                            {
                                Id = 20,
                                TeamGameId = 201919,
                                Name = SeedNames.Ferrari2007
                            },
                            new TeamEntity
                            {
                                Id = 21,
                                TeamGameId = 201921,
                                Name = SeedNames.RedBull2010
                            },
                            new TeamEntity
                            {
                                Id = 22,
                                TeamGameId = 201922,
                                Name = SeedNames.Ferrari1976
                            },
                            new TeamEntity
                            {
                                Id = 23,
                                TeamGameId = 201923,
                                Name = SeedNames.ARTGrandPrix
                            },
                            new TeamEntity
                            {
                                Id = 24,
                                TeamGameId = 201924,
                                Name = SeedNames.CamposVexatecRacing
                            },
                            new TeamEntity
                            {
                                Id = 25,
                                TeamGameId = 201925,
                                Name = SeedNames.Carlin
                            },
                            new TeamEntity
                            {
                                Id = 26,
                                TeamGameId = 201926,
                                Name = SeedNames.CharouzRacingSystem
                            },
                            new TeamEntity
                            {
                                Id = 27,
                                TeamGameId = 201927,
                                Name = SeedNames.DAMS
                            },
                            new TeamEntity
                            {
                                Id = 28,
                                TeamGameId = 201928,
                                Name = SeedNames.RussianTime
                            },
                            new TeamEntity
                            {
                                Id = 29,
                                TeamGameId = 201929,
                                Name = SeedNames.MPMotorsport
                            },
                            new TeamEntity
                            {
                                Id = 30,
                                TeamGameId = 201930,
                                Name = SeedNames.Pertamina
                            },
                            new TeamEntity
                            {
                                Id = 31,
                                TeamGameId = 201931,
                                Name = SeedNames.McLaren1990
                            },
                            new TeamEntity
                            {
                                Id = 32,
                                TeamGameId = 201932,
                                Name = SeedNames.Trident
                            },
                            new TeamEntity
                            {
                                Id = 33,
                                TeamGameId = 201933,
                                Name = SeedNames.BWTArden
                            },
                            new TeamEntity
                            {
                                Id = 34,
                                TeamGameId = 201934,
                                Name = SeedNames.McLaren1976
                            },
                            new TeamEntity
                            {
                                Id = 35,
                                TeamGameId = 201935,
                                Name = SeedNames.Lotus1972
                            },
                            new TeamEntity
                            {
                                Id = 36,
                                TeamGameId = 201936,
                                Name = SeedNames.Ferrari1979
                            },
                            new TeamEntity
                            {
                                Id = 37,
                                TeamGameId = 201937,
                                Name = SeedNames.McLaren1982
                            },
                            new TeamEntity
                            {
                                Id = 38,
                                TeamGameId = 201938,
                                Name = SeedNames.Williams2003
                            },
                            new TeamEntity
                            {
                                Id = 39,
                                TeamGameId = 201939,
                                Name = SeedNames.Brawn2009
                            },
                            new TeamEntity
                            {
                                Id = 40,
                                TeamGameId = 201940,
                                Name = SeedNames.Lotus1978
                            },
                            new TeamEntity
                            {
                                Id = 41,
                                TeamGameId = 201942,
                                Name = SeedNames.ArtGP19
                            },
                            new TeamEntity
                            {
                                Id = 42,
                                TeamGameId = 201943,
                                Name = SeedNames.Campos19
                            },
                            new TeamEntity
                            {
                                Id = 43,
                                TeamGameId = 201944,
                                Name = SeedNames.Carlin19
                            },
                            new TeamEntity
                            {
                                Id = 44,
                                TeamGameId = 201945,
                                Name = SeedNames.SauberJuniorCharouz19
                            },
                            new TeamEntity
                            {
                                Id = 45,
                                TeamGameId = 201946,
                                Name = SeedNames.Dams19
                            },
                            new TeamEntity
                            {
                                Id = 46,
                                TeamGameId = 201947,
                                Name = SeedNames.UniVirtuosi19
                            },
                            new TeamEntity
                            {
                                Id = 47,
                                TeamGameId = 201948,
                                Name = SeedNames.MPMotorsport19
                            },
                            new TeamEntity
                            {
                                Id = 48,
                                TeamGameId = 201949,
                                Name = SeedNames.Prema19
                            },
                            new TeamEntity
                            {
                                Id = 49,
                                TeamGameId = 201950,
                                Name = SeedNames.Trident19
                            },
                            new TeamEntity
                            {
                                Id = 50,
                                TeamGameId = 201951,
                                Name = SeedNames.Arden19
                            },
                            new TeamEntity
                            {
                                Id = 51,
                                TeamGameId = 201963,
                                Name = SeedNames.Ferrari1990
                            },
                            new TeamEntity
                            {
                                Id = 52,
                                TeamGameId = 201964,
                                Name = SeedNames.McLaren2010
                            },
                            new TeamEntity
                            {
                                Id = 53,
                                TeamGameId = 201965,
                                Name = SeedNames.Ferrari2010
                            },
                            new TeamEntity
                            {
                                Id = 54,
                                TeamGameId = 20200,
                                Name = SeedNames.Mercedes
                            },
                            new TeamEntity
                            {
                                Id = 55,
                                TeamGameId = 20201,
                                Name = SeedNames.Ferrari
                            },
                            new TeamEntity
                            {
                                Id = 56,
                                TeamGameId = 20202,
                                Name = SeedNames.RedBullRacing
                            },
                            new TeamEntity
                            {
                                Id = 57,
                                TeamGameId = 20203,
                                Name = SeedNames.Williams
                            },
                            new TeamEntity
                            {
                                Id = 58,
                                TeamGameId = 20204,
                                Name = SeedNames.RacingPoint
                            },
                            new TeamEntity
                            {
                                Id = 59,
                                TeamGameId = 20205,
                                Name = SeedNames.Renault
                            },
                            new TeamEntity
                            {
                                Id = 60,
                                TeamGameId = 20206,
                                Name = SeedNames.AlphaTauri
                            },
                            new TeamEntity
                            {
                                Id = 61,
                                TeamGameId = 20207,
                                Name = SeedNames.Haas
                            },
                            new TeamEntity
                            {
                                Id = 62,
                                TeamGameId = 20208,
                                Name = SeedNames.McLaren
                            },
                            new TeamEntity
                            {
                                Id = 63,
                                TeamGameId = 20209,
                                Name = SeedNames.AlfaRomeo
                            },
                            new TeamEntity
                            {
                                Id = 64,
                                TeamGameId = 202010,
                                Name = SeedNames.McLaren1988
                            },
                            new TeamEntity
                            {
                                Id = 65,
                                TeamGameId = 202011,
                                Name = SeedNames.McLaren1991
                            },
                            new TeamEntity
                            {
                                Id = 66,
                                TeamGameId = 202012,
                                Name = SeedNames.Williams1992
                            },
                            new TeamEntity
                            {
                                Id = 67,
                                TeamGameId = 202013,
                                Name = SeedNames.Ferrari1995
                            },
                            new TeamEntity
                            {
                                Id = 68,
                                TeamGameId = 202014,
                                Name = SeedNames.Williams1996
                            },
                            new TeamEntity
                            {
                                Id = 69,
                                TeamGameId = 202015,
                                Name = SeedNames.McLaren1998
                            },
                            new TeamEntity
                            {
                                Id = 70,
                                TeamGameId = 202016,
                                Name = SeedNames.Ferrari2002
                            },
                            new TeamEntity
                            {
                                Id = 71,
                                TeamGameId = 202017,
                                Name = SeedNames.Ferrari2004
                            },
                            new TeamEntity
                            {
                                Id = 72,
                                TeamGameId = 202018,
                                Name = SeedNames.Renault2006
                            },
                            new TeamEntity
                            {
                                Id = 73,
                                TeamGameId = 202019,
                                Name = SeedNames.Ferrari2007
                            },
                            new TeamEntity
                            {
                                Id = 74,
                                TeamGameId = 202020,
                                Name = SeedNames.McLaren2008
                            },
                            new TeamEntity
                            {
                                Id = 75,
                                TeamGameId = 202021,
                                Name = SeedNames.RedBull2010
                            },
                            new TeamEntity
                            {
                                Id = 76,
                                TeamGameId = 202022,
                                Name = SeedNames.Ferrari1976
                            },
                            new TeamEntity
                            {
                                Id = 77,
                                TeamGameId = 202023,
                                Name = SeedNames.ARTGrandPrix
                            },
                            new TeamEntity
                            {
                                Id = 78,
                                TeamGameId = 202024,
                                Name = SeedNames.CamposVexatecRacing
                            },
                            new TeamEntity
                            {
                                Id = 79,
                                TeamGameId = 202025,
                                Name = SeedNames.Carlin
                            },
                            new TeamEntity
                            {
                                Id = 80,
                                TeamGameId = 202026,
                                Name = SeedNames.CharouzRacingSystem
                            },
                            new TeamEntity
                            {
                                Id = 81,
                                TeamGameId = 202027,
                                Name = SeedNames.DAMS
                            },
                            new TeamEntity
                            {
                                Id = 82,
                                TeamGameId = 202028,
                                Name = SeedNames.RussianTime
                            },
                            new TeamEntity
                            {
                                Id = 83,
                                TeamGameId = 202029,
                                Name = SeedNames.MPMotorsport
                            },
                            new TeamEntity
                            {
                                Id = 84,
                                TeamGameId = 202030,
                                Name = SeedNames.Pertamina
                            },
                            new TeamEntity
                            {
                                Id = 85,
                                TeamGameId = 202031,
                                Name = SeedNames.McLaren1990
                            },
                            new TeamEntity
                            {
                                Id = 86,
                                TeamGameId = 202032,
                                Name = SeedNames.Trident
                            },
                            new TeamEntity
                            {
                                Id = 87,
                                TeamGameId = 202033,
                                Name = SeedNames.BWTArden
                            },
                            new TeamEntity
                            {
                                Id = 88,
                                TeamGameId = 202034,
                                Name = SeedNames.McLaren1976
                            },
                            new TeamEntity
                            {
                                Id = 89,
                                TeamGameId = 202035,
                                Name = SeedNames.Lotus1972
                            },
                            new TeamEntity
                            {
                                Id = 90,
                                TeamGameId = 202036,
                                Name = SeedNames.Ferrari1979
                            },
                            new TeamEntity
                            {
                                Id = 91,
                                TeamGameId = 202037,
                                Name = SeedNames.McLaren1982
                            },
                            new TeamEntity
                            {
                                Id = 92,
                                TeamGameId = 202038,
                                Name = SeedNames.Williams2003
                            },
                            new TeamEntity
                            {
                                Id = 93,
                                TeamGameId = 202039,
                                Name = SeedNames.Brawn2009
                            },
                            new TeamEntity
                            {
                                Id = 94,
                                TeamGameId = 202040,
                                Name = SeedNames.Lotus1978
                            },
                            new TeamEntity
                            {
                                Id = 95,
                                TeamGameId = 202041,
                                Name = SeedNames.F1Generic
                            },
                            new TeamEntity
                            {
                                Id = 96,
                                TeamGameId = 202042,
                                Name = SeedNames.ArtGP19
                            },
                            new TeamEntity
                            {
                                Id = 97,
                                TeamGameId = 202043,
                                Name = SeedNames.Campos19
                            },
                            new TeamEntity
                            {
                                Id = 98,
                                TeamGameId = 202044,
                                Name = SeedNames.Carlin19
                            },
                            new TeamEntity
                            {
                                Id = 99,
                                TeamGameId = 202045,
                                Name = SeedNames.SauberJuniorCharouz19
                            },
                            new TeamEntity
                            {
                                Id = 100,
                                TeamGameId = 202046,
                                Name = SeedNames.Dams19
                            },
                            new TeamEntity
                            {
                                Id = 101,
                                TeamGameId = 202047,
                                Name = SeedNames.UniVirtuosi19
                            },
                            new TeamEntity
                            {
                                Id = 102,
                                TeamGameId = 202048,
                                Name = SeedNames.MPMotorsport19
                            },
                            new TeamEntity
                            {
                                Id = 103,
                                TeamGameId = 202049,
                                Name = SeedNames.Prema19
                            },
                            new TeamEntity
                            {
                                Id = 104,
                                TeamGameId = 202050,
                                Name = SeedNames.Trident19
                            },
                            new TeamEntity
                            {
                                Id = 105,
                                TeamGameId = 202051,
                                Name = SeedNames.Arden19
                            },
                            new TeamEntity
                            {
                                Id = 106,
                                TeamGameId = 202053,
                                Name = SeedNames.Benetton1994
                            },
                            new TeamEntity
                            {
                                Id = 107,
                                TeamGameId = 202054,
                                Name = SeedNames.Benetton1995
                            },
                            new TeamEntity
                            {
                                Id = 108,
                                TeamGameId = 202055,
                                Name = SeedNames.Ferrari2000
                            },
                            new TeamEntity
                            {
                                Id = 109,
                                TeamGameId = 202056,
                                Name = SeedNames.Jordan1991
                            },
                            new TeamEntity
                            {
                                Id = 110,
                                TeamGameId = 2020255,
                                Name = SeedNames.MyTeam20
                            },
                            new TeamEntity
                            {
                                Id = 111,
                                TeamGameId = 20210,
                                Name = SeedNames.Mercedes
                            },
                            new TeamEntity
                            {
                                Id = 112,
                                TeamGameId = 20211,
                                Name = SeedNames.Ferrari
                            },
                            new TeamEntity
                            {
                                Id = 113,
                                TeamGameId = 20212,
                                Name = SeedNames.RedBullRacing
                            },
                            new TeamEntity
                            {
                                Id = 114,
                                TeamGameId = 20213,
                                Name = SeedNames.Williams
                            },
                            new TeamEntity
                            {
                                Id = 115,
                                TeamGameId = 20214,
                                Name = SeedNames.AstonMartin
                            },
                            new TeamEntity
                            {
                                Id = 116,
                                TeamGameId = 20215,
                                Name = SeedNames.Alpine
                            },
                            new TeamEntity
                            {
                                Id = 117,
                                TeamGameId = 20216,
                                Name = SeedNames.AlphaTauri
                            },
                            new TeamEntity
                            {
                                Id = 118,
                                TeamGameId = 20217,
                                Name = SeedNames.Haas
                            },
                            new TeamEntity
                            {
                                Id = 119,
                                TeamGameId = 20218,
                                Name = SeedNames.McLaren
                            },
                            new TeamEntity
                            {
                                Id = 120,
                                TeamGameId = 20219,
                                Name = SeedNames.AlfaRomeo
                            },
                            new TeamEntity
                            {
                                Id = 121,
                                TeamGameId = 202142,
                                Name = SeedNames.ArtGP19
                            },
                            new TeamEntity
                            {
                                Id = 122,
                                TeamGameId = 202143,
                                Name = SeedNames.Campos19
                            },
                            new TeamEntity
                            {
                                Id = 123,
                                TeamGameId = 202144,
                                Name = SeedNames.Carlin19
                            },
                            new TeamEntity
                            {
                                Id = 124,
                                TeamGameId = 202145,
                                Name = SeedNames.SauberJuniorCharouz19
                            },
                            new TeamEntity
                            {
                                Id = 125,
                                TeamGameId = 202146,
                                Name = SeedNames.Dams19
                            },
                            new TeamEntity
                            {
                                Id = 126,
                                TeamGameId = 202147,
                                Name = SeedNames.UniVirtuosi19
                            },
                            new TeamEntity
                            {
                                Id = 127,
                                TeamGameId = 202148,
                                Name = SeedNames.MPMotorsport19
                            },
                            new TeamEntity
                            {
                                Id = 128,
                                TeamGameId = 202149,
                                Name = SeedNames.Prema19
                            },
                            new TeamEntity
                            {
                                Id = 129,
                                TeamGameId = 202150,
                                Name = SeedNames.Trident19
                            },
                            new TeamEntity
                            {
                                Id = 130,
                                TeamGameId = 202151,
                                Name = SeedNames.Arden19
                            },
                            new TeamEntity
                            {
                                Id = 131,
                                TeamGameId = 202170,
                                Name = SeedNames.ArtGP20
                            },
                            new TeamEntity
                            {
                                Id = 132,
                                TeamGameId = 202171,
                                Name = SeedNames.Campos20
                            },
                            new TeamEntity
                            {
                                Id = 133,
                                TeamGameId = 202172,
                                Name = SeedNames.Carlin20
                            },
                            new TeamEntity
                            {
                                Id = 134,
                                TeamGameId = 202173,
                                Name = SeedNames.Charouz20
                            },
                            new TeamEntity
                            {
                                Id = 135,
                                TeamGameId = 202174,
                                Name = SeedNames.Dams20
                            },
                            new TeamEntity
                            {
                                Id = 136,
                                TeamGameId = 202175,
                                Name = SeedNames.UniVirtuosi20
                            },
                            new TeamEntity
                            {
                                Id = 137,
                                TeamGameId = 202176,
                                Name = SeedNames.MPMotorsport20
                            },
                            new TeamEntity
                            {
                                Id = 138,
                                TeamGameId = 202177,
                                Name = SeedNames.Prema20
                            },
                            new TeamEntity
                            {
                                Id = 139,
                                TeamGameId = 202178,
                                Name = SeedNames.Trident20
                            },
                            new TeamEntity
                            {
                                Id = 140,
                                TeamGameId = 202179,
                                Name = SeedNames.BWT20
                            },
                            new TeamEntity
                            {
                                Id = 141,
                                TeamGameId = 202180,
                                Name = SeedNames.Hitech20
                            },
                            new TeamEntity
                            {
                                Id = 142,
                                TeamGameId = 202185,
                                Name = SeedNames.Mercedes2020
                            },
                            new TeamEntity
                            {
                                Id = 143,
                                TeamGameId = 202186,
                                Name = SeedNames.Ferrari2020
                            },
                            new TeamEntity
                            {
                                Id = 144,
                                TeamGameId = 202187,
                                Name = SeedNames.RedBull2020
                            },
                            new TeamEntity
                            {
                                Id = 145,
                                TeamGameId = 202188,
                                Name = SeedNames.Williams2020
                            },
                            new TeamEntity
                            {
                                Id = 146,
                                TeamGameId = 202189,
                                Name = SeedNames.RacingPoint2020
                            },
                            new TeamEntity
                            {
                                Id = 147,
                                TeamGameId = 202190,
                                Name = SeedNames.Renault2020
                            },
                            new TeamEntity
                            {
                                Id = 148,
                                TeamGameId = 202191,
                                Name = SeedNames.AlphaTauri2020
                            },
                            new TeamEntity
                            {
                                Id = 149,
                                TeamGameId = 202192,
                                Name = SeedNames.Haas2020
                            },
                            new TeamEntity
                            {
                                Id = 150,
                                TeamGameId = 202193,
                                Name = SeedNames.McLaren2020
                            },
                            new TeamEntity
                            {
                                Id = 151,
                                TeamGameId = 202194,
                                Name = SeedNames.AlfaRomeo2020
                            },
                            new TeamEntity
                            {
                                Id = 152,
                                TeamGameId = 2021106,
                                Name = SeedNames.Prema21
                            },
                            new TeamEntity
                            {
                                Id = 153,
                                TeamGameId = 2021107,
                                Name = SeedNames.UniVirtuosi21
                            },
                            new TeamEntity
                            {
                                Id = 154,
                                TeamGameId = 2021108,
                                Name = SeedNames.Carlin21
                            },
                            new TeamEntity
                            {
                                Id = 155,
                                TeamGameId = 2021109,
                                Name = SeedNames.Hitech21
                            },
                            new TeamEntity
                            {
                                Id = 156,
                                TeamGameId = 2021110,
                                Name = SeedNames.ArtGP21
                            },
                            new TeamEntity
                            {
                                Id = 157,
                                TeamGameId = 2021111,
                                Name = SeedNames.MPMotorsport21
                            },
                            new TeamEntity
                            {
                                Id = 158,
                                TeamGameId = 2021112,
                                Name = SeedNames.Charouz21
                            },
                            new TeamEntity
                            {
                                Id = 159,
                                TeamGameId = 2021113,
                                Name = SeedNames.Dams21
                            },
                            new TeamEntity
                            {
                                Id = 160,
                                TeamGameId = 2021114,
                                Name = SeedNames.Campos21
                            },
                            new TeamEntity
                            {
                                Id = 161,
                                TeamGameId = 2021115,
                                Name = SeedNames.BWT21
                            },
                            new TeamEntity
                            {
                                Id = 162,
                                TeamGameId = 2021116,
                                Name = SeedNames.Trident21
                            },
                            new TeamEntity
                            {
                                Id = 163,
                                TeamGameId = 20220,
                                Name = SeedNames.Mercedes
                            },
                            new TeamEntity
                            {
                                Id = 164,
                                TeamGameId = 20221,
                                Name = SeedNames.Ferrari
                            },
                            new TeamEntity
                            {
                                Id = 165,
                                TeamGameId = 20222,
                                Name = SeedNames.RedBullRacing
                            },
                            new TeamEntity
                            {
                                Id = 166,
                                TeamGameId = 20223,
                                Name = SeedNames.Williams
                            },
                            new TeamEntity
                            {
                                Id = 167,
                                TeamGameId = 20224,
                                Name = SeedNames.AstonMartin
                            },
                            new TeamEntity
                            {
                                Id = 168,
                                TeamGameId = 20225,
                                Name = SeedNames.Alpine
                            },
                            new TeamEntity
                            {
                                Id = 169,
                                TeamGameId = 20226,
                                Name = SeedNames.AlphaTauri
                            },
                            new TeamEntity
                            {
                                Id = 170,
                                TeamGameId = 20227,
                                Name = SeedNames.Haas
                            },
                            new TeamEntity
                            {
                                Id = 171,
                                TeamGameId = 20228,
                                Name = SeedNames.McLaren
                            },
                            new TeamEntity
                            {
                                Id = 172,
                                TeamGameId = 20229,
                                Name = SeedNames.AlfaRomeo
                            },
                            new TeamEntity
                            {
                                Id = 173,
                                TeamGameId = 202285,
                                Name = SeedNames.Mercedes2020
                            },
                            new TeamEntity
                            {
                                Id = 174,
                                TeamGameId = 202286,
                                Name = SeedNames.Ferrari2020
                            },
                            new TeamEntity
                            {
                                Id = 175,
                                TeamGameId = 202287,
                                Name = SeedNames.RedBull2020
                            },
                            new TeamEntity
                            {
                                Id = 176,
                                TeamGameId = 202288,
                                Name = SeedNames.Williams2020
                            },
                            new TeamEntity
                            {
                                Id = 177,
                                TeamGameId = 202289,
                                Name = SeedNames.RacingPoint2020
                            },
                            new TeamEntity
                            {
                                Id = 178,
                                TeamGameId = 202290,
                                Name = SeedNames.Renault2020
                            },
                            new TeamEntity
                            {
                                Id = 179,
                                TeamGameId = 202291,
                                Name = SeedNames.AlphaTauri2020
                            },
                            new TeamEntity
                            {
                                Id = 180,
                                TeamGameId = 202292,
                                Name = SeedNames.Haas2020
                            },
                            new TeamEntity
                            {
                                Id = 181,
                                TeamGameId = 202293,
                                Name = SeedNames.McLaren2020
                            },
                            new TeamEntity
                            {
                                Id = 182,
                                TeamGameId = 202294,
                                Name = SeedNames.AlfaRomeo2020
                            },
                            new TeamEntity
                            {
                                Id = 183,
                                TeamGameId = 202295,
                                Name = SeedNames.AstonMartinDB11V12
                            },
                            new TeamEntity
                            {
                                Id = 184,
                                TeamGameId = 202296,
                                Name = SeedNames.AstonMartinVantageF1Edition
                            },
                            new TeamEntity
                            {
                                Id = 185,
                                TeamGameId = 202297,
                                Name = SeedNames.AstonMartinVantageSafetyCar
                            },
                            new TeamEntity
                            {
                                Id = 186,
                                TeamGameId = 202298,
                                Name = SeedNames.FerrariF8Tributo
                            },
                            new TeamEntity
                            {
                                Id = 187,
                                TeamGameId = 202299,
                                Name = SeedNames.FerrariRoma
                            },
                            new TeamEntity
                            {
                                Id = 188,
                                TeamGameId = 2022100,
                                Name = SeedNames.McLaren720S
                            },
                            new TeamEntity
                            {
                                Id = 189,
                                TeamGameId = 2022101,
                                Name = SeedNames.McLarenArtura
                            },
                            new TeamEntity
                            {
                                Id = 190,
                                TeamGameId = 2022102,
                                Name = SeedNames.MercedesAMGGTBlackSeriesSafetyCar
                            },
                            new TeamEntity
                            {
                                Id = 191,
                                TeamGameId = 2022103,
                                Name = SeedNames.MercedesAMGGTRPro
                            },
                            new TeamEntity
                            {
                                Id = 192,
                                TeamGameId = 2022104,
                                Name = SeedNames.F1CustomTeam
                            },
                            new TeamEntity
                            {
                                Id = 193,
                                TeamGameId = 2022106,
                                Name = SeedNames.Prema21
                            },
                            new TeamEntity
                            {
                                Id = 194,
                                TeamGameId = 2022107,
                                Name = SeedNames.FerrariUniVirtuosi21
                            },
                            new TeamEntity
                            {
                                Id = 195,
                                TeamGameId = 2022108,
                                Name = SeedNames.Carlin21
                            },
                            new TeamEntity
                            {
                                Id = 196,
                                TeamGameId = 2022109,
                                Name = SeedNames.Hitech21
                            },
                            new TeamEntity
                            {
                                Id = 197,
                                TeamGameId = 2022110,
                                Name = SeedNames.ArtGP21
                            },
                            new TeamEntity
                            {
                                Id = 198,
                                TeamGameId = 2022111,
                                Name = SeedNames.MPMotorsport21
                            },
                            new TeamEntity
                            {
                                Id = 199,
                                TeamGameId = 2022112,
                                Name = SeedNames.Charouz21
                            },
                            new TeamEntity
                            {
                                Id = 200,
                                TeamGameId = 2022113,
                                Name = SeedNames.Dams21
                            },
                            new TeamEntity
                            {
                                Id = 201,
                                TeamGameId = 2022114,
                                Name = SeedNames.Campos21
                            },
                            new TeamEntity
                            {
                                Id = 202,
                                TeamGameId = 2022115,
                                Name = SeedNames.BWT21
                            },
                            new TeamEntity
                            {
                                Id = 203,
                                TeamGameId = 2022116,
                                Name = SeedNames.Trident21
                            },
                            new TeamEntity
                            {
                                Id = 204,
                                TeamGameId = 2022117,
                                Name = SeedNames.MercedesAMGGTBlackSeries
                            },
                            new TeamEntity
                            {
                                Id = 205,
                                TeamGameId = 2022118,
                                Name = SeedNames.Prema22
                            },
                            new TeamEntity
                            {
                                Id = 206,
                                TeamGameId = 2022119,
                                Name = SeedNames.Virtuosi22
                            },
                            new TeamEntity
                            {
                                Id = 207,
                                TeamGameId = 2022120,
                                Name = SeedNames.Carlin22
                            },
                            new TeamEntity
                            {
                                Id = 208,
                                TeamGameId = 2022121,
                                Name = SeedNames.Hitech22
                            },
                            new TeamEntity
                            {
                                Id = 209,
                                TeamGameId = 2022122,
                                Name = SeedNames.ArtGP22
                            },
                            new TeamEntity
                            {
                                Id = 210,
                                TeamGameId = 2022123,
                                Name = SeedNames.MPMotorsport22
                            },
                            new TeamEntity
                            {
                                Id = 211,
                                TeamGameId = 2022124,
                                Name = SeedNames.Charouz22
                            },
                            new TeamEntity
                            {
                                Id = 212,
                                TeamGameId = 2022125,
                                Name = SeedNames.Dams22
                            },
                            new TeamEntity
                            {
                                Id = 213,
                                TeamGameId = 2022126,
                                Name = SeedNames.Campos22
                            },
                            new TeamEntity
                            {
                                Id = 214,
                                TeamGameId = 2022127,
                                Name = SeedNames.VanAmersfoortRacing22
                            },
                            new TeamEntity
                            {
                                Id = 215,
                                TeamGameId = 2022128,
                                Name = SeedNames.Trident22
                            },
                            new TeamEntity
                            {
                                Id = 216,
                                TeamGameId = 20230,
                                Name = SeedNames.Mercedes
                            },
                            new TeamEntity
                            {
                                Id = 217,
                                TeamGameId = 20231,
                                Name = SeedNames.Ferrari
                            },
                            new TeamEntity
                            {
                                Id = 218,
                                TeamGameId = 20232,
                                Name = SeedNames.RedBullRacing
                            },
                            new TeamEntity
                            {
                                Id = 219,
                                TeamGameId = 20233,
                                Name = SeedNames.Williams
                            },
                            new TeamEntity
                            {
                                Id = 220,
                                TeamGameId = 20234,
                                Name = SeedNames.AstonMartin
                            },
                            new TeamEntity
                            {
                                Id = 221,
                                TeamGameId = 20235,
                                Name = SeedNames.Alpine
                            },
                            new TeamEntity
                            {
                                Id = 222,
                                TeamGameId = 20236,
                                Name = SeedNames.RB
                            },
                            new TeamEntity
                            {
                                Id = 223,
                                TeamGameId = 20237,
                                Name = SeedNames.Haas
                            },
                            new TeamEntity
                            {
                                Id = 224,
                                TeamGameId = 20238,
                                Name = SeedNames.McLaren
                            },
                            new TeamEntity
                            {
                                Id = 225,
                                TeamGameId = 20239,
                                Name = SeedNames.AlfaRomeo
                            },
                            new TeamEntity
                            {
                                Id = 226,
                                TeamGameId = 202385,
                                Name = SeedNames.Mercedes2020
                            },
                            new TeamEntity
                            {
                                Id = 227,
                                TeamGameId = 202386,
                                Name = SeedNames.Ferrari2020
                            },
                            new TeamEntity
                            {
                                Id = 228,
                                TeamGameId = 202387,
                                Name = SeedNames.RedBull2020
                            },
                            new TeamEntity
                            {
                                Id = 229,
                                TeamGameId = 202388,
                                Name = SeedNames.Williams2020
                            },
                            new TeamEntity
                            {
                                Id = 230,
                                TeamGameId = 202389,
                                Name = SeedNames.RacingPoint2020
                            },
                            new TeamEntity
                            {
                                Id = 231,
                                TeamGameId = 202390,
                                Name = SeedNames.Renault2020
                            },
                            new TeamEntity
                            {
                                Id = 232,
                                TeamGameId = 202391,
                                Name = SeedNames.AlphaTauri2020
                            },
                            new TeamEntity
                            {
                                Id = 233,
                                TeamGameId = 202392,
                                Name = SeedNames.Haas2020
                            },
                            new TeamEntity
                            {
                                Id = 234,
                                TeamGameId = 202393,
                                Name = SeedNames.McLaren2020
                            },
                            new TeamEntity
                            {
                                Id = 235,
                                TeamGameId = 202394,
                                Name = SeedNames.AlfaRomeo2020
                            },
                            new TeamEntity
                            {
                                Id = 236,
                                TeamGameId = 202395,
                                Name = SeedNames.AstonMartinDB11V12
                            },
                            new TeamEntity
                            {
                                Id = 237,
                                TeamGameId = 202396,
                                Name = SeedNames.AstonMartinVantageF1Edition
                            },
                            new TeamEntity
                            {
                                Id = 238,
                                TeamGameId = 202397,
                                Name = SeedNames.AstonMartinVantageSafetyCar
                            },
                            new TeamEntity
                            {
                                Id = 239,
                                TeamGameId = 202398,
                                Name = SeedNames.FerrariF8Tributo
                            },
                            new TeamEntity
                            {
                                Id = 240,
                                TeamGameId = 202399,
                                Name = SeedNames.FerrariRoma
                            },
                            new TeamEntity
                            {
                                Id = 241,
                                TeamGameId = 2023100,
                                Name = SeedNames.McLaren720S
                            },
                            new TeamEntity
                            {
                                Id = 242,
                                TeamGameId = 2023101,
                                Name = SeedNames.McLarenArtura
                            },
                            new TeamEntity
                            {
                                Id = 243,
                                TeamGameId = 2023102,
                                Name = SeedNames.MercedesAMGGTBlackSeriesSafetyCar
                            },
                            new TeamEntity
                            {
                                Id = 244,
                                TeamGameId = 2023103,
                                Name = SeedNames.MercedesAMGGTRPro
                            },
                            new TeamEntity
                            {
                                Id = 245,
                                TeamGameId = 2023104,
                                Name = SeedNames.F1CustomTeam
                            },
                            new TeamEntity
                            {
                                Id = 246,
                                TeamGameId = 2023106,
                                Name = SeedNames.Prema21
                            },
                            new TeamEntity
                            {
                                Id = 247,
                                TeamGameId = 2023107,
                                Name = SeedNames.FerrariUniVirtuosi21
                            },
                            new TeamEntity
                            {
                                Id = 248,
                                TeamGameId = 2023108,
                                Name = SeedNames.Carlin21
                            },
                            new TeamEntity
                            {
                                Id = 249,
                                TeamGameId = 2023109,
                                Name = SeedNames.Hitech21
                            },
                            new TeamEntity
                            {
                                Id = 250,
                                TeamGameId = 2023110,
                                Name = SeedNames.ArtGP21
                            },
                            new TeamEntity
                            {
                                Id = 251,
                                TeamGameId = 2023111,
                                Name = SeedNames.MPMotorsport21
                            },
                            new TeamEntity
                            {
                                Id = 252,
                                TeamGameId = 2023112,
                                Name = SeedNames.Charouz21
                            },
                            new TeamEntity
                            {
                                Id = 253,
                                TeamGameId = 2023113,
                                Name = SeedNames.Dams21
                            },
                            new TeamEntity
                            {
                                Id = 254,
                                TeamGameId = 2023114,
                                Name = SeedNames.Campos21
                            },
                            new TeamEntity
                            {
                                Id = 255,
                                TeamGameId = 2023115,
                                Name = SeedNames.BWT21
                            },
                            new TeamEntity
                            {
                                Id = 256,
                                TeamGameId = 2023116,
                                Name = SeedNames.Trident21
                            },
                            new TeamEntity
                            {
                                Id = 257,
                                TeamGameId = 2023117,
                                Name = SeedNames.MercedesAMGGTBlackSeries
                            },
                            new TeamEntity
                            {
                                Id = 258,
                                TeamGameId = 2023118,
                                Name = SeedNames.Mercedes22
                            },
                            new TeamEntity
                            {
                                Id = 259,
                                TeamGameId = 2023119,
                                Name = SeedNames.Ferrari22
                            },
                            new TeamEntity
                            {
                                Id = 260,
                                TeamGameId = 2023120,
                                Name = SeedNames.RedBullRacing22
                            },
                            new TeamEntity
                            {
                                Id = 261,
                                TeamGameId = 2023121,
                                Name = SeedNames.Williams22
                            },
                            new TeamEntity
                            {
                                Id = 262,
                                TeamGameId = 2023122,
                                Name = SeedNames.AstonMartin22
                            },
                            new TeamEntity
                            {
                                Id = 263,
                                TeamGameId = 2023123,
                                Name = SeedNames.Alpine22
                            },
                            new TeamEntity
                            {
                                Id = 264,
                                TeamGameId = 2023124,
                                Name = SeedNames.AlphaTauri22
                            },
                            new TeamEntity
                            {
                                Id = 265,
                                TeamGameId = 2023125,
                                Name = SeedNames.Haas22
                            },
                            new TeamEntity
                            {
                                Id = 266,
                                TeamGameId = 2023126,
                                Name = SeedNames.McLaren22
                            },
                            new TeamEntity
                            {
                                Id = 267,
                                TeamGameId = 2023127,
                                Name = SeedNames.AlfaRomeo22
                            },
                            new TeamEntity
                            {
                                Id = 268,
                                TeamGameId = 2023128,
                                Name = SeedNames.Konnersport22
                            },
                            new TeamEntity
                            {
                                Id = 269,
                                TeamGameId = 2023129,
                                Name = SeedNames.Konnersport
                            },
                            new TeamEntity
                            {
                                Id = 270,
                                TeamGameId = 2023130,
                                Name = SeedNames.Prema22
                            },
                            new TeamEntity
                            {
                                Id = 271,
                                TeamGameId = 2023131,
                                Name = SeedNames.Virtuosi22
                            },
                            new TeamEntity
                            {
                                Id = 272,
                                TeamGameId = 2023132,
                                Name = SeedNames.Carlin22
                            },
                            new TeamEntity
                            {
                                Id = 273,
                                TeamGameId = 2023133,
                                Name = SeedNames.MPMotorsport22
                            },
                            new TeamEntity
                            {
                                Id = 274,
                                TeamGameId = 2023134,
                                Name = SeedNames.Charouz22
                            },
                            new TeamEntity
                            {
                                Id = 275,
                                TeamGameId = 2023135,
                                Name = SeedNames.Dams22
                            },
                            new TeamEntity
                            {
                                Id = 276,
                                TeamGameId = 2023136,
                                Name = SeedNames.Campos22
                            },
                            new TeamEntity
                            {
                                Id = 277,
                                TeamGameId = 2023137,
                                Name = SeedNames.VanAmersfoortRacing22
                            },
                            new TeamEntity
                            {
                                Id = 278,
                                TeamGameId = 2023138,
                                Name = SeedNames.Trident22
                            },
                            new TeamEntity
                            {
                                Id = 279,
                                TeamGameId = 2023139,
                                Name = SeedNames.Hitech22
                            },
                            new TeamEntity
                            {
                                Id = 280,
                                TeamGameId = 2023140,
                                Name = SeedNames.ArtGP22
                            },
                            new TeamEntity
                            {
                                Id = 281,
                                TeamGameId = 20240,
                                Name = SeedNames.Mercedes
                            },
                            new TeamEntity
                            {
                                Id = 282,
                                TeamGameId = 20241,
                                Name = SeedNames.Ferrari
                            },
                            new TeamEntity
                            {
                                Id = 283,
                                TeamGameId = 20242,
                                Name = SeedNames.RedBullRacing
                            },
                            new TeamEntity
                            {
                                Id = 284,
                                TeamGameId = 20243,
                                Name = SeedNames.Williams
                            },
                            new TeamEntity
                            {
                                Id = 285,
                                TeamGameId = 20244,
                                Name = SeedNames.AstonMartin
                            },
                            new TeamEntity
                            {
                                Id = 286,
                                TeamGameId = 20245,
                                Name = SeedNames.Alpine
                            },
                            new TeamEntity
                            {
                                Id = 287,
                                TeamGameId = 20246,
                                Name = SeedNames.RB
                            },
                            new TeamEntity
                            {
                                Id = 288,
                                TeamGameId = 20247,
                                Name = SeedNames.Haas
                            },
                            new TeamEntity
                            {
                                Id = 289,
                                TeamGameId = 20248,
                                Name = SeedNames.McLaren
                            },
                            new TeamEntity
                            {
                                Id = 290,
                                TeamGameId = 20249,
                                Name = SeedNames.Sauber
                            },
                            new TeamEntity
                            {
                                Id = 291,
                                TeamGameId = 202441,
                                Name = SeedNames.F1Generic
                            },
                            new TeamEntity
                            {
                                Id = 292,
                                TeamGameId = 2024104,
                                Name = SeedNames.F1CustomTeam
                            },
                            new TeamEntity
                            {
                                Id = 293,
                                TeamGameId = 2024143,
                                Name = SeedNames.ArtGP23
                            },
                            new TeamEntity
                            {
                                Id = 294,
                                TeamGameId = 2024144,
                                Name = SeedNames.Campos23
                            },
                            new TeamEntity
                            {
                                Id = 295,
                                TeamGameId = 2024145,
                                Name = SeedNames.Carlin23
                            },
                            new TeamEntity
                            {
                                Id = 296,
                                TeamGameId = 2024146,
                                Name = SeedNames.PHM23
                            },
                            new TeamEntity
                            {
                                Id = 297,
                                TeamGameId = 2024147,
                                Name = SeedNames.Dams23
                            },
                            new TeamEntity
                            {
                                Id = 298,
                                TeamGameId = 2024148,
                                Name = SeedNames.Hitech23
                            },
                            new TeamEntity
                            {
                                Id = 299,
                                TeamGameId = 2024149,
                                Name = SeedNames.MPMotorsport23
                            },
                            new TeamEntity
                            {
                                Id = 300,
                                TeamGameId = 2024150,
                                Name = SeedNames.Prema23
                            },
                            new TeamEntity
                            {
                                Id = 301,
                                TeamGameId = 2024151,
                                Name = SeedNames.Trident23
                            },
                            new TeamEntity
                            {
                                Id = 302,
                                TeamGameId = 2024152,
                                Name = SeedNames.VanAmersfoortRacing23
                            },
                            new TeamEntity
                            {
                                Id = 303,
                                TeamGameId = 2024153,
                                Name = SeedNames.Virtuosi23
                            },
                            new TeamEntity
                            {
                                Id = 304,
                                TeamGameId = 20250,
                                Name = SeedNames.Mercedes
                            },
                            new TeamEntity
                            {
                                Id = 305,
                                TeamGameId = 20251,
                                Name = SeedNames.Ferrari
                            },
                            new TeamEntity
                            {
                                Id = 306,
                                TeamGameId = 20252,
                                Name = SeedNames.RedBullRacing
                            },
                            new TeamEntity
                            {
                                Id = 307,
                                TeamGameId = 20253,
                                Name = SeedNames.Williams
                            },
                            new TeamEntity
                            {
                                Id = 308,
                                TeamGameId = 20254,
                                Name = SeedNames.AstonMartin
                            },
                            new TeamEntity
                            {
                                Id = 309,
                                TeamGameId = 20255,
                                Name = SeedNames.Alpine
                            },
                            new TeamEntity
                            {
                                Id = 310,
                                TeamGameId = 20256,
                                Name = SeedNames.RB
                            },
                            new TeamEntity
                            {
                                Id = 311,
                                TeamGameId = 20257,
                                Name = SeedNames.Haas
                            },
                            new TeamEntity
                            {
                                Id = 312,
                                TeamGameId = 20258,
                                Name = SeedNames.McLaren
                            },
                            new TeamEntity
                            {
                                Id = 313,
                                TeamGameId = 20259,
                                Name = SeedNames.Sauber
                            },
                            new TeamEntity
                            {
                                Id = 314,
                                TeamGameId = 202541,
                                Name = SeedNames.F1Generic
                            },
                            new TeamEntity
                            {
                                Id = 315,
                                TeamGameId = 2025104,
                                Name = SeedNames.F1CustomTeam
                            },
                            new TeamEntity
                            {
                                Id = 316,
                                TeamGameId = 2025129,
                                Name = SeedNames.Konnersport
                            },
                            new TeamEntity
                            {
                                Id = 317,
                                TeamGameId = 2025142,
                                Name = SeedNames.APXGP24
                            },
                            new TeamEntity
                            {
                                Id = 318,
                                TeamGameId = 2025154,
                                Name = SeedNames.APXGP25
                            },
                            new TeamEntity
                            {
                                Id = 319,
                                TeamGameId = 2025155,
                                Name = SeedNames.Konnersport24
                            },
                            new TeamEntity
                            {
                                Id = 320,
                                TeamGameId = 2025158,
                                Name = SeedNames.ArtGP24
                            },
                            new TeamEntity
                            {
                                Id = 321,
                                TeamGameId = 2025159,
                                Name = SeedNames.Campos24
                            },
                            new TeamEntity
                            {
                                Id = 322,
                                TeamGameId = 2025160,
                                Name = SeedNames.RodinMotorsport24
                            },
                            new TeamEntity
                            {
                                Id = 323,
                                TeamGameId = 2025161,
                                Name = SeedNames.AIXRacing24
                            },
                            new TeamEntity
                            {
                                Id = 324,
                                TeamGameId = 2025162,
                                Name = SeedNames.Dams24
                            },
                            new TeamEntity
                            {
                                Id = 325,
                                TeamGameId = 2025163,
                                Name = SeedNames.Hitech24
                            },
                            new TeamEntity
                            {
                                Id = 326,
                                TeamGameId = 2025164,
                                Name = SeedNames.MPMotorsport24
                            },
                            new TeamEntity
                            {
                                Id = 327,
                                TeamGameId = 2025165,
                                Name = SeedNames.Prema24
                            },
                            new TeamEntity
                            {
                                Id = 328,
                                TeamGameId = 2025166,
                                Name = SeedNames.Trident24
                            },
                            new TeamEntity
                            {
                                Id = 329,
                                TeamGameId = 2025167,
                                Name = SeedNames.VanAmersfoortRacing24
                            },
                            new TeamEntity
                            {
                                Id = 330,
                                TeamGameId = 2025168,
                                Name = SeedNames.Invicta24
                            },
                            new TeamEntity
                            {
                                Id = 331,
                                TeamGameId = 2025185,
                                Name = SeedNames.Mercedes24
                            },
                            new TeamEntity
                            {
                                Id = 332,
                                TeamGameId = 2025186,
                                Name = SeedNames.Ferrari24
                            },
                            new TeamEntity
                            {
                                Id = 333,
                                TeamGameId = 2025187,
                                Name = SeedNames.RedBullRacing24
                            },
                            new TeamEntity
                            {
                                Id = 334,
                                TeamGameId = 2025188,
                                Name = SeedNames.Williams24
                            },
                            new TeamEntity
                            {
                                Id = 335,
                                TeamGameId = 2025189,
                                Name = SeedNames.AstonMartin24
                            },
                            new TeamEntity
                            {
                                Id = 336,
                                TeamGameId = 2025190,
                                Name = SeedNames.Alpine24
                            },
                            new TeamEntity
                            {
                                Id = 337,
                                TeamGameId = 2025191,
                                Name = SeedNames.RB24
                            },
                            new TeamEntity
                            {
                                Id = 338,
                                TeamGameId = 2025192,
                                Name = SeedNames.Haas24
                            },
                            new TeamEntity
                            {
                                Id = 339,
                                TeamGameId = 2025193,
                                Name = SeedNames.McLaren24
                            },
                            new TeamEntity
                            {
                                Id = 340,
                                TeamGameId = 2025194,
                                Name = SeedNames.Sauber24
                            },
                            new TeamEntity
                            {
                                Id = 341,
                                TeamGameId = 20260,
                                Name = SeedNames.Mercedes
                            },
                            new TeamEntity
                            {
                                Id = 342,
                                TeamGameId = 20261,
                                Name = SeedNames.Ferrari
                            },
                            new TeamEntity
                            {
                                Id = 343,
                                TeamGameId = 20262,
                                Name = SeedNames.RedBullRacing
                            },
                            new TeamEntity
                            {
                                Id = 344,
                                TeamGameId = 20263,
                                Name = SeedNames.Williams
                            },
                            new TeamEntity
                            {
                                Id = 345,
                                TeamGameId = 20264,
                                Name = SeedNames.AstonMartin
                            },
                            new TeamEntity
                            {
                                Id = 346,
                                TeamGameId = 20265,
                                Name = SeedNames.Alpine
                            },
                            new TeamEntity
                            {
                                Id = 347,
                                TeamGameId = 20266,
                                Name = SeedNames.RB
                            },
                            new TeamEntity
                            {
                                Id = 348,
                                TeamGameId = 20267,
                                Name = SeedNames.Haas
                            },
                            new TeamEntity
                            {
                                Id = 349,
                                TeamGameId = 20268,
                                Name = SeedNames.McLaren
                            },
                            new TeamEntity
                            {
                                Id = 350,
                                TeamGameId = 20269,
                                Name = SeedNames.Sauber
                            },
                            new TeamEntity
                            {
                                Id = 351,
                                TeamGameId = 202641,
                                Name = SeedNames.F1Generic
                            },
                            new TeamEntity
                            {
                                Id = 352,
                                TeamGameId = 2026104,
                                Name = SeedNames.F1CustomTeam
                            },
                            new TeamEntity
                            {
                                Id = 353,
                                TeamGameId = 2026129,
                                Name = SeedNames.Konnersport
                            },
                            new TeamEntity
                            {
                                Id = 354,
                                TeamGameId = 2026142,
                                Name = SeedNames.APXGP24
                            },
                            new TeamEntity
                            {
                                Id = 355,
                                TeamGameId = 2026154,
                                Name = SeedNames.APXGP25
                            },
                            new TeamEntity
                            {
                                Id = 356,
                                TeamGameId = 2026155,
                                Name = SeedNames.Konnersport24
                            },
                            new TeamEntity
                            {
                                Id = 357,
                                TeamGameId = 2026158,
                                Name = SeedNames.ArtGP24
                            },
                            new TeamEntity
                            {
                                Id = 358,
                                TeamGameId = 2026159,
                                Name = SeedNames.Campos24
                            },
                            new TeamEntity
                            {
                                Id = 359,
                                TeamGameId = 2026160,
                                Name = SeedNames.RodinMotorsport24
                            },
                            new TeamEntity
                            {
                                Id = 360,
                                TeamGameId = 2026161,
                                Name = SeedNames.AIXRacing24
                            },
                            new TeamEntity
                            {
                                Id = 361,
                                TeamGameId = 2026162,
                                Name = SeedNames.Dams24
                            },
                            new TeamEntity
                            {
                                Id = 362,
                                TeamGameId = 2026163,
                                Name = SeedNames.Hitech24
                            },
                            new TeamEntity
                            {
                                Id = 363,
                                TeamGameId = 2026164,
                                Name = SeedNames.MPMotorsport24
                            },
                            new TeamEntity
                            {
                                Id = 364,
                                TeamGameId = 2026165,
                                Name = SeedNames.Prema24
                            },
                            new TeamEntity
                            {
                                Id = 365,
                                TeamGameId = 2026166,
                                Name = SeedNames.Trident24
                            },
                            new TeamEntity
                            {
                                Id = 366,
                                TeamGameId = 2026167,
                                Name = SeedNames.VanAmersfoortRacing24
                            },
                            new TeamEntity
                            {
                                Id = 367,
                                TeamGameId = 2026168,
                                Name = SeedNames.Invicta24
                            },
                            new TeamEntity
                            {
                                Id = 368,
                                TeamGameId = 2026185,
                                Name = SeedNames.Mercedes24
                            },
                            new TeamEntity
                            {
                                Id = 369,
                                TeamGameId = 2026186,
                                Name = SeedNames.Ferrari24
                            },
                            new TeamEntity
                            {
                                Id = 370,
                                TeamGameId = 2026187,
                                Name = SeedNames.RedBullRacing24
                            },
                            new TeamEntity
                            {
                                Id = 371,
                                TeamGameId = 2026188,
                                Name = SeedNames.Williams24
                            },
                            new TeamEntity
                            {
                                Id = 372,
                                TeamGameId = 2026189,
                                Name = SeedNames.AstonMartin24
                            },
                            new TeamEntity
                            {
                                Id = 373,
                                TeamGameId = 2026190,
                                Name = SeedNames.Alpine24
                            },
                            new TeamEntity
                            {
                                Id = 374,
                                TeamGameId = 2026191,
                                Name = SeedNames.RB24
                            },
                            new TeamEntity
                            {
                                Id = 375,
                                TeamGameId = 2026192,
                                Name = SeedNames.Haas24
                            },
                            new TeamEntity
                            {
                                Id = 376,
                                TeamGameId = 2026193,
                                Name = SeedNames.McLaren24
                            },
                            new TeamEntity
                            {
                                Id = 377,
                                TeamGameId = 2026194,
                                Name = SeedNames.Sauber24
                            },
                            new TeamEntity
                            {
                                Id = 378,
                                TeamGameId = 2026465,
                                Name = SeedNames.ArtGP25
                            },
                            new TeamEntity
                            {
                                Id = 379,
                                TeamGameId = 2026466,
                                Name = SeedNames.Campos25
                            },
                            new TeamEntity
                            {
                                Id = 380,
                                TeamGameId = 2026467,
                                Name = SeedNames.RodinMotorsport25
                            },
                            new TeamEntity
                            {
                                Id = 381,
                                TeamGameId = 2026468,
                                Name = SeedNames.AIXRacing25
                            },
                            new TeamEntity
                            {
                                Id = 382,
                                TeamGameId = 2026469,
                                Name = SeedNames.Dams25
                            },
                            new TeamEntity
                            {
                                Id = 383,
                                TeamGameId = 2026470,
                                Name = SeedNames.Hitech25
                            },
                            new TeamEntity
                            {
                                Id = 384,
                                TeamGameId = 2026471,
                                Name = SeedNames.MPMotorsport25
                            },
                            new TeamEntity
                            {
                                Id = 385,
                                TeamGameId = 2026472,
                                Name = SeedNames.Prema25
                            },
                            new TeamEntity
                            {
                                Id = 386,
                                TeamGameId = 2026473,
                                Name = SeedNames.Trident25
                            },
                            new TeamEntity
                            {
                                Id = 387,
                                TeamGameId = 2026474,
                                Name = SeedNames.VanAmersfoortRacing25
                            },
                            new TeamEntity
                            {
                                Id = 388,
                                TeamGameId = 2026475,
                                Name = SeedNames.Invicta25
                            },
                            new TeamEntity
                            {
                                Id = 389,
                                TeamGameId = 2026476,
                                Name = SeedNames.Mercedes26
                            },
                            new TeamEntity
                            {
                                Id = 390,
                                TeamGameId = 2026477,
                                Name = SeedNames.Ferrari26
                            },
                            new TeamEntity
                            {
                                Id = 391,
                                TeamGameId = 2026478,
                                Name = SeedNames.RedBullRacing26
                            },
                            new TeamEntity
                            {
                                Id = 392,
                                TeamGameId = 2026479,
                                Name = SeedNames.Williams26
                            },
                            new TeamEntity
                            {
                                Id = 393,
                                TeamGameId = 2026480,
                                Name = SeedNames.AstonMartin26
                            },
                            new TeamEntity
                            {
                                Id = 394,
                                TeamGameId = 2026481,
                                Name = SeedNames.Alpine26
                            },
                            new TeamEntity
                            {
                                Id = 395,
                                TeamGameId = 2026482,
                                Name = SeedNames.RB26
                            },
                            new TeamEntity
                            {
                                Id = 396,
                                TeamGameId = 2026483,
                                Name = SeedNames.Haas26
                            },
                            new TeamEntity
                            {
                                Id = 397,
                                TeamGameId = 2026484,
                                Name = SeedNames.McLaren26
                            },
                            new TeamEntity
                            {
                                Id = 398,
                                TeamGameId = 2026485,
                                Name = SeedNames.Audi26
                            },
                            new TeamEntity
                            {
                                Id = 399,
                                TeamGameId = 2026486,
                                Name = SeedNames.Cadillac26
                            },
                            new TeamEntity
                            {
                                Id = 1000,
                                TeamGameId = 2020255,
                                Name = SeedNames.MyTeam20
                            },
                            new TeamEntity
                            {
                                Id = 1001,
                                TeamGameId = 2021255,
                                Name = SeedNames.MyTeam21
                            },
                            new TeamEntity
                            {
                                Id = 1002,
                                TeamGameId = 2022255,
                                Name = SeedNames.MyTeam22
                            },
                            new TeamEntity
                            {
                                Id = 1003,
                                TeamGameId = 2023255,
                                Name = SeedNames.MyTeam23
                            },
                            new TeamEntity
                            {
                                Id = 1004,
                                TeamGameId = 2024255,
                                Name = SeedNames.MyTeam24
                            },
                            new TeamEntity
                            {
                                Id = 1005,
                                TeamGameId = 2025255,
                                Name = SeedNames.MyTeam25
                            },
                            new TeamEntity
                            {
                                Id = 1006,
                                TeamGameId = 2026255,
                                Name = SeedNames.MyTeam26
                            });
        }
        catch
        {
            // Ignore exceptions in this step
        }
    }

    #endregion // IEntityTypeConfiguration
}