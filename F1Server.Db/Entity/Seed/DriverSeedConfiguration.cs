using F1Server.Core.Data;
using F1Server.Db.Entity.Tables;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace F1Server.Db.Entity.Seed;

/// <summary>
/// Seeds the default set of drivers
/// </summary>
public sealed class DriverSeedConfiguration : IEntityTypeConfiguration<DriverEntity>
{
    #region IEntityTypeConfiguration

    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<DriverEntity> builder)
    {
        try
        {
            builder.HasData(new DriverEntity
                            {
                                Id = 1,
                                DriverGameId = 0,
                                Name = SeedNames.CarlosSainz
                            },
                            new DriverEntity
                            {
                                Id = 2,
                                DriverGameId = 1,
                                Name = SeedNames.DaniilKvyat
                            },
                            new DriverEntity
                            {
                                Id = 3,
                                DriverGameId = 2,
                                Name = SeedNames.DanielRicciardo
                            },
                            new DriverEntity
                            {
                                Id = 4,
                                DriverGameId = 3,
                                Name = SeedNames.FernandoAlonso
                            },
                            new DriverEntity
                            {
                                Id = 5,
                                DriverGameId = 4,
                                Name = SeedNames.FelipeMassa
                            },
                            new DriverEntity
                            {
                                Id = 6,
                                DriverGameId = 6,
                                Name = SeedNames.KimiRaikkonen
                            },
                            new DriverEntity
                            {
                                Id = 7,
                                DriverGameId = 7,
                                Name = SeedNames.LewisHamilton
                            },
                            new DriverEntity
                            {
                                Id = 8,
                                DriverGameId = 9,
                                Name = SeedNames.MaxVerstappen
                            },
                            new DriverEntity
                            {
                                Id = 9,
                                DriverGameId = 10,
                                Name = SeedNames.NicoHulkenberg
                            },
                            new DriverEntity
                            {
                                Id = 10,
                                DriverGameId = 11,
                                Name = SeedNames.KevinMagnussen
                            },
                            new DriverEntity
                            {
                                Id = 11,
                                DriverGameId = 12,
                                Name = SeedNames.RomainGrosjean
                            },
                            new DriverEntity
                            {
                                Id = 12,
                                DriverGameId = 13,
                                Name = SeedNames.SebastianVettel
                            },
                            new DriverEntity
                            {
                                Id = 13,
                                DriverGameId = 14,
                                Name = SeedNames.SergioPerez
                            },
                            new DriverEntity
                            {
                                Id = 14,
                                DriverGameId = 15,
                                Name = SeedNames.ValtteriBottas
                            },
                            new DriverEntity
                            {
                                Id = 15,
                                DriverGameId = 17,
                                Name = SeedNames.EstebanOcon
                            },
                            new DriverEntity
                            {
                                Id = 16,
                                DriverGameId = 19,
                                Name = SeedNames.LanceStroll
                            },
                            new DriverEntity
                            {
                                Id = 17,
                                DriverGameId = 20,
                                Name = SeedNames.ArronBarnes
                            },
                            new DriverEntity
                            {
                                Id = 18,
                                DriverGameId = 21,
                                Name = SeedNames.MartinGiles
                            },
                            new DriverEntity
                            {
                                Id = 19,
                                DriverGameId = 22,
                                Name = SeedNames.AlexMurray
                            },
                            new DriverEntity
                            {
                                Id = 20,
                                DriverGameId = 23,
                                Name = SeedNames.LucasRoth
                            },
                            new DriverEntity
                            {
                                Id = 21,
                                DriverGameId = 24,
                                Name = SeedNames.IgorCorreia
                            },
                            new DriverEntity
                            {
                                Id = 22,
                                DriverGameId = 25,
                                Name = SeedNames.SophieLevasseur
                            },
                            new DriverEntity
                            {
                                Id = 23,
                                DriverGameId = 26,
                                Name = SeedNames.JonasSchiffer
                            },
                            new DriverEntity
                            {
                                Id = 24,
                                DriverGameId = 27,
                                Name = SeedNames.AlainForest
                            },
                            new DriverEntity
                            {
                                Id = 25,
                                DriverGameId = 28,
                                Name = SeedNames.JayLetourneau
                            },
                            new DriverEntity
                            {
                                Id = 26,
                                DriverGameId = 29,
                                Name = SeedNames.EstoSaari
                            },
                            new DriverEntity
                            {
                                Id = 27,
                                DriverGameId = 30,
                                Name = SeedNames.YasarAtiyeh
                            },
                            new DriverEntity
                            {
                                Id = 28,
                                DriverGameId = 31,
                                Name = SeedNames.CallistoCalabresi
                            },
                            new DriverEntity
                            {
                                Id = 29,
                                DriverGameId = 32,
                                Name = SeedNames.NaotaIzumi
                            },
                            new DriverEntity
                            {
                                Id = 30,
                                DriverGameId = 33,
                                Name = SeedNames.HowardClarke
                            },
                            new DriverEntity
                            {
                                Id = 31,
                                DriverGameId = 34,
                                Name = SeedNames.WilheimKaufmann
                            },
                            new DriverEntity
                            {
                                Id = 32,
                                DriverGameId = 35,
                                Name = SeedNames.MarieLaursen
                            },
                            new DriverEntity
                            {
                                Id = 33,
                                DriverGameId = 36,
                                Name = SeedNames.FlavioNieves
                            },
                            new DriverEntity
                            {
                                Id = 34,
                                DriverGameId = 37,
                                Name = SeedNames.PeterBelousov
                            },
                            new DriverEntity
                            {
                                Id = 35,
                                DriverGameId = 38,
                                Name = SeedNames.KlimekMichalski
                            },
                            new DriverEntity
                            {
                                Id = 36,
                                DriverGameId = 39,
                                Name = SeedNames.SantiagoMoreno
                            },
                            new DriverEntity
                            {
                                Id = 37,
                                DriverGameId = 40,
                                Name = SeedNames.BenjaminCoppens
                            },
                            new DriverEntity
                            {
                                Id = 38,
                                DriverGameId = 41,
                                Name = SeedNames.NoahVisser
                            },
                            new DriverEntity
                            {
                                Id = 39,
                                DriverGameId = 42,
                                Name = SeedNames.GertWaldmuller
                            },
                            new DriverEntity
                            {
                                Id = 40,
                                DriverGameId = 43,
                                Name = SeedNames.JulianQuesada
                            },
                            new DriverEntity
                            {
                                Id = 41,
                                DriverGameId = 44,
                                Name = SeedNames.DanielJones
                            },
                            new DriverEntity
                            {
                                Id = 42,
                                DriverGameId = 45,
                                Name = SeedNames.ArtemMarkelov
                            },
                            new DriverEntity
                            {
                                Id = 43,
                                DriverGameId = 46,
                                Name = SeedNames.TadasukeMakino
                            },
                            new DriverEntity
                            {
                                Id = 44,
                                DriverGameId = 47,
                                Name = SeedNames.SeanGelael
                            },
                            new DriverEntity
                            {
                                Id = 45,
                                DriverGameId = 48,
                                Name = SeedNames.NyckDeVries
                            },
                            new DriverEntity
                            {
                                Id = 46,
                                DriverGameId = 49,
                                Name = SeedNames.JackAitken
                            },
                            new DriverEntity
                            {
                                Id = 47,
                                DriverGameId = 50,
                                Name = SeedNames.GeorgeRussell
                            },
                            new DriverEntity
                            {
                                Id = 48,
                                DriverGameId = 51,
                                Name = SeedNames.MaximilianGunther
                            },
                            new DriverEntity
                            {
                                Id = 49,
                                DriverGameId = 52,
                                Name = SeedNames.NireiFukuzumi
                            },
                            new DriverEntity
                            {
                                Id = 50,
                                DriverGameId = 53,
                                Name = SeedNames.LucaGhiotto
                            },
                            new DriverEntity
                            {
                                Id = 51,
                                DriverGameId = 54,
                                Name = SeedNames.LandoNorris
                            },
                            new DriverEntity
                            {
                                Id = 52,
                                DriverGameId = 55,
                                Name = SeedNames.SergioSetteCamara
                            },
                            new DriverEntity
                            {
                                Id = 53,
                                DriverGameId = 56,
                                Name = SeedNames.LouisDeletraz
                            },
                            new DriverEntity
                            {
                                Id = 54,
                                DriverGameId = 57,
                                Name = SeedNames.AntonioFuoco
                            },
                            new DriverEntity
                            {
                                Id = 55,
                                DriverGameId = 58,
                                Name = SeedNames.CharlesLeclerc
                            },
                            new DriverEntity
                            {
                                Id = 56,
                                DriverGameId = 59,
                                Name = SeedNames.PierreGasly
                            },
                            new DriverEntity
                            {
                                Id = 57,
                                DriverGameId = 62,
                                Name = SeedNames.AlexanderAlbon
                            },
                            new DriverEntity
                            {
                                Id = 58,
                                DriverGameId = 63,
                                Name = SeedNames.NicholasLatifi
                            },
                            new DriverEntity
                            {
                                Id = 59,
                                DriverGameId = 64,
                                Name = SeedNames.DorianBoccolacci
                            },
                            new DriverEntity
                            {
                                Id = 60,
                                DriverGameId = 65,
                                Name = SeedNames.NikoKari
                            },
                            new DriverEntity
                            {
                                Id = 61,
                                DriverGameId = 66,
                                Name = SeedNames.RobertoMerhi
                            },
                            new DriverEntity
                            {
                                Id = 62,
                                DriverGameId = 67,
                                Name = SeedNames.ArjunMaini
                            },
                            new DriverEntity
                            {
                                Id = 63,
                                DriverGameId = 68,
                                Name = SeedNames.AlessioLorandi
                            },
                            new DriverEntity
                            {
                                Id = 64,
                                DriverGameId = 69,
                                Name = SeedNames.RubenMeijer
                            },
                            new DriverEntity
                            {
                                Id = 65,
                                DriverGameId = 70,
                                Name = SeedNames.RashidNair
                            },
                            new DriverEntity
                            {
                                Id = 66,
                                DriverGameId = 71,
                                Name = SeedNames.JackTremblay
                            },
                            new DriverEntity
                            {
                                Id = 67,
                                DriverGameId = 72,
                                Name = SeedNames.DevonButler
                            },
                            new DriverEntity
                            {
                                Id = 68,
                                DriverGameId = 73,
                                Name = SeedNames.LukasWeber
                            },
                            new DriverEntity
                            {
                                Id = 69,
                                DriverGameId = 74,
                                Name = SeedNames.AntonioGiovinazzi
                            },
                            new DriverEntity
                            {
                                Id = 70,
                                DriverGameId = 75,
                                Name = SeedNames.RobertKubica
                            },
                            new DriverEntity
                            {
                                Id = 71,
                                DriverGameId = 76,
                                Name = SeedNames.AlainProst
                            },
                            new DriverEntity
                            {
                                Id = 72,
                                DriverGameId = 77,
                                Name = SeedNames.AyrtonSenna
                            },
                            new DriverEntity
                            {
                                Id = 73,
                                DriverGameId = 78,
                                Name = SeedNames.NobuharuMatsushita
                            },
                            new DriverEntity
                            {
                                Id = 74,
                                DriverGameId = 79,
                                Name = SeedNames.NikitaMazepin
                            },
                            new DriverEntity
                            {
                                Id = 75,
                                DriverGameId = 80,
                                Name = SeedNames.GuanyuZhou
                            },
                            new DriverEntity
                            {
                                Id = 76,
                                DriverGameId = 81,
                                Name = SeedNames.MickSchumacher
                            },
                            new DriverEntity
                            {
                                Id = 77,
                                DriverGameId = 82,
                                Name = SeedNames.CallumIlott
                            },
                            new DriverEntity
                            {
                                Id = 78,
                                DriverGameId = 83,
                                Name = SeedNames.JuanManuelCorrea
                            },
                            new DriverEntity
                            {
                                Id = 79,
                                DriverGameId = 84,
                                Name = SeedNames.JordanKing
                            },
                            new DriverEntity
                            {
                                Id = 80,
                                DriverGameId = 85,
                                Name = SeedNames.MahaveerRaghunathan
                            },
                            new DriverEntity
                            {
                                Id = 81,
                                DriverGameId = 86,
                                Name = SeedNames.TatianaCalderon
                            },
                            new DriverEntity
                            {
                                Id = 82,
                                DriverGameId = 87,
                                Name = SeedNames.AnthoineHubert
                            },
                            new DriverEntity
                            {
                                Id = 83,
                                DriverGameId = 88,
                                Name = SeedNames.GuilianoAlesi
                            },
                            new DriverEntity
                            {
                                Id = 84,
                                DriverGameId = 89,
                                Name = SeedNames.RalphBoschung
                            },
                            new DriverEntity
                            {
                                Id = 85,
                                DriverGameId = 90,
                                Name = SeedNames.MichaelSchumacher
                            },
                            new DriverEntity
                            {
                                Id = 86,
                                DriverGameId = 91,
                                Name = SeedNames.DanTicktum
                            },
                            new DriverEntity
                            {
                                Id = 87,
                                DriverGameId = 92,
                                Name = SeedNames.MarcusArmstrong
                            },
                            new DriverEntity
                            {
                                Id = 88,
                                DriverGameId = 93,
                                Name = SeedNames.ChristianLundgaard
                            },
                            new DriverEntity
                            {
                                Id = 89,
                                DriverGameId = 94,
                                Name = SeedNames.YukiTsunoda
                            },
                            new DriverEntity
                            {
                                Id = 90,
                                DriverGameId = 95,
                                Name = SeedNames.JehanDaruvala
                            },
                            new DriverEntity
                            {
                                Id = 91,
                                DriverGameId = 96,
                                Name = SeedNames.GulhermeSamaia
                            },
                            new DriverEntity
                            {
                                Id = 92,
                                DriverGameId = 97,
                                Name = SeedNames.PedroPiquet
                            },
                            new DriverEntity
                            {
                                Id = 93,
                                DriverGameId = 98,
                                Name = SeedNames.FelipeDrugovich
                            },
                            new DriverEntity
                            {
                                Id = 94,
                                DriverGameId = 99,
                                Name = SeedNames.RobertSchwartzman
                            },
                            new DriverEntity
                            {
                                Id = 95,
                                DriverGameId = 100,
                                Name = SeedNames.RoyNissany
                            },
                            new DriverEntity
                            {
                                Id = 96,
                                DriverGameId = 101,
                                Name = SeedNames.MarinoSato
                            },
                            new DriverEntity
                            {
                                Id = 97,
                                DriverGameId = 102,
                                Name = SeedNames.AidanJackson
                            },
                            new DriverEntity
                            {
                                Id = 98,
                                DriverGameId = 103,
                                Name = SeedNames.CasperAkkerman
                            },
                            new DriverEntity
                            {
                                Id = 99,
                                DriverGameId = 109,
                                Name = SeedNames.JensonButton
                            },
                            new DriverEntity
                            {
                                Id = 100,
                                DriverGameId = 110,
                                Name = SeedNames.DavidCoulthard
                            },
                            new DriverEntity
                            {
                                Id = 101,
                                DriverGameId = 111,
                                Name = SeedNames.NicoRosberg
                            },
                            new DriverEntity
                            {
                                Id = 102,
                                DriverGameId = 112,
                                Name = SeedNames.OscarPiastri
                            },
                            new DriverEntity
                            {
                                Id = 103,
                                DriverGameId = 113,
                                Name = SeedNames.LiamLawson
                            },
                            new DriverEntity
                            {
                                Id = 104,
                                DriverGameId = 114,
                                Name = SeedNames.JuriVips
                            },
                            new DriverEntity
                            {
                                Id = 105,
                                DriverGameId = 115,
                                Name = SeedNames.TheoPourchaire
                            },
                            new DriverEntity
                            {
                                Id = 106,
                                DriverGameId = 116,
                                Name = SeedNames.RichardVerschoor
                            },
                            new DriverEntity
                            {
                                Id = 107,
                                DriverGameId = 117,
                                Name = SeedNames.LirimZendeli
                            },
                            new DriverEntity
                            {
                                Id = 108,
                                DriverGameId = 118,
                                Name = SeedNames.DavidBeckmann
                            },
                            new DriverEntity
                            {
                                Id = 109,
                                DriverGameId = 119,
                                Name = SeedNames.GianlucaPetecof
                            },
                            new DriverEntity
                            {
                                Id = 110,
                                DriverGameId = 120,
                                Name = SeedNames.MatteoNannini
                            },
                            new DriverEntity
                            {
                                Id = 111,
                                DriverGameId = 121,
                                Name = SeedNames.AlessioDeledda
                            },
                            new DriverEntity
                            {
                                Id = 112,
                                DriverGameId = 122,
                                Name = SeedNames.BentViscaal
                            },
                            new DriverEntity
                            {
                                Id = 113,
                                DriverGameId = 123,
                                Name = SeedNames.EnzoFittipaldi
                            },
                            new DriverEntity
                            {
                                Id = 114,
                                DriverGameId = 125,
                                Name = SeedNames.MarkWebber
                            },
                            new DriverEntity
                            {
                                Id = 115,
                                DriverGameId = 126,
                                Name = SeedNames.JacquesVilleneuve
                            },
                            new DriverEntity
                            {
                                Id = 116,
                                DriverGameId = 127,
                                Name = SeedNames.JakeHughes
                            },
                            new DriverEntity
                            {
                                Id = 117,
                                DriverGameId = 128,
                                Name = SeedNames.FrederikVesti
                            },
                            new DriverEntity
                            {
                                Id = 118,
                                DriverGameId = 129,
                                Name = SeedNames.OlliCaldwell
                            },
                            new DriverEntity
                            {
                                Id = 119,
                                DriverGameId = 130,
                                Name = SeedNames.LoganSargeant
                            },
                            new DriverEntity
                            {
                                Id = 120,
                                DriverGameId = 131,
                                Name = SeedNames.CemBolukbasi
                            },
                            new DriverEntity
                            {
                                Id = 121,
                                DriverGameId = 132,
                                Name = SeedNames.AyumuIwasa
                            },
                            new DriverEntity
                            {
                                Id = 122,
                                DriverGameId = 133,
                                Name = SeedNames.ClementNovalak
                            },
                            new DriverEntity
                            {
                                Id = 123,
                                DriverGameId = 134,
                                Name = SeedNames.DennisHauger
                            },
                            new DriverEntity
                            {
                                Id = 124,
                                DriverGameId = 135,
                                Name = SeedNames.CalanWilliams
                            },
                            new DriverEntity
                            {
                                Id = 125,
                                DriverGameId = 136,
                                Name = SeedNames.JackDoohan
                            },
                            new DriverEntity
                            {
                                Id = 126,
                                DriverGameId = 137,
                                Name = SeedNames.AmauryCordeel
                            },
                            new DriverEntity
                            {
                                Id = 127,
                                DriverGameId = 138,
                                Name = SeedNames.MikaHakkinen
                            },
                            new DriverEntity
                            {
                                Id = 128,
                                DriverGameId = 139,
                                Name = SeedNames.CallieMayer
                            },
                            new DriverEntity
                            {
                                Id = 129,
                                DriverGameId = 140,
                                Name = SeedNames.NoahBell
                            },
                            new DriverEntity
                            {
                                Id = 130,
                                DriverGameId = 141,
                                Name = SeedNames.JakeHughes
                            },
                            new DriverEntity
                            {
                                Id = 131,
                                DriverGameId = 142,
                                Name = SeedNames.FrederikVesti
                            },
                            new DriverEntity
                            {
                                Id = 132,
                                DriverGameId = 143,
                                Name = SeedNames.OlliCaldwell
                            },
                            new DriverEntity
                            {
                                Id = 133,
                                DriverGameId = 144,
                                Name = SeedNames.LoganSargeant
                            },
                            new DriverEntity
                            {
                                Id = 134,
                                DriverGameId = 145,
                                Name = SeedNames.CemBolukbasi
                            },
                            new DriverEntity
                            {
                                Id = 135,
                                DriverGameId = 146,
                                Name = SeedNames.AyumuIwasa
                            },
                            new DriverEntity
                            {
                                Id = 136,
                                DriverGameId = 147,
                                Name = SeedNames.ClementNovalak
                            },
                            new DriverEntity
                            {
                                Id = 137,
                                DriverGameId = 148,
                                Name = SeedNames.JackDoohan
                            },
                            new DriverEntity
                            {
                                Id = 138,
                                DriverGameId = 149,
                                Name = SeedNames.AmauryCordeel
                            },
                            new DriverEntity
                            {
                                Id = 139,
                                DriverGameId = 150,
                                Name = SeedNames.DennisHauger
                            },
                            new DriverEntity
                            {
                                Id = 140,
                                DriverGameId = 151,
                                Name = SeedNames.CalanWilliams
                            },
                            new DriverEntity
                            {
                                Id = 141,
                                DriverGameId = 152,
                                Name = SeedNames.JamieChadwick
                            },
                            new DriverEntity
                            {
                                Id = 142,
                                DriverGameId = 153,
                                Name = SeedNames.KamuiKobayashi
                            },
                            new DriverEntity
                            {
                                Id = 143,
                                DriverGameId = 154,
                                Name = SeedNames.PastorMaldonado
                            },
                            new DriverEntity
                            {
                                Id = 144,
                                DriverGameId = 155,
                                Name = SeedNames.MikaHakkinen
                            },
                            new DriverEntity
                            {
                                Id = 145,
                                DriverGameId = 156,
                                Name = SeedNames.NigelMansell
                            },
                            new DriverEntity
                            {
                                Id = 146,
                                DriverGameId = 157,
                                Name = SeedNames.ZaneMaloney
                            },
                            new DriverEntity
                            {
                                Id = 147,
                                DriverGameId = 158,
                                Name = SeedNames.VictorMartins
                            },
                            new DriverEntity
                            {
                                Id = 148,
                                DriverGameId = 159,
                                Name = SeedNames.OliverBearman
                            },
                            new DriverEntity
                            {
                                Id = 149,
                                DriverGameId = 160,
                                Name = SeedNames.JakCrawford
                            },
                            new DriverEntity
                            {
                                Id = 150,
                                DriverGameId = 161,
                                Name = SeedNames.IsackHadjar
                            },
                            new DriverEntity
                            {
                                Id = 151,
                                DriverGameId = 162,
                                Name = SeedNames.ArthurLeclerc
                            },
                            new DriverEntity
                            {
                                Id = 152,
                                DriverGameId = 163,
                                Name = SeedNames.BradBenavides
                            },
                            new DriverEntity
                            {
                                Id = 153,
                                DriverGameId = 164,
                                Name = SeedNames.RomanStanek
                            },
                            new DriverEntity
                            {
                                Id = 154,
                                DriverGameId = 165,
                                Name = SeedNames.KushMaini
                            },
                            new DriverEntity
                            {
                                Id = 155,
                                DriverGameId = 166,
                                Name = SeedNames.JamesHunt
                            },
                            new DriverEntity
                            {
                                Id = 156,
                                DriverGameId = 167,
                                Name = SeedNames.JuanPabloMontoya
                            },
                            new DriverEntity
                            {
                                Id = 157,
                                DriverGameId = 168,
                                Name = SeedNames.BrendonLeigh
                            },
                            new DriverEntity
                            {
                                Id = 158,
                                DriverGameId = 169,
                                Name = SeedNames.DavidTonizza
                            },
                            new DriverEntity
                            {
                                Id = 159,
                                DriverGameId = 170,
                                Name = SeedNames.JarnoOpmeer
                            },
                            new DriverEntity
                            {
                                Id = 160,
                                DriverGameId = 171,
                                Name = SeedNames.LucasBlakeley
                            },
                            new DriverEntity
                            {
                                Id = 161,
                                DriverGameId = 20250,
                                Name = SeedNames.CarlosSainz
                            },
                            new DriverEntity
                            {
                                Id = 162,
                                DriverGameId = 20252,
                                Name = SeedNames.DanielRicciardo
                            },
                            new DriverEntity
                            {
                                Id = 163,
                                DriverGameId = 20253,
                                Name = SeedNames.FernandoAlonso
                            },
                            new DriverEntity
                            {
                                Id = 164,
                                DriverGameId = 20254,
                                Name = SeedNames.FelipeMassa
                            },
                            new DriverEntity
                            {
                                Id = 165,
                                DriverGameId = 20257,
                                Name = SeedNames.LewisHamilton
                            },
                            new DriverEntity
                            {
                                Id = 166,
                                DriverGameId = 20259,
                                Name = SeedNames.MaxVerstappen
                            },
                            new DriverEntity
                            {
                                Id = 167,
                                DriverGameId = 202510,
                                Name = SeedNames.NicoHulkenberg
                            },
                            new DriverEntity
                            {
                                Id = 168,
                                DriverGameId = 202511,
                                Name = SeedNames.KevinMagnussen
                            },
                            new DriverEntity
                            {
                                Id = 169,
                                DriverGameId = 202514,
                                Name = SeedNames.SergioPerez
                            },
                            new DriverEntity
                            {
                                Id = 170,
                                DriverGameId = 202515,
                                Name = SeedNames.ValtteriBottas
                            },
                            new DriverEntity
                            {
                                Id = 171,
                                DriverGameId = 202517,
                                Name = SeedNames.EstebanOcon
                            },
                            new DriverEntity
                            {
                                Id = 172,
                                DriverGameId = 202519,
                                Name = SeedNames.LanceStroll
                            },
                            new DriverEntity
                            {
                                Id = 173,
                                DriverGameId = 202520,
                                Name = SeedNames.ArronBarnes
                            },
                            new DriverEntity
                            {
                                Id = 174,
                                DriverGameId = 202521,
                                Name = SeedNames.MartinGiles
                            },
                            new DriverEntity
                            {
                                Id = 175,
                                DriverGameId = 202522,
                                Name = SeedNames.AlexMurray
                            },
                            new DriverEntity
                            {
                                Id = 176,
                                DriverGameId = 202523,
                                Name = SeedNames.LucasRoth
                            },
                            new DriverEntity
                            {
                                Id = 177,
                                DriverGameId = 202524,
                                Name = SeedNames.IgorCorreia
                            },
                            new DriverEntity
                            {
                                Id = 178,
                                DriverGameId = 202525,
                                Name = SeedNames.SophieLevasseur
                            },
                            new DriverEntity
                            {
                                Id = 179,
                                DriverGameId = 202526,
                                Name = SeedNames.JonasSchiffer
                            },
                            new DriverEntity
                            {
                                Id = 180,
                                DriverGameId = 202527,
                                Name = SeedNames.AlainForest
                            },
                            new DriverEntity
                            {
                                Id = 181,
                                DriverGameId = 202528,
                                Name = SeedNames.JayLetourneau
                            },
                            new DriverEntity
                            {
                                Id = 182,
                                DriverGameId = 202529,
                                Name = SeedNames.EstoSaari
                            },
                            new DriverEntity
                            {
                                Id = 183,
                                DriverGameId = 202530,
                                Name = SeedNames.YasarAtiyeh
                            },
                            new DriverEntity
                            {
                                Id = 184,
                                DriverGameId = 202531,
                                Name = SeedNames.CallistoCalabresi
                            },
                            new DriverEntity
                            {
                                Id = 185,
                                DriverGameId = 202532,
                                Name = SeedNames.NaotaIzumi
                            },
                            new DriverEntity
                            {
                                Id = 186,
                                DriverGameId = 202533,
                                Name = SeedNames.HowardClarke
                            },
                            new DriverEntity
                            {
                                Id = 187,
                                DriverGameId = 202534,
                                Name = SeedNames.LarsKaufmann
                            },
                            new DriverEntity
                            {
                                Id = 188,
                                DriverGameId = 202535,
                                Name = SeedNames.MarieLaursen
                            },
                            new DriverEntity
                            {
                                Id = 189,
                                DriverGameId = 202536,
                                Name = SeedNames.FlavioNieves
                            },
                            new DriverEntity
                            {
                                Id = 190,
                                DriverGameId = 202538,
                                Name = SeedNames.KlimekMichalski
                            },
                            new DriverEntity
                            {
                                Id = 191,
                                DriverGameId = 202539,
                                Name = SeedNames.SantiagoMoreno
                            },
                            new DriverEntity
                            {
                                Id = 192,
                                DriverGameId = 202540,
                                Name = SeedNames.BenjaminCoppens
                            },
                            new DriverEntity
                            {
                                Id = 193,
                                DriverGameId = 202541,
                                Name = SeedNames.NoahVisser
                            },
                            new DriverEntity
                            {
                                Id = 194,
                                DriverGameId = 202550,
                                Name = SeedNames.GeorgeRussell
                            },
                            new DriverEntity
                            {
                                Id = 195,
                                DriverGameId = 202554,
                                Name = SeedNames.LandoNorris
                            },
                            new DriverEntity
                            {
                                Id = 196,
                                DriverGameId = 202558,
                                Name = SeedNames.CharlesLeclerc
                            },
                            new DriverEntity
                            {
                                Id = 197,
                                DriverGameId = 202559,
                                Name = SeedNames.PierreGasly
                            },
                            new DriverEntity
                            {
                                Id = 198,
                                DriverGameId = 202562,
                                Name = SeedNames.AlexanderAlbon
                            },
                            new DriverEntity
                            {
                                Id = 199,
                                DriverGameId = 202570,
                                Name = SeedNames.RashidNair
                            },
                            new DriverEntity
                            {
                                Id = 200,
                                DriverGameId = 202571,
                                Name = SeedNames.JackTremblay
                            },
                            new DriverEntity
                            {
                                Id = 201,
                                DriverGameId = 202577,
                                Name = SeedNames.AyrtonSenna
                            },
                            new DriverEntity
                            {
                                Id = 202,
                                DriverGameId = 202580,
                                Name = SeedNames.GuanyuZhou
                            },
                            new DriverEntity
                            {
                                Id = 203,
                                DriverGameId = 202583,
                                Name = SeedNames.JuanManuelCorrea
                            },
                            new DriverEntity
                            {
                                Id = 204,
                                DriverGameId = 202590,
                                Name = SeedNames.MichaelSchumacher
                            },
                            new DriverEntity
                            {
                                Id = 205,
                                DriverGameId = 202594,
                                Name = SeedNames.YukiTsunoda
                            },
                            new DriverEntity
                            {
                                Id = 206,
                                DriverGameId = 2025102,
                                Name = SeedNames.AidanJackson
                            },
                            new DriverEntity
                            {
                                Id = 207,
                                DriverGameId = 2025109,
                                Name = SeedNames.JensonButton
                            },
                            new DriverEntity
                            {
                                Id = 208,
                                DriverGameId = 2025110,
                                Name = SeedNames.DavidCoulthard
                            },
                            new DriverEntity
                            {
                                Id = 209,
                                DriverGameId = 2025112,
                                Name = SeedNames.OscarPiastri
                            },
                            new DriverEntity
                            {
                                Id = 210,
                                DriverGameId = 2025113,
                                Name = SeedNames.LiamLawson
                            },
                            new DriverEntity
                            {
                                Id = 211,
                                DriverGameId = 2025116,
                                Name = SeedNames.RichardVerschoor
                            },
                            new DriverEntity
                            {
                                Id = 212,
                                DriverGameId = 2025123,
                                Name = SeedNames.EnzoFittipaldi
                            },
                            new DriverEntity
                            {
                                Id = 213,
                                DriverGameId = 2025125,
                                Name = SeedNames.MarkWebber
                            },
                            new DriverEntity
                            {
                                Id = 214,
                                DriverGameId = 2025126,
                                Name = SeedNames.JacquesVilleneuve
                            },
                            new DriverEntity
                            {
                                Id = 215,
                                DriverGameId = 2025127,
                                Name = SeedNames.CallieMayer
                            },
                            new DriverEntity
                            {
                                Id = 216,
                                DriverGameId = 2025132,
                                Name = SeedNames.LoganSargeant
                            },
                            new DriverEntity
                            {
                                Id = 217,
                                DriverGameId = 2025136,
                                Name = SeedNames.JackDoohan
                            },
                            new DriverEntity
                            {
                                Id = 218,
                                DriverGameId = 2025137,
                                Name = SeedNames.AmauryCordeel
                            },
                            new DriverEntity
                            {
                                Id = 219,
                                DriverGameId = 2025138,
                                Name = SeedNames.DennisHauger
                            },
                            new DriverEntity
                            {
                                Id = 220,
                                DriverGameId = 2025145,
                                Name = SeedNames.ZaneMaloney
                            },
                            new DriverEntity
                            {
                                Id = 221,
                                DriverGameId = 2025146,
                                Name = SeedNames.VictorMartins
                            },
                            new DriverEntity
                            {
                                Id = 222,
                                DriverGameId = 2025147,
                                Name = SeedNames.OliverBearman
                            },
                            new DriverEntity
                            {
                                Id = 223,
                                DriverGameId = 2025148,
                                Name = SeedNames.JakCrawford
                            },
                            new DriverEntity
                            {
                                Id = 224,
                                DriverGameId = 2025149,
                                Name = SeedNames.IsackHadjar
                            },
                            new DriverEntity
                            {
                                Id = 225,
                                DriverGameId = 2025152,
                                Name = SeedNames.RomanStanek
                            },
                            new DriverEntity
                            {
                                Id = 226,
                                DriverGameId = 2025153,
                                Name = SeedNames.KushMaini
                            },
                            new DriverEntity
                            {
                                Id = 227,
                                DriverGameId = 2025156,
                                Name = SeedNames.BrendonLeigh
                            },
                            new DriverEntity
                            {
                                Id = 228,
                                DriverGameId = 2025157,
                                Name = SeedNames.DavidTonizza
                            },
                            new DriverEntity
                            {
                                Id = 229,
                                DriverGameId = 2025158,
                                Name = SeedNames.JarnoOpmeer
                            },
                            new DriverEntity
                            {
                                Id = 230,
                                DriverGameId = 2025159,
                                Name = SeedNames.LucasBlakeley
                            },
                            new DriverEntity
                            {
                                Id = 231,
                                DriverGameId = 2025160,
                                Name = SeedNames.PaulAron
                            },
                            new DriverEntity
                            {
                                Id = 232,
                                DriverGameId = 2025161,
                                Name = SeedNames.GabrielBortoleto
                            },
                            new DriverEntity
                            {
                                Id = 233,
                                DriverGameId = 2025162,
                                Name = SeedNames.FrancoColapinto
                            },
                            new DriverEntity
                            {
                                Id = 234,
                                DriverGameId = 2025163,
                                Name = SeedNames.TaylorBarnard
                            },
                            new DriverEntity
                            {
                                Id = 235,
                                DriverGameId = 2025164,
                                Name = SeedNames.JoshuaDurksen
                            },
                            new DriverEntity
                            {
                                Id = 236,
                                DriverGameId = 2025165,
                                Name = SeedNames.AndreaKimiAntonelli
                            },
                            new DriverEntity
                            {
                                Id = 237,
                                DriverGameId = 2025166,
                                Name = SeedNames.RitomoMiyata
                            },
                            new DriverEntity
                            {
                                Id = 238,
                                DriverGameId = 2025167,
                                Name = SeedNames.RafaelVillagomez
                            },
                            new DriverEntity
                            {
                                Id = 239,
                                DriverGameId = 2025168,
                                Name = SeedNames.ZakOSullivan
                            },
                            new DriverEntity
                            {
                                Id = 240,
                                DriverGameId = 2025169,
                                Name = SeedNames.PepeMarti
                            },
                            new DriverEntity
                            {
                                Id = 241,
                                DriverGameId = 2025170,
                                Name = SeedNames.SonnyHayes
                            },
                            new DriverEntity
                            {
                                Id = 242,
                                DriverGameId = 2025171,
                                Name = SeedNames.JoshuaPearce
                            },
                            new DriverEntity
                            {
                                Id = 243,
                                DriverGameId = 2025172,
                                Name = SeedNames.CallumVoisin
                            },
                            new DriverEntity
                            {
                                Id = 244,
                                DriverGameId = 2025173,
                                Name = SeedNames.MatiasZagazeta
                            },
                            new DriverEntity
                            {
                                Id = 245,
                                DriverGameId = 2025174,
                                Name = SeedNames.NikolaTsolov
                            },
                            new DriverEntity
                            {
                                Id = 246,
                                DriverGameId = 2025175,
                                Name = SeedNames.TimTramnitz
                            },
                            new DriverEntity
                            {
                                Id = 247,
                                DriverGameId = 2025185,
                                Name = SeedNames.LucaCortez
                            },
                            new DriverEntity
                            {
                                Id = 248,
                                DriverGameId = 20260,
                                Name = SeedNames.CarlosSainz
                            },
                            new DriverEntity
                            {
                                Id = 249,
                                DriverGameId = 20262,
                                Name = SeedNames.DanielRicciardo
                            },
                            new DriverEntity
                            {
                                Id = 250,
                                DriverGameId = 20263,
                                Name = SeedNames.FernandoAlonso
                            },
                            new DriverEntity
                            {
                                Id = 251,
                                DriverGameId = 20264,
                                Name = SeedNames.FelipeMassa
                            },
                            new DriverEntity
                            {
                                Id = 252,
                                DriverGameId = 20267,
                                Name = SeedNames.LewisHamilton
                            },
                            new DriverEntity
                            {
                                Id = 253,
                                DriverGameId = 20269,
                                Name = SeedNames.MaxVerstappen
                            },
                            new DriverEntity
                            {
                                Id = 254,
                                DriverGameId = 202610,
                                Name = SeedNames.NicoHulkenberg
                            },
                            new DriverEntity
                            {
                                Id = 255,
                                DriverGameId = 202611,
                                Name = SeedNames.KevinMagnussen
                            },
                            new DriverEntity
                            {
                                Id = 256,
                                DriverGameId = 202614,
                                Name = SeedNames.SergioPerez
                            },
                            new DriverEntity
                            {
                                Id = 257,
                                DriverGameId = 202615,
                                Name = SeedNames.ValtteriBottas
                            },
                            new DriverEntity
                            {
                                Id = 258,
                                DriverGameId = 202617,
                                Name = SeedNames.EstebanOcon
                            },
                            new DriverEntity
                            {
                                Id = 259,
                                DriverGameId = 202619,
                                Name = SeedNames.LanceStroll
                            },
                            new DriverEntity
                            {
                                Id = 260,
                                DriverGameId = 202620,
                                Name = SeedNames.ArronBarnes
                            },
                            new DriverEntity
                            {
                                Id = 261,
                                DriverGameId = 202621,
                                Name = SeedNames.MartinGiles
                            },
                            new DriverEntity
                            {
                                Id = 262,
                                DriverGameId = 202622,
                                Name = SeedNames.AlexMurray
                            },
                            new DriverEntity
                            {
                                Id = 263,
                                DriverGameId = 202623,
                                Name = SeedNames.LucasRoth
                            },
                            new DriverEntity
                            {
                                Id = 264,
                                DriverGameId = 202624,
                                Name = SeedNames.IgorCorreia
                            },
                            new DriverEntity
                            {
                                Id = 265,
                                DriverGameId = 202625,
                                Name = SeedNames.SophieLevasseur
                            },
                            new DriverEntity
                            {
                                Id = 266,
                                DriverGameId = 202626,
                                Name = SeedNames.JonasSchiffer
                            },
                            new DriverEntity
                            {
                                Id = 267,
                                DriverGameId = 202627,
                                Name = SeedNames.AlainForest
                            },
                            new DriverEntity
                            {
                                Id = 268,
                                DriverGameId = 202628,
                                Name = SeedNames.JayLetourneau
                            },
                            new DriverEntity
                            {
                                Id = 269,
                                DriverGameId = 202629,
                                Name = SeedNames.EstoSaari
                            },
                            new DriverEntity
                            {
                                Id = 270,
                                DriverGameId = 202630,
                                Name = SeedNames.YasarAtiyeh
                            },
                            new DriverEntity
                            {
                                Id = 271,
                                DriverGameId = 202631,
                                Name = SeedNames.CallistoCalabresi
                            },
                            new DriverEntity
                            {
                                Id = 272,
                                DriverGameId = 202632,
                                Name = SeedNames.NaotaIzumi
                            },
                            new DriverEntity
                            {
                                Id = 273,
                                DriverGameId = 202633,
                                Name = SeedNames.HowardClarke
                            },
                            new DriverEntity
                            {
                                Id = 274,
                                DriverGameId = 202634,
                                Name = SeedNames.LarsKaufmann
                            },
                            new DriverEntity
                            {
                                Id = 275,
                                DriverGameId = 202635,
                                Name = SeedNames.MarieLaursen
                            },
                            new DriverEntity
                            {
                                Id = 276,
                                DriverGameId = 202636,
                                Name = SeedNames.FlavioNieves
                            },
                            new DriverEntity
                            {
                                Id = 277,
                                DriverGameId = 202638,
                                Name = SeedNames.KlimekMichalski
                            },
                            new DriverEntity
                            {
                                Id = 278,
                                DriverGameId = 202639,
                                Name = SeedNames.SantiagoMoreno
                            },
                            new DriverEntity
                            {
                                Id = 279,
                                DriverGameId = 202640,
                                Name = SeedNames.BenjaminCoppens
                            },
                            new DriverEntity
                            {
                                Id = 280,
                                DriverGameId = 202641,
                                Name = SeedNames.NoahVisser
                            },
                            new DriverEntity
                            {
                                Id = 281,
                                DriverGameId = 202650,
                                Name = SeedNames.GeorgeRussell
                            },
                            new DriverEntity
                            {
                                Id = 282,
                                DriverGameId = 202654,
                                Name = SeedNames.LandoNorris
                            },
                            new DriverEntity
                            {
                                Id = 283,
                                DriverGameId = 202658,
                                Name = SeedNames.CharlesLeclerc
                            },
                            new DriverEntity
                            {
                                Id = 284,
                                DriverGameId = 202659,
                                Name = SeedNames.PierreGasly
                            },
                            new DriverEntity
                            {
                                Id = 285,
                                DriverGameId = 202662,
                                Name = SeedNames.AlexanderAlbon
                            },
                            new DriverEntity
                            {
                                Id = 286,
                                DriverGameId = 202670,
                                Name = SeedNames.RashidNair
                            },
                            new DriverEntity
                            {
                                Id = 287,
                                DriverGameId = 202671,
                                Name = SeedNames.JackTremblay
                            },
                            new DriverEntity
                            {
                                Id = 288,
                                DriverGameId = 202677,
                                Name = SeedNames.AyrtonSenna
                            },
                            new DriverEntity
                            {
                                Id = 289,
                                DriverGameId = 202680,
                                Name = SeedNames.GuanyuZhou
                            },
                            new DriverEntity
                            {
                                Id = 290,
                                DriverGameId = 202683,
                                Name = SeedNames.JuanManuelCorrea
                            },
                            new DriverEntity
                            {
                                Id = 291,
                                DriverGameId = 202690,
                                Name = SeedNames.MichaelSchumacher
                            },
                            new DriverEntity
                            {
                                Id = 292,
                                DriverGameId = 202694,
                                Name = SeedNames.YukiTsunoda
                            },
                            new DriverEntity
                            {
                                Id = 293,
                                DriverGameId = 2026102,
                                Name = SeedNames.AidanJackson
                            },
                            new DriverEntity
                            {
                                Id = 294,
                                DriverGameId = 2026109,
                                Name = SeedNames.JensonButton
                            },
                            new DriverEntity
                            {
                                Id = 295,
                                DriverGameId = 2026110,
                                Name = SeedNames.DavidCoulthard
                            },
                            new DriverEntity
                            {
                                Id = 296,
                                DriverGameId = 2026112,
                                Name = SeedNames.OscarPiastri
                            },
                            new DriverEntity
                            {
                                Id = 297,
                                DriverGameId = 2026113,
                                Name = SeedNames.LiamLawson
                            },
                            new DriverEntity
                            {
                                Id = 298,
                                DriverGameId = 2026116,
                                Name = SeedNames.RichardVerschoor
                            },
                            new DriverEntity
                            {
                                Id = 299,
                                DriverGameId = 2026123,
                                Name = SeedNames.EnzoFittipaldi
                            },
                            new DriverEntity
                            {
                                Id = 300,
                                DriverGameId = 2026125,
                                Name = SeedNames.MarkWebber
                            },
                            new DriverEntity
                            {
                                Id = 301,
                                DriverGameId = 2026126,
                                Name = SeedNames.JacquesVilleneuve
                            },
                            new DriverEntity
                            {
                                Id = 302,
                                DriverGameId = 2026127,
                                Name = SeedNames.CallieMayer
                            },
                            new DriverEntity
                            {
                                Id = 303,
                                DriverGameId = 2026132,
                                Name = SeedNames.LoganSargeant
                            },
                            new DriverEntity
                            {
                                Id = 304,
                                DriverGameId = 2026136,
                                Name = SeedNames.JackDoohan
                            },
                            new DriverEntity
                            {
                                Id = 305,
                                DriverGameId = 2026137,
                                Name = SeedNames.AmauryCordeel
                            },
                            new DriverEntity
                            {
                                Id = 306,
                                DriverGameId = 2026138,
                                Name = SeedNames.DennisHauger
                            },
                            new DriverEntity
                            {
                                Id = 307,
                                DriverGameId = 2026145,
                                Name = SeedNames.ZaneMaloney
                            },
                            new DriverEntity
                            {
                                Id = 308,
                                DriverGameId = 2026146,
                                Name = SeedNames.VictorMartins
                            },
                            new DriverEntity
                            {
                                Id = 309,
                                DriverGameId = 2026147,
                                Name = SeedNames.OliverBearman
                            },
                            new DriverEntity
                            {
                                Id = 310,
                                DriverGameId = 2026148,
                                Name = SeedNames.JakCrawford
                            },
                            new DriverEntity
                            {
                                Id = 311,
                                DriverGameId = 2026149,
                                Name = SeedNames.IsackHadjar
                            },
                            new DriverEntity
                            {
                                Id = 312,
                                DriverGameId = 2026152,
                                Name = SeedNames.RomanStanek
                            },
                            new DriverEntity
                            {
                                Id = 313,
                                DriverGameId = 2026153,
                                Name = SeedNames.KushMaini
                            },
                            new DriverEntity
                            {
                                Id = 314,
                                DriverGameId = 2026156,
                                Name = SeedNames.BrendonLeigh
                            },
                            new DriverEntity
                            {
                                Id = 315,
                                DriverGameId = 2026157,
                                Name = SeedNames.DavidTonizza
                            },
                            new DriverEntity
                            {
                                Id = 316,
                                DriverGameId = 2026158,
                                Name = SeedNames.JarnoOpmeer
                            },
                            new DriverEntity
                            {
                                Id = 317,
                                DriverGameId = 2026159,
                                Name = SeedNames.LucasBlakeley
                            },
                            new DriverEntity
                            {
                                Id = 318,
                                DriverGameId = 2026160,
                                Name = SeedNames.PaulAron
                            },
                            new DriverEntity
                            {
                                Id = 319,
                                DriverGameId = 2026161,
                                Name = SeedNames.GabrielBortoleto
                            },
                            new DriverEntity
                            {
                                Id = 320,
                                DriverGameId = 2026162,
                                Name = SeedNames.FrancoColapinto
                            },
                            new DriverEntity
                            {
                                Id = 321,
                                DriverGameId = 2026163,
                                Name = SeedNames.TaylorBarnard
                            },
                            new DriverEntity
                            {
                                Id = 322,
                                DriverGameId = 2026164,
                                Name = SeedNames.JoshuaDurksen
                            },
                            new DriverEntity
                            {
                                Id = 323,
                                DriverGameId = 2026165,
                                Name = SeedNames.AndreaKimiAntonelli
                            },
                            new DriverEntity
                            {
                                Id = 324,
                                DriverGameId = 2026166,
                                Name = SeedNames.RitomoMiyata
                            },
                            new DriverEntity
                            {
                                Id = 325,
                                DriverGameId = 2026167,
                                Name = SeedNames.RafaelVillagomez
                            },
                            new DriverEntity
                            {
                                Id = 326,
                                DriverGameId = 2026168,
                                Name = SeedNames.ZakOSullivan
                            },
                            new DriverEntity
                            {
                                Id = 327,
                                DriverGameId = 2026169,
                                Name = SeedNames.PepeMarti
                            },
                            new DriverEntity
                            {
                                Id = 328,
                                DriverGameId = 2026170,
                                Name = SeedNames.SonnyHayes
                            },
                            new DriverEntity
                            {
                                Id = 329,
                                DriverGameId = 2026171,
                                Name = SeedNames.JoshuaPearce
                            },
                            new DriverEntity
                            {
                                Id = 330,
                                DriverGameId = 2026172,
                                Name = SeedNames.CallumVoisin
                            },
                            new DriverEntity
                            {
                                Id = 331,
                                DriverGameId = 2026173,
                                Name = SeedNames.MatiasZagazeta
                            },
                            new DriverEntity
                            {
                                Id = 332,
                                DriverGameId = 2026174,
                                Name = SeedNames.NikolaTsolov
                            },
                            new DriverEntity
                            {
                                Id = 333,
                                DriverGameId = 2026175,
                                Name = SeedNames.TimTramnitz
                            },
                            new DriverEntity
                            {
                                Id = 334,
                                DriverGameId = 2026185,
                                Name = SeedNames.LucaCortez
                            },
                            new DriverEntity
                            {
                                Id = 335,
                                DriverGameId = 2026186,
                                Name = SeedNames.LukeBrowning
                            },
                            new DriverEntity
                            {
                                Id = 336,
                                DriverGameId = 2026187,
                                Name = SeedNames.CianShields
                            },
                            new DriverEntity
                            {
                                Id = 337,
                                DriverGameId = 2026188,
                                Name = SeedNames.ArvidLindblad
                            },
                            new DriverEntity
                            {
                                Id = 338,
                                DriverGameId = 2026189,
                                Name = SeedNames.DinoBeganovic
                            },
                            new DriverEntity
                            {
                                Id = 339,
                                DriverGameId = 2026190,
                                Name = SeedNames.LeonardoFornaroli
                            },
                            new DriverEntity
                            {
                                Id = 340,
                                DriverGameId = 2026191,
                                Name = SeedNames.OliverGoethe
                            },
                            new DriverEntity
                            {
                                Id = 341,
                                DriverGameId = 2026192,
                                Name = SeedNames.GabrieleMini
                            },
                            new DriverEntity
                            {
                                Id = 342,
                                DriverGameId = 2026193,
                                Name = SeedNames.SebastianMontoya
                            },
                            new DriverEntity
                            {
                                Id = 343,
                                DriverGameId = 2026194,
                                Name = SeedNames.AlexanderDunne
                            },
                            new DriverEntity
                            {
                                Id = 344,
                                DriverGameId = 2026195,
                                Name = SeedNames.MaxEsterson
                            },
                            new DriverEntity
                            {
                                Id = 345,
                                DriverGameId = 2026196,
                                Name = SeedNames.SamiMeguetounif
                            },
                            new DriverEntity
                            {
                                Id = 346,
                                DriverGameId = 2026197,
                                Name = SeedNames.JohnBennett
                            },
                            new DriverEntity
                            {
                                Id = 1000,
                                DriverGameId = 255,
                                Name = string.Empty,
                                IsHumanDriver = true
                            });
        }
        catch
        {
            // Ignore exceptions in this step
        }
    }

    #endregion // IEntityTypeConfiguration
}