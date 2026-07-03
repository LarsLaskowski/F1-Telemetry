using System;
using System.IO;
using System.Linq;

using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Interfaces;
using F1Server.Data;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Service.Processors;
using F1Server.Service.Runtime;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F1Server.Tests.Data;

/// <summary>
/// Provides test data
/// </summary>
internal static class TestData
{
    #region Fields

    /// <summary>
    /// Database initialized?
    /// </summary>
    private static bool _isDatabaseInitialized;

    #endregion // Fields

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    static TestData()
    {
        var services = new ServiceCollection();

        services.AddSingleton(new F1ServerApplicationData());
        services.AddSingleton(new PacketAnalyzer());

        ServiceProvider = services.BuildServiceProvider();

        Processor2020 = new PacketProcessor(ServiceProvider, true);
        Processor2021 = new PacketProcessor(ServiceProvider, true);
        Processor2022 = new PacketProcessor(ServiceProvider, true);
        Processor2023 = new PacketProcessor(ServiceProvider, true);
        Processor2024 = new PacketProcessor(ServiceProvider, true);
        Processor2025 = new PacketProcessor(ServiceProvider, true);
        Processor2026 = new PacketProcessor(ServiceProvider, true);
        LiveSessionProcessor = new PacketProcessor(ServiceProvider, true);
    }

    #endregion // Constructor

    #region Properties

    /// <summary>
    /// Service provider instance
    /// </summary>
    public static IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Packet processor for F1 2020
    /// </summary>
    public static PacketProcessor Processor2020 { get; }

    /// <summary>
    /// Packet processor for F1 2021
    /// </summary>
    public static PacketProcessor Processor2021 { get; }

    /// <summary>
    /// Packet processor for F1 2022
    /// </summary>
    public static PacketProcessor Processor2022 { get; }

    /// <summary>
    /// Packet processor for F1 2023
    /// </summary>
    public static PacketProcessor Processor2023 { get; }

    /// <summary>
    /// Packet processor for F1 2024
    /// </summary>
    public static PacketProcessor Processor2024 { get; }

    /// <summary>
    /// Packet processor for F1 2025
    /// </summary>
    public static PacketProcessor Processor2025 { get; }

    /// <summary>
    /// Packet processor for F1 2026
    /// </summary>
    public static PacketProcessor Processor2026 { get; }

    /// <summary>
    /// Packet processor for live session
    /// </summary>
    public static PacketProcessor LiveSessionProcessor { get; }

    /// <summary>
    /// Current game version of live session
    /// </summary>
    public static int LiveSessionGameVersion { get; private set; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Prepare InMemory database
    /// </summary>
    /// <returns>Status</returns>
    public static bool PrepareDatabase()
    {
        if (_isDatabaseInitialized == false)
        {
            var packetAnalyzer = ServiceProvider.GetRequiredService<PacketAnalyzer>();

            PrepareSession2020(packetAnalyzer);
            PrepareSession2021(packetAnalyzer);
            PrepareSession2022(packetAnalyzer);
            PrepareSession2023(packetAnalyzer);
            PrepareSession2024(packetAnalyzer);
            PrepareSession2025(packetAnalyzer);
            PrepareSession2026(packetAnalyzer);

            PrepareLiveSession();

            _isDatabaseInitialized = true;
        }

        return _isDatabaseInitialized;
    }

    #endregion // Methods

    #region Private methods

    /// <summary>
    /// Prepare a F1 2020 session in test database
    /// </summary>
    /// <param name="packetAnalyzer">Packet analyzer</param>
    private static void PrepareSession2020(PacketAnalyzer packetAnalyzer)
    {
        var sessionFile = @"SampleData/F1-2020-MontrealPractice-SessionFirst.packet";
        var isSessionFile = File.Exists(sessionFile);

        Assert.IsTrue(isSessionFile, "File F1-2020-MontrealPractice-SessionFirst.packet is missing!");

        var sessionFileContent = File.ReadAllBytes(sessionFile);

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(sessionFileContent);

        Assert.IsNotNull(packetData.PacketHeader, "Analyzing session packet (2020) failed!");

        var sessionData = packetAnalyzer.GetSessionData(packetData.PacketHeader, sessionFileContent);

        // process packet normally - create a game database entry
        var isProcessed = Processor2020.ProcessPacket(packetData);

        Assert.IsTrue(isProcessed, "Session packet not correctly processed!");

        var sessionProcessor = Processor2020.GetProcessor(packetData.PacketHeader);

        Assert.IsInstanceOfType<SessionProcessor>(sessionProcessor);

        var sessionDataBase = sessionData as ISessionDataBase;

        // process directly to create a sessioin database entry
        isProcessed = sessionProcessor.Process(sessionData,
                                               new SessionRuntimeData(packetData.PacketHeader.GameVersion,
                                                                      packetData.PacketHeader.UniqueSessionId,
                                                                      sessionDataBase?.SessionType ?? SessionType.Unknown));

        Assert.IsTrue(isProcessed, "Session packet not correctly processed (directly)!");
    }

    /// <summary>
    /// Prepare a F1 2021 session in test database
    /// </summary>
    /// <param name="packetAnalyzer">Packet analyzer</param>
    private static void PrepareSession2021(PacketAnalyzer packetAnalyzer)
    {
        var sessionFile = @"SampleData/F1-2021-MonzaRace-SessionFirst.packet";
        var isSessionFile = File.Exists(sessionFile);

        Assert.IsTrue(isSessionFile, "File F1-2021-MonzaRace-SessionFirst.packet is missing!");

        var sessionFileContent = File.ReadAllBytes(sessionFile);

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(sessionFileContent);

        Assert.IsNotNull(packetData.PacketHeader, "Analyzing session packet (2021) failed!");

        var sessionData = packetAnalyzer.GetSessionData(packetData.PacketHeader, sessionFileContent);

        // process packet normally - create a game database entry
        var isProcessed = Processor2021.ProcessPacket(packetData);

        Assert.IsTrue(isProcessed, "Session packet not correctly processed!");

        var sessionProcessor = Processor2021.GetProcessor(packetData.PacketHeader);

        Assert.IsInstanceOfType<SessionProcessor>(sessionProcessor);

        var sessionDataBase = sessionData as ISessionDataBase;

        // process directly to create a sessioin database entry
        isProcessed = sessionProcessor.Process(sessionData,
                                               new SessionRuntimeData(packetData.PacketHeader.GameVersion,
                                                                      packetData.PacketHeader.UniqueSessionId,
                                                                      sessionDataBase?.SessionType ?? SessionType.Unknown));

        Assert.IsTrue(isProcessed, "Session packet not correctly processed (directly)!");
    }

    /// <summary>
    /// Prepare a F1 2022 session in test database
    /// </summary>
    /// <param name="packetAnalyzer">Packet analyzer</param>
    private static void PrepareSession2022(PacketAnalyzer packetAnalyzer)
    {
        var sessionFile = @"SampleData/F1-2022-MiamiQ1-SessionFirst.packet";
        var isSessionFile = File.Exists(sessionFile);

        Assert.IsTrue(isSessionFile, "File F1-2022-MiamiQ1-SessionFirst.packet is missing!");

        var sessionFileContent = File.ReadAllBytes(sessionFile);

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(sessionFileContent);

        Assert.IsNotNull(packetData.PacketHeader, "Analyzing session packet (2022) failed!");

        var sessionData = packetAnalyzer.GetSessionData(packetData.PacketHeader, sessionFileContent);

        // process packet normally - create a game database entry
        var isProcessed = Processor2022.ProcessPacket(packetData);

        Assert.IsTrue(isProcessed, "Session packet not correctly processed!");

        var sessionProcessor = Processor2022.GetProcessor(packetData.PacketHeader);

        Assert.IsInstanceOfType<SessionProcessor>(sessionProcessor);

        var sessionDataBase = sessionData as ISessionDataBase;

        // process directly to create a sessioin database entry
        isProcessed = sessionProcessor.Process(sessionData,
                                               new SessionRuntimeData(packetData.PacketHeader.GameVersion,
                                                                      packetData.PacketHeader.UniqueSessionId,
                                                                      sessionDataBase?.SessionType ?? SessionType.Unknown));

        Assert.IsTrue(isProcessed, "Session packet not correctly processed (directly)!");
    }

    /// <summary>
    /// Prepare a F1 2023 session in test database
    /// </summary>
    /// <param name="packetAnalyzer">Packet analyzer</param>
    private static void PrepareSession2023(PacketAnalyzer packetAnalyzer)
    {
        var sessionFile = @"SampleData/F1-2023-JeddahQ3-SessionFirst.packet";
        var isSessionFile = File.Exists(sessionFile);

        Assert.IsTrue(isSessionFile, "File F1-2023-JeddahQ3-SessionFirst.packet is missing!");

        var sessionFileContent = File.ReadAllBytes(sessionFile);

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(sessionFileContent);

        Assert.IsNotNull(packetData.PacketHeader, "Analyzing session packet (2023) failed!");

        var sessionData = packetAnalyzer.GetSessionData(packetData.PacketHeader, sessionFileContent);

        // process packet normally - create a game database entry
        var isProcessed = Processor2023.ProcessPacket(packetData);

        Assert.IsTrue(isProcessed, "Session packet not correctly processed!");

        var sessionProcessor = Processor2023.GetProcessor(packetData.PacketHeader);

        Assert.IsInstanceOfType<SessionProcessor>(sessionProcessor);

        var sessionDataBase = sessionData as ISessionDataBase;

        // process directly to create a sessioin database entry
        isProcessed = sessionProcessor.Process(sessionData,
                                               new SessionRuntimeData(packetData.PacketHeader.GameVersion,
                                                                      packetData.PacketHeader.UniqueSessionId,
                                                                      sessionDataBase?.SessionType ?? SessionType.Unknown));

        Assert.IsTrue(isProcessed, "Session packet not correctly processed (directly)!");
    }

    /// <summary>
    /// Prepare a F1 2024 session in test database
    /// </summary>
    /// <param name="packetAnalyzer">Packet analyzer</param>
    private static void PrepareSession2024(PacketAnalyzer packetAnalyzer)
    {
        var sessionFile = @"SampleData/F1-2024-ShanghaiSprint-SessionFirst.packet";
        var isSessionFile = File.Exists(sessionFile);

        Assert.IsTrue(isSessionFile, "File F1-2024-ShanghaiSprint-SessionFirst.packet is missing!");

        var sessionFileContent = File.ReadAllBytes(sessionFile);

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(sessionFileContent);

        Assert.IsNotNull(packetData.PacketHeader, "Analyzing session packet (2024) failed!");

        var sessionData = packetAnalyzer.GetSessionData(packetData.PacketHeader, sessionFileContent);

        // process packet normally - create a game database entry
        var isProcessed = Processor2024.ProcessPacket(packetData);

        Assert.IsTrue(isProcessed, "Session packet not correctly processed!");

        var sessionProcessor = Processor2024.GetProcessor(packetData.PacketHeader);

        Assert.IsInstanceOfType<SessionProcessor>(sessionProcessor);

        var sessionDataBase = sessionData as ISessionDataBase;

        // process directly to create a session database entry
        isProcessed = sessionProcessor.Process(sessionData,
                                               new SessionRuntimeData(packetData.PacketHeader.GameVersion,
                                                                      packetData.PacketHeader.UniqueSessionId,
                                                                      sessionDataBase?.SessionType ?? SessionType.Unknown));

        Assert.IsTrue(isProcessed, "Session packet not correctly processed (directly)!");
    }

    /// <summary>
    /// Prepare a F1 2025 session in test database
    /// </summary>
    /// <param name="packetAnalyzer">Packet analyzer</param>
    private static void PrepareSession2025(PacketAnalyzer packetAnalyzer)
    {
        var sessionFile = @"SampleData/F1-2025-MelbourneQ3-SessionFirst.packet";
        var isSessionFile = File.Exists(sessionFile);

        Assert.IsTrue(isSessionFile, "File F1-2025-MelbourneQ3-SessionFirst.packet is missing!");

        var sessionFileContent = File.ReadAllBytes(sessionFile);

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(sessionFileContent);

        Assert.IsNotNull(packetData.PacketHeader, "Analyzing session packet (2025) failed!");

        var sessionData = packetAnalyzer.GetSessionData(packetData.PacketHeader, sessionFileContent);

        // process packet normally - create a game database entry
        var isProcessed = Processor2025.ProcessPacket(packetData);

        Assert.IsTrue(isProcessed, "Session packet not correctly processed!");

        var sessionProcessor = Processor2025.GetProcessor(packetData.PacketHeader);

        Assert.IsInstanceOfType<SessionProcessor>(sessionProcessor);

        var sessionDataBase = sessionData as ISessionDataBase;

        // process directly to create a session database entry
        isProcessed = sessionProcessor.Process(sessionData,
                                               new SessionRuntimeData(packetData.PacketHeader.GameVersion,
                                                                      packetData.PacketHeader.UniqueSessionId,
                                                                      sessionDataBase?.SessionType ?? SessionType.Unknown));

        Assert.IsTrue(isProcessed, "Session packet not correctly processed (directly)!");
    }

    /// <summary>
    /// Prepare a F1 2026 session in test database
    /// </summary>
    /// <param name="packetAnalyzer">Packet analyzer</param>
    private static void PrepareSession2026(PacketAnalyzer packetAnalyzer)
    {
        var sessionFile = @"SampleData/F1-2026-MelbourneQ3-SessionFirst.packet";
        var isSessionFile = File.Exists(sessionFile);

        Assert.IsTrue(isSessionFile, "File F1-2026-MelbourneQ3-SessionFirst.packet is missing!");

        var sessionFileContent = File.ReadAllBytes(sessionFile);

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(sessionFileContent);

        Assert.IsNotNull(packetData.PacketHeader, "Analyzing session packet (2026) failed!");

        var sessionData = packetAnalyzer.GetSessionData(packetData.PacketHeader, sessionFileContent);

        // process packet normally - create a game database entry
        var isProcessed = Processor2026.ProcessPacket(packetData);

        Assert.IsTrue(isProcessed, "Session packet not correctly processed!");

        var sessionProcessor = Processor2026.GetProcessor(packetData.PacketHeader);

        Assert.IsInstanceOfType<SessionProcessor>(sessionProcessor);

        var sessionDataBase = sessionData as ISessionDataBase;

        // process directly to create a session database entry
        isProcessed = sessionProcessor.Process(sessionData,
                                               new SessionRuntimeData(packetData.PacketHeader.GameVersion,
                                                                      packetData.PacketHeader.UniqueSessionId,
                                                                      sessionDataBase?.SessionType ?? SessionType.Unknown));

        Assert.IsTrue(isProcessed, "Session packet not correctly processed (directly)!");
    }

    /// <summary>
    /// Prepare live session for F1 2025 in test database
    /// </summary>
    private static void PrepareLiveSession()
    {
        var eventFile = @"SampleData/F1-2025-MelbourneRace-Event-SessionStart.packet";
        var sessionFile = @"SampleData/F1-2025-MelbourneRace-SessionFirst.packet";
        var isEventFile = File.Exists(eventFile);
        var isSessionFile = File.Exists(sessionFile);

        Assert.IsTrue(isEventFile, "File F1-2025-MelbourneRace-Event-SessionStart.packet is missing!");

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(File.ReadAllBytes(eventFile));

        var isProcessed = LiveSessionProcessor.ProcessPacket(packetData);

        Assert.IsTrue(isProcessed, "Event packet processing failed!");

        Assert.IsTrue(isSessionFile, "File F1-2025-MelbourneRace-SessionFirst.packet is missing!");

        var sessionFileContent = File.ReadAllBytes(sessionFile);

        var sessionPacketData = new ReceivedPacketData();

        sessionPacketData.SetRawData(sessionFileContent);

        Assert.IsNotNull(sessionPacketData.PacketHeader, "Analyzing session packet failed!");

        isProcessed = LiveSessionProcessor.ProcessPacket(sessionPacketData);

        Assert.IsTrue(isProcessed, "Session packet processing failed!");

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var dbSessionData = dbFactory.GetRepository<SessionRepository>()?.GetQuery()?.FirstOrDefault(s => s.SessionId == sessionPacketData.PacketHeader.UniqueSessionId);

            Assert.IsNotNull(dbSessionData, "Session database object is null!");
        }

        LiveSessionGameVersion = LiveSessionProcessor.Session?.GameVersion ?? 0;
    }

    #endregion // Private methods
}