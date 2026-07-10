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
        LiveSessionProcessor2026 = new PacketProcessor(ServiceProvider, true);
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
    /// Packet processor for F1 2026 live session
    /// </summary>
    public static PacketProcessor LiveSessionProcessor2026 { get; }

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

            PrepareSession(packetAnalyzer, @"SampleData/F1-2020-MontrealPractice-SessionFirst.packet", Processor2020, 2020);
            PrepareSession(packetAnalyzer, @"SampleData/F1-2021-MonzaRace-SessionFirst.packet", Processor2021, 2021);
            PrepareSession(packetAnalyzer, @"SampleData/F1-2022-MiamiQ1-SessionFirst.packet", Processor2022, 2022);
            PrepareSession(packetAnalyzer, @"SampleData/F1-2023-JeddahQ3-SessionFirst.packet", Processor2023, 2023);
            PrepareSession(packetAnalyzer, @"SampleData/F1-2024-ShanghaiSprint-SessionFirst.packet", Processor2024, 2024);
            PrepareSession(packetAnalyzer, @"SampleData/F1-2025-MelbourneQ3-SessionFirst.packet", Processor2025, 2025);
            PrepareSession(packetAnalyzer, @"SampleData/F1-2026-MelbourneQ3-SessionFirst.packet", Processor2026, 2026);

            PrepareLiveSession(@"SampleData/F1-2025-MelbourneRace-Event-SessionStart.packet", @"SampleData/F1-2025-MelbourneRace-SessionFirst.packet", LiveSessionProcessor);
            PrepareLiveSession(@"SampleData/F1-2026-MelbourneRace-Event-SessionStart.packet", @"SampleData/F1-2026-MelbourneRace-SessionFirst.packet", LiveSessionProcessor2026);

            LiveSessionGameVersion = LiveSessionProcessor.Session?.GameVersion ?? 0;

            _isDatabaseInitialized = true;
        }

        return _isDatabaseInitialized;
    }

    #endregion // Methods

    #region Private methods

    /// <summary>
    /// Prepare a game session in test database
    /// </summary>
    /// <param name="packetAnalyzer">Packet analyzer</param>
    /// <param name="sessionFile">Path to the sample session packet file</param>
    /// <param name="processor">Packet processor for the game version</param>
    /// <param name="gameVersion">Game version</param>
    private static void PrepareSession(PacketAnalyzer packetAnalyzer, string sessionFile, PacketProcessor processor, int gameVersion)
    {
        var isSessionFile = File.Exists(sessionFile);

        Assert.IsTrue(isSessionFile, $"File {Path.GetFileName(sessionFile)} is missing!");

        var sessionFileContent = File.ReadAllBytes(sessionFile);

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(sessionFileContent);

        Assert.IsNotNull(packetData.PacketHeader, $"Analyzing session packet ({gameVersion}) failed!");

        var sessionData = packetAnalyzer.GetSessionData(packetData.PacketHeader, sessionFileContent);

        // process packet normally - create a game database entry
        var isProcessed = processor.ProcessPacket(packetData);

        Assert.IsTrue(isProcessed, "Session packet not correctly processed!");

        var sessionProcessor = processor.GetProcessor(packetData.PacketHeader);

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
    /// Prepare a live session in test database
    /// </summary>
    /// <param name="eventFile">Path to the sample session start event packet file</param>
    /// <param name="sessionFile">Path to the sample session packet file</param>
    /// <param name="processor">Packet processor for the live session</param>
    private static void PrepareLiveSession(string eventFile, string sessionFile, PacketProcessor processor)
    {
        var isEventFile = File.Exists(eventFile);
        var isSessionFile = File.Exists(sessionFile);

        Assert.IsTrue(isEventFile, $"File {Path.GetFileName(eventFile)} is missing!");

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(File.ReadAllBytes(eventFile));

        var isProcessed = processor.ProcessPacket(packetData);

        Assert.IsTrue(isProcessed, "Event packet processing failed!");

        Assert.IsTrue(isSessionFile, $"File {Path.GetFileName(sessionFile)} is missing!");

        var sessionFileContent = File.ReadAllBytes(sessionFile);

        var sessionPacketData = new ReceivedPacketData();

        sessionPacketData.SetRawData(sessionFileContent);

        Assert.IsNotNull(sessionPacketData.PacketHeader, "Analyzing session packet failed!");

        isProcessed = processor.ProcessPacket(sessionPacketData);

        Assert.IsTrue(isProcessed, "Session packet processing failed!");

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var dbSessionData = dbFactory.GetRepository<SessionRepository>()?.GetQuery()?.FirstOrDefault(s => s.SessionId == sessionPacketData.PacketHeader.UniqueSessionId);

            Assert.IsNotNull(dbSessionData, "Session database object is null!");
        }
    }

    #endregion // Private methods
}