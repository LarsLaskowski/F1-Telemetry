using System.IO;
using System.Linq;

using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Service.Processors;
using F1Server.Service.Runtime;
using F1Server.Tests.Data;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F1Server.Tests.Processors;

/// <summary>
/// Class to test the session processor class
/// </summary>
[TestClass]
public class SessionProcessorTests
{
    #region Fields

    private static PacketAnalyzer _packetAnalyzer;
    private static ReceivedPacketData _packetData20;
    private static ReceivedPacketData _packetData21;
    private static ReceivedPacketData _packetData22;
    private static ReceivedPacketData _packetData23;
    private static ReceivedPacketData _packetData24;
    private static ReceivedPacketData _packetData25;
    private static ReceivedPacketData _packetData26;
    private static ReceivedPacketData _packetDataLive;
    private static SessionData? _sessionData20;
    private static SessionData? _sessionData21;
    private static SessionData? _sessionData22;
    private static SessionData? _sessionData23;
    private static SessionData? _sessionData24;
    private static SessionData? _sessionData25;
    private static SessionData? _sessionData26;
    private static SessionData? _sessionDataLive;

    #endregion // Fields

    #region Initializer/Cleanup

    /// <summary>
    /// Class initializer
    /// </summary>
    /// <param name="testContext">Context</param>
    [ClassInitialize]
    public static void PacketSessionInit(TestContext testContext)
    {
        var isFile20 = File.Exists(@"SampleData/F1-2020-MontrealPractice-Session.packet");
        var isFile21 = File.Exists(@"SampleData/F1-2021-MonzaRace-Session.packet");
        var isFile22 = File.Exists(@"SampleData/F1-2022-MiamiQ1-Session.packet");
        var isFile23 = File.Exists(@"SampleData/F1-2023-JeddahQ3-Session.packet");
        var isFile24 = File.Exists(@"SampleData/F1-2024-ShanghaiSprint-Session.packet");
        var isFile25 = File.Exists(@"SampleData/F1-2025-MelbourneQ3-Session.packet");
        var isFile26 = File.Exists(@"SampleData/F1-2026-MelbourneQ3-Session.packet");
        var isFileLive = File.Exists(@"SampleData/F1-2025-MelbourneRace-Session.packet");

        Assert.IsTrue(isFile20, "File F1-2020-MontrealPractice-Session.packet is missing!");
        Assert.IsTrue(isFile21, "File F1-2021-MonzaRace-Session.packet is missing!");
        Assert.IsTrue(isFile22, "File F1-2022-MiamiQ1-Session.packet is missing!");
        Assert.IsTrue(isFile23, "File F1-2023-JeddahQ3-Session.packet is missing!");
        Assert.IsTrue(isFile24, "File F1-2024-ShanghaiSprint-Session.packet is missing!");
        Assert.IsTrue(isFile25, "File F1-2025-MelbourneQ3-Session.packet is missing!");
        Assert.IsTrue(isFile26, "File F1-2026-MelbourneQ3-Session.packet is missing!");
        Assert.IsTrue(isFileLive, "File F1-2025-MelbourneRace-Session.packet is missing!");

        _packetAnalyzer = new PacketAnalyzer();

        var fileContent = File.ReadAllBytes(@"SampleData/F1-2020-MontrealPractice-Session.packet");

        _packetData20 = new ReceivedPacketData();

        _packetData20.SetRawData(fileContent);

        Assert.IsNotNull(_packetData20.PacketHeader, "Test packet file (2020) is not a session file!");
        Assert.AreEqual(PacketTypes.Session, _packetData20.PacketHeader.PacketType, "Test packet file (2020) is not a session file!");

        var sessionData = _packetAnalyzer.GetSessionData(_packetData20.PacketHeader, _packetData20.PacketRawData);

        if (sessionData is SessionData sessionDataObj20)
        {
            _sessionData20 = sessionDataObj20;
        }

        Assert.IsNotNull(_sessionData20, "Session data object (2020) is null!");

        fileContent = File.ReadAllBytes(@"SampleData/F1-2021-MonzaRace-Session.packet");

        _packetData21 = new ReceivedPacketData();

        _packetData21.SetRawData(fileContent);

        Assert.IsNotNull(_packetData21.PacketHeader, "Test packet file (2021) is not a session file!");
        Assert.AreEqual(PacketTypes.Session, _packetData21.PacketHeader.PacketType, "Test packet file (2021) is not a session file!");

        sessionData = _packetAnalyzer.GetSessionData(_packetData21.PacketHeader, _packetData21.PacketRawData);

        if (sessionData is SessionData sessionDataObj21)
        {
            _sessionData21 = sessionDataObj21;
        }

        Assert.IsNotNull(_sessionData21, "Session data object (2021) is null!");

        fileContent = File.ReadAllBytes(@"SampleData/F1-2022-MiamiQ1-Session.packet");

        _packetData22 = new ReceivedPacketData();

        _packetData22.SetRawData(fileContent);

        Assert.IsNotNull(_packetData22.PacketHeader, "Test packet file (2022) is not a session file!");
        Assert.AreEqual(PacketTypes.Session, _packetData22.PacketHeader.PacketType, "Test packet file (2022) is not a session file!");

        sessionData = _packetAnalyzer.GetSessionData(_packetData22.PacketHeader, _packetData22.PacketRawData);

        if (sessionData is SessionData sessionDataObj22)
        {
            _sessionData22 = sessionDataObj22;
        }

        Assert.IsNotNull(_sessionData22, "Session data object (2022) is null!");

        fileContent = File.ReadAllBytes(@"SampleData/F1-2023-JeddahQ3-Session.packet");

        _packetData23 = new ReceivedPacketData();

        _packetData23.SetRawData(fileContent);

        Assert.IsNotNull(_packetData23.PacketHeader, "Test packet file (2023) is not a session file!");
        Assert.AreEqual(PacketTypes.Session, _packetData23.PacketHeader.PacketType, "Test packet file (2023) is not a session file!");

        sessionData = _packetAnalyzer.GetSessionData(_packetData23.PacketHeader, _packetData23.PacketRawData);

        if (sessionData is SessionData sessionDataObj23)
        {
            _sessionData23 = sessionDataObj23;
        }

        Assert.IsNotNull(_sessionData23, "Session data object (2023) is null!");

        fileContent = File.ReadAllBytes(@"SampleData/F1-2024-ShanghaiSprint-Session.packet");

        _packetData24 = new ReceivedPacketData();

        _packetData24.SetRawData(fileContent);

        Assert.IsNotNull(_packetData24.PacketHeader, "Test packet file (2024) is not a session file!");
        Assert.AreEqual(PacketTypes.Session, _packetData24.PacketHeader.PacketType, "Test packet file (2024) is not a session file!");

        sessionData = _packetAnalyzer.GetSessionData(_packetData24.PacketHeader, _packetData24.PacketRawData);

        if (sessionData is SessionData sessionDataObj24)
        {
            _sessionData24 = sessionDataObj24;
        }

        Assert.IsNotNull(_sessionData24, "Session data object (2024) is null!");

        fileContent = File.ReadAllBytes(@"SampleData/F1-2025-MelbourneQ3-Session.packet");

        _packetData25 = new ReceivedPacketData();

        _packetData25.SetRawData(fileContent);

        Assert.IsNotNull(_packetData25.PacketHeader, "Test packet file (2025) is not a session file!");
        Assert.AreEqual(PacketTypes.Session, _packetData25.PacketHeader.PacketType, "Test packet file (2025) is not a session file!");

        sessionData = _packetAnalyzer.GetSessionData(_packetData25.PacketHeader, _packetData25.PacketRawData);

        if (sessionData is SessionData sessionDataObj25)
        {
            _sessionData25 = sessionDataObj25;
        }

        Assert.IsNotNull(_sessionData25, "Session data object (2025) is null!");

        fileContent = File.ReadAllBytes(@"SampleData/F1-2026-MelbourneQ3-Session.packet");

        _packetData26 = new ReceivedPacketData();

        _packetData26.SetRawData(fileContent);

        Assert.IsNotNull(_packetData26.PacketHeader, "Test packet file (2026) is not a session file!");
        Assert.AreEqual(PacketTypes.Session, _packetData26.PacketHeader.PacketType, "Test packet file (2026) is not a session file!");

        sessionData = _packetAnalyzer.GetSessionData(_packetData26.PacketHeader, _packetData26.PacketRawData);

        if (sessionData is SessionData sessionDataObj26)
        {
            _sessionData26 = sessionDataObj26;
        }

        Assert.IsNotNull(_sessionData26, "Session data object (2026) is null!");

        fileContent = File.ReadAllBytes(@"SampleData/F1-2025-MelbourneRace-Session.packet");

        _packetDataLive = new ReceivedPacketData();

        _packetDataLive.SetRawData(fileContent);

        Assert.IsNotNull(_packetDataLive.PacketHeader, "Test packet file (2025 - Live) is not a session file!");
        Assert.AreEqual(PacketTypes.Session, _packetDataLive.PacketHeader.PacketType, "Test packet file (2025 - Live) is not a session file!");

        sessionData = _packetAnalyzer.GetSessionData(_packetDataLive.PacketHeader, _packetDataLive.PacketRawData);

        if (sessionData is SessionData sessionDataObjLive)
        {
            _sessionDataLive = sessionDataObjLive;
        }

        Assert.IsNotNull(_sessionDataLive, "Session data object (2025 - Live) is null!");
    }

    #endregion // Initializer/Cleanup

    #region Methods

    /// <summary>
    /// Returns game dependent packet header
    /// </summary>
    /// <param name="gameVersion">Version of game</param>
    /// <returns>Packet header</returns>
    private PacketHeader? GetGameDependentPacketHeader(int gameVersion)
    {
        return gameVersion switch
               {
                   2020 => _packetData20.PacketHeader,
                   2021 => _packetData21.PacketHeader,
                   2022 => _packetData22.PacketHeader,
                   2023 => _packetData23.PacketHeader,
                   2024 => _packetData24.PacketHeader,
                   2025 => _packetData25.PacketHeader,
                   2026 => _packetData26.PacketHeader,
                   _ => null
               };
    }

    /// <summary>
    /// Returns a game dependent session data object
    /// </summary>
    /// <param name="gameVersion">Version of game</param>
    /// <returns>Session data object</returns>
    private SessionData? GetGameDependentSessionData(int gameVersion)
    {
        return gameVersion switch
               {
                   2020 => _sessionData20,
                   2021 => _sessionData21,
                   2022 => _sessionData22,
                   2023 => _sessionData23,
                   2024 => _sessionData24,
                   2025 => _sessionData25,
                   2026 => _sessionData26,
                   _ => null
               };
    }

    /// <summary>
    /// Returns a game dependent packet processor
    /// </summary>
    /// <param name="gameVersion">Version of game</param>
    /// <returns>Packet processor object</returns>
    private PacketProcessor? GetGameDependentPacketProcessor(int gameVersion)
    {
        return gameVersion switch
               {
                   2020 => TestData.Processor2020,
                   2021 => TestData.Processor2021,
                   2022 => TestData.Processor2022,
                   2023 => TestData.Processor2023,
                   2024 => TestData.Processor2024,
                   2025 => TestData.Processor2025,
                   2026 => TestData.Processor2026,
                   _ => null
               };
    }

    #endregion // Methods

    #region Test methods

    /// <summary>
    /// Test the expected game version is 2020
    /// </summary>
    [TestMethod]
    public void PacketProcessorExpectedGame2020()
    {
        Assert.AreEqual(2020, TestData.Processor2020.CurrentGame, "Game version is invalid!");
    }

    /// <summary>
    /// Test the expected game version is 2021
    /// </summary>
    [TestMethod]
    public void PacketProcessorExpectedGame2021()
    {
        Assert.AreEqual(2021, TestData.Processor2021.CurrentGame, "Game version is invalid!");
    }

    /// <summary>
    /// Test the expected game version is 2022
    /// </summary>
    [TestMethod]
    public void PacketProcessorExpectedGame2022()
    {
        Assert.AreEqual(2022, TestData.Processor2022.CurrentGame, "Game version is invalid!");
    }

    /// <summary>
    /// Test the expected game version is 2023
    /// </summary>
    [TestMethod]
    public void PacketProcessorExpectedGame2023()
    {
        Assert.AreEqual(2023, TestData.Processor2023.CurrentGame, "Game version is invalid!");
    }

    /// <summary>
    /// Test the expected game version is 2024
    /// </summary>
    [TestMethod]
    public void PacketProcessorExpectedGame2024()
    {
        Assert.AreEqual(2024, TestData.Processor2024.CurrentGame, "Game version is invalid!");
    }

    /// <summary>
    /// Method to test receiving the correct processor for given packet
    /// </summary>
    [TestMethod]
    public void ReceiveProcessorExpectedSessionProcessor2024()
    {
        Assert.IsNotNull(_packetData24.PacketHeader, "Packet header variable is null!");

        var sessionProcessor = TestData.Processor2024.GetProcessor(_packetData24.PacketHeader);

        Assert.IsInstanceOfType<SessionProcessor>(sessionProcessor);
    }

    /// <summary>
    /// Test the expected game version is 2025
    /// </summary>
    [TestMethod]
    public void PacketProcessorExpectedGame2025()
    {
        Assert.AreEqual(2025, TestData.Processor2025.CurrentGame, "Game version is invalid!");
    }

    /// <summary>
    /// Method to test receiving the correct processor for given packet
    /// </summary>
    [TestMethod]
    public void ReceiveProcessorExpectedSessionProcessor2025()
    {
        Assert.IsNotNull(_packetData25.PacketHeader, "Packet header variable is null!");

        var sessionProcessor = TestData.Processor2025.GetProcessor(_packetData25.PacketHeader);

        Assert.IsInstanceOfType<SessionProcessor>(sessionProcessor);
    }

    /// <summary>
    /// Test the expected game version is 2026
    /// </summary>
    [TestMethod]
    public void PacketProcessorExpectedGame2026()
    {
        Assert.AreEqual(2026, TestData.Processor2026.CurrentGame, "Game version is invalid!");
    }

    /// <summary>
    /// Method to test receiving the correct processor for given packet
    /// </summary>
    [TestMethod]
    public void ReceiveProcessorExpectedSessionProcessor2026()
    {
        Assert.IsNotNull(_packetData26.PacketHeader, "Packet header variable is null!");

        var sessionProcessor = TestData.Processor2026.GetProcessor(_packetData26.PacketHeader);

        Assert.IsInstanceOfType<SessionProcessor>(sessionProcessor);
    }

    /// <summary>
    /// Method to test changing weather condition from session packet
    /// </summary>
    /// <param name="gameVersion">Game version</param>
    [TestMethod]
    [DataRow(2020, DisplayName = "ProcessSessionPacket_ExpectedWeatherLightRain2020")]
    [DataRow(2021, DisplayName = "ProcessSessionPacket_ExpectedWeatherLightRain2021")]
    [DataRow(2022, DisplayName = "ProcessSessionPacket_ExpectedWeatherLightRain2022")]
    [DataRow(2023, DisplayName = "ProcessSessionPacket_ExpectedWeatherLightRain2023")]
    [DataRow(2024, DisplayName = "ProcessSessionPacket_ExpectedWeatherLightRain2024")]
    [DataRow(2025, DisplayName = "ProcessSessionPacket_ExpectedWeatherLightRain2025")]
    [DataRow(2026, DisplayName = "ProcessSessionPacket_ExpectedWeatherLightRain2026")]
    public void ProcessSessionPacketExpectedWeatherLightRain(int gameVersion)
    {
        var packetHeader = GetGameDependentPacketHeader(gameVersion);
        var sessionData = GetGameDependentSessionData(gameVersion);
        var processor = GetGameDependentPacketProcessor(gameVersion);

        Assert.IsNotNull(packetHeader, $"Packet header variable ({gameVersion}) is null!");
        Assert.IsNotNull(sessionData, $"Session data ({gameVersion}) is null!");
        Assert.IsNotNull(processor, $"Packet processor ({gameVersion}) is null!");

        if (gameVersion == 2020 && sessionData.PacketData is SessionData2020 sessionData2020)
        {
            sessionData2020.Weather = WeatherCondition.LightRain;
        }
        else if (gameVersion == 2021 && sessionData.PacketData is SessionData2021 sessionData2021)
        {
            sessionData2021.Weather = WeatherCondition.LightRain;
        }
        else if (gameVersion == 2022 && sessionData.PacketData is SessionData2022 sessionData2022)
        {
            sessionData2022.Weather = WeatherCondition.LightRain;
        }
        else if (gameVersion == 2023 && sessionData.PacketData is SessionData2023 sessionData2023)
        {
            sessionData2023.Weather = WeatherCondition.LightRain;
        }
        else if (gameVersion == 2024 && sessionData.PacketData is SessionData2024 sessionData2024)
        {
            sessionData2024.Weather = WeatherCondition.LightRain;
        }
        else if (gameVersion == 2025 && sessionData.PacketData is SessionData2025 sessionData2025)
        {
            sessionData2025.Weather = WeatherCondition.LightRain;
        }
        else if (gameVersion == 2026 && sessionData.PacketData is SessionData2026 sessionData2026)
        {
            sessionData2026.Weather = WeatherCondition.LightRain;
        }
        else
        {
            Assert.Fail("Invalid game version!");
        }

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var dbSessionData = dbFactory.GetRepository<SessionRepository>()?.GetQuery()?.FirstOrDefault(s => s.SessionId == packetHeader.UniqueSessionId);

            Assert.IsNotNull(dbSessionData, $"Session database object ({gameVersion}) is null!");

            var sessionProcessor = processor.GetProcessor(packetHeader);

            Assert.IsInstanceOfType<SessionProcessor>(sessionProcessor);

            var isProcessed = sessionProcessor.Process(sessionData, processor.Session);

            Assert.IsTrue(isProcessed, $"Session packet ({gameVersion}) not correctly processed!");

            var dbSessionAttrData = dbFactory.GetRepository<SessionAttributesRepository>()?.GetQuery()?.FirstOrDefault(s => s.SessionId == dbSessionData.Id);

            Assert.IsNotNull(dbSessionAttrData, $"Session database attributes object ({gameVersion}) is null!");
            Assert.AreEqual(WeatherCondition.LightRain, dbSessionAttrData.WeatherEnd, $"Weather ({gameVersion}) is not changed to light rain!");
        }
    }

    /// <summary>
    /// Method to test changing weather condition from session packet
    /// </summary>
    /// <param name="gameVersion">Game version</param>
    [TestMethod]
    [DataRow(2021, DisplayName = "ProcessSessionPacket_ExpectedSteeringAssistChanged2021")]
    [DataRow(2022, DisplayName = "ProcessSessionPacket_ExpectedSteeringAssistChanged2022")]
    [DataRow(2023, DisplayName = "ProcessSessionPacket_ExpectedSteeringAssistChanged2023")]
    [DataRow(2024, DisplayName = "ProcessSessionPacket_ExpectedSteeringAssistChanged2024")]
    [DataRow(2025, DisplayName = "ProcessSessionPacket_ExpectedSteeringAssistChanged2025")]
    [DataRow(2026, DisplayName = "ProcessSessionPacket_ExpectedSteeringAssistChanged2026")]
    public void ProcessSessionPacketExpectedSteeringAssistChanged(int gameVersion)
    {
        var packetHeader = GetGameDependentPacketHeader(gameVersion);
        var sessionData = GetGameDependentSessionData(gameVersion);
        var processor = GetGameDependentPacketProcessor(gameVersion);

        Assert.IsNotNull(packetHeader, $"Packet header variable ({gameVersion}) is null!");
        Assert.IsNotNull(sessionData, $"Session data ({gameVersion}) is null!");
        Assert.IsNotNull(processor, $"Packet processor ({gameVersion}) is null!");

        if (gameVersion == 2021 && sessionData.PacketData is SessionData2021 sessionData2021)
        {
            sessionData2021.IsSteeringAssist = true;
        }
        else if (gameVersion == 2022 && sessionData.PacketData is SessionData2022 sessionData2022)
        {
            sessionData2022.IsSteeringAssist = true;
        }
        else if (gameVersion == 2023 && sessionData.PacketData is SessionData2023 sessionData2023)
        {
            sessionData2023.IsSteeringAssist = true;
        }
        else if (gameVersion == 2024 && sessionData.PacketData is SessionData2024 sessionData2024)
        {
            sessionData2024.IsSteeringAssist = true;
        }
        else if (gameVersion == 2025 && sessionData.PacketData is SessionData2025 sessionData2025)
        {
            sessionData2025.IsSteeringAssist = true;
        }
        else if (gameVersion == 2026 && sessionData.PacketData is SessionData2026 sessionData2026)
        {
            sessionData2026.IsSteeringAssist = true;
        }
        else
        {
            Assert.Fail("Invalid game version!");
        }

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var dbSessionData = dbFactory.GetRepository<SessionRepository>()?.GetQuery()?.FirstOrDefault(s => s.SessionId == packetHeader.UniqueSessionId);

            Assert.IsNotNull(dbSessionData, $"Session database object ({gameVersion}) is null!");

            var dbSessionAttrData = dbFactory.GetRepository<SessionAttributesRepository>()?.GetQuery()?.FirstOrDefault(s => s.SessionId == dbSessionData.Id);

            Assert.IsNotNull(dbSessionAttrData, $"Session database attributes object ({gameVersion}) is null!");
            Assert.IsFalse(dbSessionAttrData.SteeringAssistChanged, $"Steering assist ({gameVersion}) is changed!");

            var sessionProcessor = processor.GetProcessor(packetHeader);

            Assert.IsInstanceOfType<SessionProcessor>(sessionProcessor);

            var isProcessed = sessionProcessor.Process(sessionData, processor.Session);

            Assert.IsTrue(isProcessed, $"Session packet ({gameVersion}) not correctly processed!");

            dbSessionAttrData = dbFactory.GetRepository<SessionAttributesRepository>()?.GetQuery()?.FirstOrDefault(s => s.SessionId == dbSessionData.Id);

            Assert.IsNotNull(dbSessionAttrData, $"Session database attributes object ({gameVersion}) is null!");
            Assert.IsTrue(dbSessionAttrData.SteeringAssistChanged, $"Steering assist ({gameVersion}) is not changed!");
        }
    }

    /// <summary>
    /// Method to test changing weather condition from session packet
    /// </summary>
    /// <param name="gameVersion">Game version</param>
    [TestMethod]
    [DataRow(2021, DisplayName = "ProcessSessionPacket_ExpectedSteeringAssistChanged2021")]
    [DataRow(2022, DisplayName = "ProcessSessionPacket_ExpectedSteeringAssistChanged2022")]
    [DataRow(2023, DisplayName = "ProcessSessionPacket_ExpectedSteeringAssistChanged2023")]
    [DataRow(2024, DisplayName = "ProcessSessionPacket_ExpectedSteeringAssistChanged2024")]
    [DataRow(2025, DisplayName = "ProcessSessionPacket_ExpectedSteeringAssistChanged2025")]
    [DataRow(2026, DisplayName = "ProcessSessionPacket_ExpectedSteeringAssistChanged2026")]
    public void ProcessSessionPacketExpectedBrakingAssistChanged(int gameVersion)
    {
        var packetHeader = GetGameDependentPacketHeader(gameVersion);
        var sessionData = GetGameDependentSessionData(gameVersion);
        var processor = GetGameDependentPacketProcessor(gameVersion);

        Assert.IsNotNull(packetHeader, $"Packet header variable ({gameVersion}) is null!");
        Assert.IsNotNull(sessionData, $"Session data ({gameVersion}) is null!");
        Assert.IsNotNull(processor, $"Packet processor ({gameVersion}) is null!");

        if (gameVersion == 2021 && sessionData.PacketData is SessionData2021 sessionData2021)
        {
            sessionData2021.BrakingAssist = BrakingAssist.Medium;
        }
        else if (gameVersion == 2022 && sessionData.PacketData is SessionData2022 sessionData2022)
        {
            sessionData2022.BrakingAssist = BrakingAssist.Medium;
        }
        else if (gameVersion == 2023 && sessionData.PacketData is SessionData2023 sessionData2023)
        {
            sessionData2023.BrakingAssist = BrakingAssist.Medium;
        }
        else if (gameVersion == 2024 && sessionData.PacketData is SessionData2024 sessionData2024)
        {
            sessionData2024.BrakingAssist = BrakingAssist.Medium;
        }
        else if (gameVersion == 2025 && sessionData.PacketData is SessionData2025 sessionData2025)
        {
            sessionData2025.BrakingAssist = BrakingAssist.Medium;
        }
        else if (gameVersion == 2026 && sessionData.PacketData is SessionData2026 sessionData2026)
        {
            sessionData2026.BrakingAssist = BrakingAssist.Medium;
        }
        else
        {
            Assert.Fail("Invalid game version!");
        }

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var dbSessionData = dbFactory.GetRepository<SessionRepository>()?.GetQuery()?.FirstOrDefault(s => s.SessionId == packetHeader.UniqueSessionId);

            Assert.IsNotNull(dbSessionData, $"Session database object ({gameVersion}) is null!");

            var dbSessionAttrData = dbFactory.GetRepository<SessionAttributesRepository>()?.GetQuery()?.FirstOrDefault(s => s.SessionId == dbSessionData.Id);

            Assert.IsNotNull(dbSessionAttrData, $"Session database attributes object ({gameVersion}) is null!");
            Assert.IsFalse(dbSessionAttrData.BrakingAssistChanged, $"Braking assitant ({gameVersion}) is changed!");

            var sessionProcessor = processor.GetProcessor(packetHeader);

            Assert.IsInstanceOfType<SessionProcessor>(sessionProcessor);

            var isProcessed = sessionProcessor.Process(sessionData, processor.Session);

            Assert.IsTrue(isProcessed, $"Session packet ({gameVersion}) not correctly processed!");

            dbSessionAttrData = dbFactory.GetRepository<SessionAttributesRepository>()?.GetQuery()?.FirstOrDefault(s => s.SessionId == dbSessionData.Id);

            Assert.IsNotNull(dbSessionAttrData, $"Session database attributes object ({gameVersion}) is null!");
            Assert.IsTrue(dbSessionAttrData.BrakingAssistChanged, $"Braking assistant ({gameVersion}) is not changed!");
        }
    }

    /// <summary>
    /// Method to test changing weather condition from session packet
    /// </summary>
    /// <param name="gameVersion">Game version</param>
    [TestMethod]
    [DataRow(2021, DisplayName = "ProcessSessionPacket_ExpectedSteeringAssistChanged2021")]
    [DataRow(2022, DisplayName = "ProcessSessionPacket_ExpectedSteeringAssistChanged2022")]
    [DataRow(2023, DisplayName = "ProcessSessionPacket_ExpectedSteeringAssistChanged2023")]
    [DataRow(2024, DisplayName = "ProcessSessionPacket_ExpectedSteeringAssistChanged2024")]
    [DataRow(2025, DisplayName = "ProcessSessionPacket_ExpectedSteeringAssistChanged2025")]
    public void ProcessSessionPacketExpectedGearBoxAssistChanged(int gameVersion)
    {
        var packetHeader = GetGameDependentPacketHeader(gameVersion);
        var sessionData = GetGameDependentSessionData(gameVersion);
        var processor = GetGameDependentPacketProcessor(gameVersion);

        Assert.IsNotNull(packetHeader, $"Packet header variable ({gameVersion}) is null!");
        Assert.IsNotNull(sessionData, $"Session data ({gameVersion}) is null!");
        Assert.IsNotNull(processor, $"Packet processor ({gameVersion}) is null!");

        if (gameVersion == 2021 && sessionData.PacketData is SessionData2021 sessionData2021)
        {
            sessionData2021.GearboxAssist = GearboxAssist.ManualSuggestedGear;
        }
        else if (gameVersion == 2022 && sessionData.PacketData is SessionData2022 sessionData2022)
        {
            sessionData2022.GearboxAssist = GearboxAssist.ManualSuggestedGear;
        }
        else if (gameVersion == 2023 && sessionData.PacketData is SessionData2023 sessionData2023)
        {
            sessionData2023.GearboxAssist = GearboxAssist.ManualSuggestedGear;
        }
        else if (gameVersion == 2024 && sessionData.PacketData is SessionData2024 sessionData2024)
        {
            sessionData2024.GearboxAssist = GearboxAssist.ManualSuggestedGear;
        }
        else if (gameVersion == 2025 && sessionData.PacketData is SessionData2025 sessionData2025)
        {
            sessionData2025.GearboxAssist = GearboxAssist.ManualSuggestedGear;
        }
        else if (gameVersion == 2026 && sessionData.PacketData is SessionData2026 sessionData2026)
        {
            sessionData2026.GearboxAssist = GearboxAssist.ManualSuggestedGear;
        }
        else
        {
            Assert.Fail("Invalid game version!");
        }

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var dbSessionData = dbFactory.GetRepository<SessionRepository>()?.GetQuery()?.FirstOrDefault(s => s.SessionId == packetHeader.UniqueSessionId);

            Assert.IsNotNull(dbSessionData, $"Session database object ({gameVersion}) is null!");

            var dbSessionAttrData = dbFactory.GetRepository<SessionAttributesRepository>()?.GetQuery()?.FirstOrDefault(s => s.SessionId == dbSessionData.Id);

            Assert.IsNotNull(dbSessionAttrData, $"Session database attributes object ({gameVersion}) is null!");

            Assert.IsFalse(dbSessionAttrData.GearBoxAssistChanged, $"Gearbox assistant ({gameVersion}) is changed!");

            var sessionProcessor = processor.GetProcessor(packetHeader);

            Assert.IsInstanceOfType<SessionProcessor>(sessionProcessor);

            var isProcessed = sessionProcessor.Process(sessionData, processor.Session);

            Assert.IsTrue(isProcessed, $"Session packet ({gameVersion}) not correctly processed!");

            dbSessionAttrData = dbFactory.GetRepository<SessionAttributesRepository>()?.GetQuery()?.FirstOrDefault(s => s.SessionId == dbSessionData.Id);

            Assert.IsNotNull(dbSessionAttrData, $"Session database attributes object ({gameVersion}) is null!");

            Assert.IsTrue(dbSessionAttrData.GearBoxAssistChanged, $"Gearbox assistant ({gameVersion}) is not changed!");
        }
    }

    /// <summary>
    /// Method to test receiving the correct processor for given packet
    /// </summary>
    [TestMethod]
    public void ProcessSessionPacketExpectedSafetyCar()
    {
        var packetHeader = _packetDataLive.PacketHeader;
        var sessionData = _sessionDataLive;

        Assert.IsNotNull(packetHeader, $"Packet header variable ({TestData.LiveSessionGameVersion}) is null!");
        Assert.IsNotNull(sessionData, $"sessionData ({TestData.LiveSessionGameVersion}) is null!");

        if (sessionData.PacketData is SessionData2025 sessionData2025)
        {
            sessionData2025.SafetyCar = SafetyCarStatus.SafetyCar;
        }
        else
        {
            Assert.Fail("Invalid game version!");
        }

        Assert.IsNotNull(TestData.LiveSessionProcessor.Session?.CurrentSession, $"Current session data ({TestData.LiveSessionGameVersion}) is null!");
        Assert.IsFalse(TestData.LiveSessionProcessor.Session.CurrentSession.IsSafetyCar, $"Safety car status ({TestData.LiveSessionGameVersion}) is wrong, should be false!");

        var sessionProcessor = TestData.LiveSessionProcessor.GetProcessor(packetHeader);

        Assert.IsInstanceOfType<SessionProcessor>(sessionProcessor);

        var isProcessed = sessionProcessor.Process(sessionData, TestData.LiveSessionProcessor.Session);

        Assert.IsTrue(isProcessed, $"Session packet ({TestData.LiveSessionGameVersion}) not correctly processed!");

        Assert.IsTrue(TestData.LiveSessionProcessor.Session.CurrentSession.IsSafetyCar, $"Safety car status ({TestData.LiveSessionGameVersion}) is wrong, should be true!");
    }

    #endregion // Test methods
}