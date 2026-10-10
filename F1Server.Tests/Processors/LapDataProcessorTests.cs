using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;
using F1Server.Data;
using F1Server.Db.Entity.Tables;
using F1Server.Service.Processors;
using F1Server.Service.Runtime;
using F1Server.Tests.Data;

using Microsoft.Extensions.DependencyInjection;

namespace F1Server.Tests.Processors;

/// <summary>
/// Class to test the lap data processor class
/// </summary>
[TestClass]
public class LapDataProcessorTests
{
    #region Constants

    /// <summary>
    /// Database id of the test session
    /// </summary>
    private const long SessionDbId = 450100;

    /// <summary>
    /// Current lap time of the synthetic lap in milliseconds
    /// </summary>
    private const uint CurrentLapTimeMs = 90500;

    /// <summary>
    /// Last lap time of the synthetic lap in milliseconds
    /// </summary>
    private const uint LastLapTimeMs = 91000;

    /// <summary>
    /// Sector 1 time of the synthetic lap in milliseconds
    /// </summary>
    private const uint Sector1TimeMs = 30250;

    /// <summary>
    /// Sector 2 time of the synthetic lap in milliseconds
    /// </summary>
    private const uint Sector2TimeMs = 30500;

    #endregion // Constants

    #region Static methods

    /// <summary>
    /// Creates a recordable session runtime data object
    /// </summary>
    /// <param name="gameVersion">Game version</param>
    /// <param name="sessionType">Session type</param>
    /// <returns>Session runtime data</returns>
    private static SessionRuntimeData CreateSessionRuntimeData(int gameVersion, SessionType sessionType)
    {
        return new SessionRuntimeData(gameVersion, (ulong)SessionDbId, sessionType)
               {
                   HasParticipants = true,
                   IsRecordable = true,
                   SessionDbId = SessionDbId,
                   CurrentSession = new LiveSessionData
                                    {
                                        DbId = SessionDbId,
                                        SessionGameId = (ulong)SessionDbId,
                                        SessionType = sessionType
                                    }
               };
    }

    /// <summary>
    /// Creates a participant with live data and registers it at car index 0
    /// </summary>
    /// <param name="sessionRuntimeData">Session runtime data</param>
    /// <param name="participantDbId">Database id of the participant</param>
    /// <returns>Participant runtime data</returns>
    private static ParticipantRuntimeData AddParticipant(SessionRuntimeData sessionRuntimeData, long participantDbId)
    {
        var participantRuntimeData = new ParticipantRuntimeData(sessionRuntimeData)
                                     {
                                         IsValidObject = true,
                                         ParticipantDbId = participantDbId,
                                         LiveData = new LiveDriverData
                                                    {
                                                        ArrayIndex = 0
                                                    }
                                     };

        Assert.IsTrue(sessionRuntimeData.Participants.TryAdd(0, participantRuntimeData), "Participant runtime data could not be registered!");

        return participantRuntimeData;
    }

    /// <summary>
    /// Creates a F1 2025 lap data packet with the given lap information on car index 0
    /// </summary>
    /// <param name="lapNumber">Current lap number</param>
    /// <param name="driverStatus">Driver status</param>
    /// <param name="lapDistance">Lap distance</param>
    /// <param name="lastLapTime">Last lap time in milliseconds</param>
    /// <returns>Lap data packet</returns>
    private static LapData CreateLapData2025(ushort lapNumber, DriverStatus driverStatus, float lapDistance, uint lastLapTime)
    {
        var lapDataComplete = new LapDataComplete2025();

        lapDataComplete.LapData[0] = new LapData2025
                                     {
                                         CurrentLapNumber = lapNumber,
                                         CurrentDriverStatus = driverStatus,
                                         CurrentSector = Sector.Sector1,
                                         LapDistance = lapDistance,
                                         CarPosition = 3,
                                         GridPosition = 4,
                                         CurrentLapTime = CurrentLapTimeMs,
                                         LastLapTime = lastLapTime,
                                         Sector1Time = (ushort)Sector1TimeMs,
                                         Sector2Time = (ushort)Sector2TimeMs
                                     };

        return CreateLapData(2025, lapDataComplete);
    }

    /// <summary>
    /// Creates a lap data packet with a flying lap on car index 0 for the given game version
    /// </summary>
    /// <param name="gameVersion">Game version</param>
    /// <returns>Lap data packet</returns>
    private static LapData CreateFlyingLapData(int gameVersion)
    {
        ILapDataComplete lapDataComplete = gameVersion switch
                                           {
                                               2019 => new LapDataComplete2019(),
                                               2020 => new LapDataComplete2020(),
                                               2021 => new LapDataComplete2021(),
                                               2022 => new LapDataComplete2022(),
                                               2023 => new LapDataComplete2023(),
                                               2024 => new LapDataComplete2024(),
                                               2026 => new LapDataComplete2026(),
                                               _ => new LapDataComplete2025()
                                           };

        lapDataComplete.LapData[0] = gameVersion switch
                                     {
                                         2019 => new LapData2019
                                                 {
                                                     CurrentLapNumber = 1,
                                                     CurrentDriverStatus = DriverStatus.FlyingLap,
                                                     CurrentLapTime = CurrentLapTimeMs / 1000f,
                                                     Sector1Time = Sector1TimeMs / 1000f,
                                                     Sector2Time = Sector2TimeMs / 1000f
                                                 },
                                         2020 => new LapData2020
                                                 {
                                                     CurrentLapNumber = 1,
                                                     CurrentDriverStatus = DriverStatus.FlyingLap,
                                                     CurrentLapTime = CurrentLapTimeMs / 1000f,
                                                     Sector1Time = (ushort)Sector1TimeMs,
                                                     Sector2Time = (ushort)Sector2TimeMs
                                                 },
                                         2021 => new LapData2021
                                                 {
                                                     CurrentLapNumber = 1,
                                                     CurrentDriverStatus = DriverStatus.FlyingLap,
                                                     CurrentLapTime = CurrentLapTimeMs,
                                                     Sector1Time = (ushort)Sector1TimeMs,
                                                     Sector2Time = (ushort)Sector2TimeMs
                                                 },
                                         2022 => new LapData2022
                                                 {
                                                     CurrentLapNumber = 1,
                                                     CurrentDriverStatus = DriverStatus.FlyingLap,
                                                     CurrentLapTime = CurrentLapTimeMs,
                                                     Sector1Time = (ushort)Sector1TimeMs,
                                                     Sector2Time = (ushort)Sector2TimeMs
                                                 },
                                         2023 => new LapData2023
                                                 {
                                                     CurrentLapNumber = 1,
                                                     CurrentDriverStatus = DriverStatus.FlyingLap,
                                                     CurrentLapTime = CurrentLapTimeMs,
                                                     Sector1Time = (ushort)Sector1TimeMs,
                                                     Sector2Time = (ushort)Sector2TimeMs
                                                 },
                                         2024 => new LapData2024
                                                 {
                                                     CurrentLapNumber = 1,
                                                     CurrentDriverStatus = DriverStatus.FlyingLap,
                                                     CurrentLapTime = CurrentLapTimeMs,
                                                     Sector1Time = (ushort)Sector1TimeMs,
                                                     Sector2Time = (ushort)Sector2TimeMs
                                                 },
                                         2026 => new LapData2026
                                                 {
                                                     CurrentLapNumber = 1,
                                                     CurrentDriverStatus = DriverStatus.FlyingLap,
                                                     CurrentLapTime = CurrentLapTimeMs,
                                                     Sector1Time = (ushort)Sector1TimeMs,
                                                     Sector2Time = (ushort)Sector2TimeMs
                                                 },
                                         _ => new LapData2025
                                              {
                                                  CurrentLapNumber = 1,
                                                  CurrentDriverStatus = DriverStatus.FlyingLap,
                                                  CurrentLapTime = CurrentLapTimeMs,
                                                  Sector1Time = (ushort)Sector1TimeMs,
                                                  Sector2Time = (ushort)Sector2TimeMs
                                              }
                                     };

        return CreateLapData(gameVersion, lapDataComplete);
    }

    /// <summary>
    /// Creates a lap data packet object
    /// </summary>
    /// <param name="gameVersion">Game version</param>
    /// <param name="lapDataComplete">Lap data of all cars</param>
    /// <returns>Lap data packet</returns>
    private static LapData CreateLapData(int gameVersion, ILapDataComplete lapDataComplete)
    {
        var packetHeader = new PacketHeader
                           {
                               GameVersion = (ushort)gameVersion,
                               PacketType = PacketTypes.LapData,
                               UniqueSessionId = (ulong)SessionDbId,
                               FrameIdentifier = 100
                           };

        return new LapData(packetHeader, lapDataComplete);
    }

    /// <summary>
    /// Creates a lap data processor
    /// </summary>
    /// <param name="lapData">Lap data packet</param>
    /// <returns>Lap data processor</returns>
    private static LapDataProcessor CreateProcessor(LapData lapData)
    {
        return new LapDataProcessor(TestData.ServiceProvider,
                                    lapData.PacketHeader,
                                    new LiveGameData
                                    {
                                        GameVersion = lapData.PacketHeader.GameVersion
                                    });
    }

    /// <summary>
    /// Creates a lap data processor using the given telemetry writer
    /// </summary>
    /// <param name="lapData">Lap data packet</param>
    /// <param name="telemetryWriter">Telemetry writer</param>
    /// <returns>Lap data processor</returns>
    private static LapDataProcessor CreateProcessor(LapData lapData, RecordingTelemetryWriter telemetryWriter)
    {
        var services = new ServiceCollection();

        services.AddSingleton(new F1ServerApplicationData
                              {
                                  TelemetryWriter = telemetryWriter
                              });

        return new LapDataProcessor(services.BuildServiceProvider(),
                                    lapData.PacketHeader,
                                    new LiveGameData
                                    {
                                        GameVersion = lapData.PacketHeader.GameVersion
                                    });
    }

    #endregion // Static methods

    #region Test methods

    /// <summary>
    /// A data object that is no lap data packet is ignored without reporting an error
    /// </summary>
    [TestMethod]
    public void LapDataProcessorUnexpectedDataObjectReturnsTrue()
    {
        var lapData = CreateLapData2025(1, DriverStatus.FlyingLap, 100, 0);
        var lapDataProcessor = CreateProcessor(lapData);

        var isProcessed = lapDataProcessor.Process(new object(), CreateSessionRuntimeData(2025, SessionType.Practice1));

        Assert.IsTrue(isProcessed, "An unexpected data object should be ignored without an error!");
        Assert.AreEqual(string.Empty, lapDataProcessor.LastException, "Ignoring an unexpected data object must not record an exception!");
    }

    /// <summary>
    /// A lap data packet of a session that is not recordable is ignored
    /// </summary>
    [TestMethod]
    public void LapDataProcessorNotRecordableSessionDoesNotUpdateSession()
    {
        var sessionRuntimeData = CreateSessionRuntimeData(2025, SessionType.Practice1);

        sessionRuntimeData.IsRecordable = false;
        sessionRuntimeData.CurrentSession.CurrentCarsOnTrack = 7;

        var lapData = CreateLapData2025(1, DriverStatus.FlyingLap, 100, 0);

        var isProcessed = CreateProcessor(lapData).Process(lapData, sessionRuntimeData);

        Assert.IsTrue(isProcessed, "A lap data packet of a not recordable session should be ignored without an error!");
        Assert.AreEqual(7, sessionRuntimeData.CurrentSession.CurrentCarsOnTrack, "The session information must not be updated for a not recordable session!");
    }

    /// <summary>
    /// A lap data packet of a finished session is ignored
    /// </summary>
    [TestMethod]
    public void LapDataProcessorFinishedSessionDoesNotUpdateSession()
    {
        var sessionRuntimeData = CreateSessionRuntimeData(2025, SessionType.Race);

        sessionRuntimeData.CurrentSession.IsFinished = true;
        sessionRuntimeData.CurrentSession.CurrentCarsOnTrack = 7;

        var lapData = CreateLapData2025(1, DriverStatus.OnTrack, 100, 0);

        var isProcessed = CreateProcessor(lapData).Process(lapData, sessionRuntimeData);

        Assert.IsTrue(isProcessed, "A lap data packet of a finished session should be ignored without an error!");
        Assert.AreEqual(7, sessionRuntimeData.CurrentSession.CurrentCarsOnTrack, "The session information must not be updated for a finished session!");
    }

    /// <summary>
    /// A lap data packet without registered participants does not update the session information
    /// </summary>
    [TestMethod]
    public void LapDataProcessorWithoutParticipantsDoesNotUpdateSession()
    {
        var sessionRuntimeData = CreateSessionRuntimeData(2025, SessionType.Practice1);

        sessionRuntimeData.HasParticipants = false;
        sessionRuntimeData.CurrentSession.CurrentCarsOnTrack = 7;

        var lapData = CreateLapData2025(1, DriverStatus.FlyingLap, 100, 0);

        var isProcessed = CreateProcessor(lapData).Process(lapData, sessionRuntimeData);

        Assert.IsTrue(isProcessed, "A lap data packet without participants should be processed without an error!");
        Assert.AreEqual(7, sessionRuntimeData.CurrentSession.CurrentCarsOnTrack, "The session information must not be updated without participants!");
    }

    /// <summary>
    /// An error while processing a lap is reported as failed processing with the exception
    /// </summary>
    [TestMethod]
    public void LapDataProcessorMissingLapEntryReturnsFalse()
    {
        var sessionRuntimeData = CreateSessionRuntimeData(2025, SessionType.Practice1);

        AddParticipant(sessionRuntimeData, 450101);

        var lapDataComplete = new LapDataComplete2025();

        lapDataComplete.LapData[0] = null!;

        var lapData = CreateLapData(2025, lapDataComplete);
        var lapDataProcessor = CreateProcessor(lapData);

        var isProcessed = lapDataProcessor.Process(lapData, sessionRuntimeData);

        Assert.IsFalse(isProcessed, "A missing lap entry of a registered participant should fail the processing!");
        Assert.IsFalse(string.IsNullOrEmpty(lapDataProcessor.LastException), "The processing error should be recorded as last exception!");
    }

    /// <summary>
    /// The lap times of every supported game version are converted into the game independent live data
    /// </summary>
    /// <param name="gameVersion">Game version</param>
    /// <param name="expectedLapsDriven">Expected number of driven laps after the first lap entity is created</param>
    [TestMethod]
    [DataRow(2019, 1)]
    [DataRow(2020, 1)]
    [DataRow(2021, 0)]
    [DataRow(2022, 0)]
    [DataRow(2023, 0)]
    [DataRow(2024, 0)]
    [DataRow(2025, 0)]
    [DataRow(2026, 0)]
    public void LapDataProcessorFlyingLapUpdatesLiveData(int gameVersion, int expectedLapsDriven)
    {
        var sessionRuntimeData = CreateSessionRuntimeData(gameVersion, SessionType.Practice1);
        var participantRuntimeData = AddParticipant(sessionRuntimeData, 450110 + gameVersion);
        var lapData = CreateFlyingLapData(gameVersion);

        var isProcessed = CreateProcessor(lapData).Process(lapData, sessionRuntimeData);

        Assert.IsTrue(isProcessed, $"F1 {gameVersion} lap data packet not correctly processed!");
        Assert.IsNotNull(participantRuntimeData.LiveData, "The live data of the participant should still be available!");
        Assert.AreEqual(CurrentLapTimeMs, participantRuntimeData.LiveData.CurrentLapTime, $"The current lap time of F1 {gameVersion} should be converted to milliseconds!");
        Assert.AreEqual(Sector1TimeMs, participantRuntimeData.LiveData.FastestSector1, $"The sector 1 time of F1 {gameVersion} should be the fastest sector 1 of the driver!");
        Assert.AreEqual(Sector2TimeMs, participantRuntimeData.LiveData.FastestSector2, $"The sector 2 time of F1 {gameVersion} should be the fastest sector 2 of the driver!");
        Assert.AreEqual(Sector1TimeMs, sessionRuntimeData.CurrentSession.FastestSector1, $"The sector 1 time of F1 {gameVersion} should be the fastest sector 1 of the session!");
        Assert.AreEqual(Sector2TimeMs, sessionRuntimeData.CurrentSession.FastestSector2, $"The sector 2 time of F1 {gameVersion} should be the fastest sector 2 of the session!");
        Assert.AreEqual(expectedLapsDriven, participantRuntimeData.LiveData.LapsDriven, $"The driven laps of F1 {gameVersion} are not counted as expected!");
        Assert.AreEqual(1, sessionRuntimeData.CurrentSession.CurrentCarsOnTrack, "The car on a flying lap should be counted as on track!");
        Assert.IsNotNull(participantRuntimeData.GetLap(1), "A lap entity should be started for the recordable lap!");
    }

    /// <summary>
    /// An unsupported game version does not provide any lap times
    /// </summary>
    [TestMethod]
    public void LapDataProcessorUnknownGameVersionHasNoLapTimes()
    {
        var sessionRuntimeData = CreateSessionRuntimeData(2018, SessionType.Practice1);
        var participantRuntimeData = AddParticipant(sessionRuntimeData, 450130);
        var lapData = CreateFlyingLapData(2018);

        var isProcessed = CreateProcessor(lapData).Process(lapData, sessionRuntimeData);

        Assert.IsTrue(isProcessed, "Lap data packet of an unknown game version should be processed without an error!");
        Assert.IsNotNull(participantRuntimeData.LiveData, "The live data of the participant should still be available!");
        Assert.AreEqual(0u, participantRuntimeData.LiveData.CurrentLapTime, "An unknown game version must not provide a current lap time!");
        Assert.AreEqual(0u, sessionRuntimeData.CurrentSession.FastestSector1, "An unknown game version must not provide sector times!");
    }

    /// <summary>
    /// The driver status decides depending on the session type whether a lap is recordable
    /// </summary>
    /// <param name="sessionType">Session type</param>
    /// <param name="driverStatus">Driver status</param>
    /// <param name="lapNumber">Current lap number</param>
    /// <param name="expectedIsRecordable">Expected recordable state</param>
    /// <param name="expectedCarsOnTrack">Expected number of cars on track</param>
    [TestMethod]
    [DataRow(SessionType.Race, DriverStatus.InLap, (ushort)1, true, 1)]
    [DataRow(SessionType.Race2, DriverStatus.OutLap, (ushort)1, true, 1)]
    [DataRow(SessionType.Race3, DriverStatus.OnTrack, (ushort)1, true, 1)]
    [DataRow(SessionType.Race, DriverStatus.InGarage, (ushort)1, false, 0)]
    [DataRow(SessionType.Race, DriverStatus.FlyingLap, (ushort)1, false, 1)]
    [DataRow(SessionType.Race, DriverStatus.FlyingLap, (ushort)60, false, 1)]
    [DataRow(SessionType.Practice1, DriverStatus.FlyingLap, (ushort)1, true, 1)]
    [DataRow(SessionType.Practice2, DriverStatus.OnTrack, (ushort)1, false, 1)]
    [DataRow(SessionType.Practice3, DriverStatus.InLap, (ushort)1, false, 1)]
    [DataRow(SessionType.ShortPractice, DriverStatus.FlyingLap, (ushort)1, true, 1)]
    [DataRow(SessionType.Qualifying1, DriverStatus.FlyingLap, (ushort)1, true, 1)]
    [DataRow(SessionType.Qualifying2, DriverStatus.OutLap, (ushort)1, false, 1)]
    [DataRow(SessionType.Qualifying3, DriverStatus.FlyingLap, (ushort)1, true, 1)]
    [DataRow(SessionType.ShortQualifying, DriverStatus.FlyingLap, (ushort)1, true, 1)]
    [DataRow(SessionType.OneShotQualifying, DriverStatus.FlyingLap, (ushort)1, true, 1)]
    [DataRow(SessionType.SprintShootout1, DriverStatus.FlyingLap, (ushort)1, true, 1)]
    [DataRow(SessionType.SprintShootout2, DriverStatus.InLap, (ushort)1, false, 1)]
    [DataRow(SessionType.SprintShootout3, DriverStatus.FlyingLap, (ushort)1, true, 1)]
    [DataRow(SessionType.ShortSprintShootout, DriverStatus.FlyingLap, (ushort)1, true, 1)]
    [DataRow(SessionType.OneShotSprintShootout, DriverStatus.FlyingLap, (ushort)1, true, 1)]
    [DataRow(SessionType.TimeTrial, DriverStatus.FlyingLap, (ushort)1, true, 1)]
    [DataRow(SessionType.TimeTrial, DriverStatus.OnTrack, (ushort)1, true, 1)]
    [DataRow(SessionType.TimeTrial, DriverStatus.InLap, (ushort)1, false, 1)]
    [DataRow(SessionType.Unknown, DriverStatus.FlyingLap, (ushort)1, false, 1)]
    public void LapDataProcessorDriverStatusDecidesRecordableLap(SessionType sessionType, DriverStatus driverStatus, ushort lapNumber, bool expectedIsRecordable, int expectedCarsOnTrack)
    {
        var sessionRuntimeData = CreateSessionRuntimeData(2025, sessionType);

        sessionRuntimeData.TotalLaps = 50;

        var participantRuntimeData = AddParticipant(sessionRuntimeData, 450140);
        var lapData = CreateLapData2025(lapNumber, driverStatus, 100, 0);

        var isProcessed = CreateProcessor(lapData).Process(lapData, sessionRuntimeData);

        Assert.IsTrue(isProcessed, "Lap data packet not correctly processed!");
        Assert.AreEqual(expectedIsRecordable, participantRuntimeData.IsOnRecordableLap, $"The recordable state for {driverStatus} in {sessionType} is not as expected!");
        Assert.AreEqual(expectedCarsOnTrack, sessionRuntimeData.CurrentSession.CurrentCarsOnTrack, $"The cars on track for {driverStatus} in {sessionType} are not as expected!");
        Assert.AreEqual(driverStatus, participantRuntimeData.CurrentStatus, "The driver status of the packet should be stored at the participant!");
    }

    /// <summary>
    /// A new lap number completes the previous lap with its lap time and starts the next lap
    /// </summary>
    [TestMethod]
    public void LapDataProcessorNewLapNumberCompletesPreviousLap()
    {
        var sessionRuntimeData = CreateSessionRuntimeData(2025, SessionType.Race);
        var participantRuntimeData = AddParticipant(sessionRuntimeData, 450150);

        participantRuntimeData.IsHumanDriver = true;
        participantRuntimeData.IsOnRecordableLap = true;
        participantRuntimeData.CurrentLapNumber = 1;
        participantRuntimeData.CurrentPacketLapNumber = 1;
        participantRuntimeData.LastLapDistance = 5000;

        Assert.IsTrue(participantRuntimeData.AddLap(new LapEntity
                                                    {
                                                        LapNumber = 1,
                                                        SessionId = SessionDbId,
                                                        ParticipantId = 450150,
                                                        Sector1Time = Sector1TimeMs,
                                                        Sector2Time = Sector2TimeMs
                                                    }),
                      "The previous lap could not be added!");

        var lapData = CreateLapData2025(2, DriverStatus.OnTrack, 10, LastLapTimeMs);

        var isProcessed = CreateProcessor(lapData).Process(lapData, sessionRuntimeData);

        Assert.IsTrue(isProcessed, "Lap data packet not correctly processed!");
        Assert.AreEqual(LastLapTimeMs, participantRuntimeData.LastLapTime, "The last lap time should be taken from the packet!");
        Assert.AreEqual(1, participantRuntimeData.LastLapTimeNumber, "The last lap time should belong to the previous lap!");
        Assert.IsNull(participantRuntimeData.GetLap(1), "The completed previous lap should no longer be an unfinished lap!");
        Assert.IsNotNull(participantRuntimeData.GetLap(2), "The next lap should be started!");
        Assert.AreEqual((ushort)2, participantRuntimeData.CurrentLapNumber, "The current lap number should be taken from the packet!");
        Assert.IsTrue(participantRuntimeData.IsNewTelemetry, "A new lap should request a new telemetry buffer!");
    }

    /// <summary>
    /// A previous lap without sector times keeps its lap time but is not completed
    /// </summary>
    [TestMethod]
    public void LapDataProcessorPreviousLapWithoutSectorsIsNotCompleted()
    {
        var sessionRuntimeData = CreateSessionRuntimeData(2025, SessionType.Race);
        var participantRuntimeData = AddParticipant(sessionRuntimeData, 450160);

        participantRuntimeData.IsOnRecordableLap = true;
        participantRuntimeData.CurrentLapNumber = 1;
        participantRuntimeData.CurrentPacketLapNumber = 1;

        var previousLap = new LapEntity
                          {
                              LapNumber = 1,
                              SessionId = SessionDbId,
                              ParticipantId = 450160
                          };

        Assert.IsTrue(participantRuntimeData.AddLap(previousLap), "The previous lap could not be added!");

        var lapData = CreateLapData2025(2, DriverStatus.OnTrack, 10, LastLapTimeMs);

        var isProcessed = CreateProcessor(lapData).Process(lapData, sessionRuntimeData);

        Assert.IsTrue(isProcessed, "Lap data packet not correctly processed!");
        Assert.AreSame(previousLap, participantRuntimeData.GetLap(1), "The previous lap without sector times should stay an unfinished lap!");
        Assert.AreEqual(LastLapTimeMs, previousLap.LapTime, "The lap time should be stored at the previous lap!");
        Assert.IsFalse(previousLap.IsCompleted, "A lap without sector times must not be completed!");
    }

    /// <summary>
    /// A lap distance reset with an unchanged lap number moves the participant to the next lap
    /// </summary>
    [TestMethod]
    public void LapDataProcessorLapDistanceResetMovesToNextLap()
    {
        var sessionRuntimeData = CreateSessionRuntimeData(2025, SessionType.Race);
        var participantRuntimeData = AddParticipant(sessionRuntimeData, 450170);

        participantRuntimeData.IsOnRecordableLap = true;
        participantRuntimeData.CurrentLapNumber = 1;
        participantRuntimeData.CurrentPacketLapNumber = 1;
        participantRuntimeData.LastLapDistance = 5000;

        Assert.IsTrue(participantRuntimeData.AddLap(new LapEntity
                                                    {
                                                        LapNumber = 1,
                                                        SessionId = SessionDbId,
                                                        ParticipantId = 450170,
                                                        Sector1Time = Sector1TimeMs,
                                                        Sector2Time = Sector2TimeMs
                                                    }),
                      "The previous lap could not be added!");

        var lapData = CreateLapData2025(1, DriverStatus.OnTrack, 10, LastLapTimeMs);

        var isProcessed = CreateProcessor(lapData).Process(lapData, sessionRuntimeData);

        Assert.IsTrue(isProcessed, "Lap data packet not correctly processed!");
        Assert.IsTrue(participantRuntimeData.IsOnNextLap, "The participant should be marked as being on the next lap!");
        Assert.AreEqual((ushort)2, participantRuntimeData.CurrentLapNumber, "The lap number should be increased although the packet still reports the old lap!");
        Assert.IsNotNull(participantRuntimeData.GetLap(2), "The next lap should be started!");
    }

    /// <summary>
    /// The next lap marker is reset as soon as the packet reports the expected lap number
    /// </summary>
    [TestMethod]
    public void LapDataProcessorMatchingLapNumberResetsNextLapMarker()
    {
        var sessionRuntimeData = CreateSessionRuntimeData(2025, SessionType.Practice1);
        var participantRuntimeData = AddParticipant(sessionRuntimeData, 450180);

        participantRuntimeData.IsOnNextLap = true;
        participantRuntimeData.CurrentLapNumber = 2;
        participantRuntimeData.CurrentPacketLapNumber = 1;

        var lapData = CreateLapData2025(2, DriverStatus.InLap, 10, 0);

        var isProcessed = CreateProcessor(lapData).Process(lapData, sessionRuntimeData);

        Assert.IsTrue(isProcessed, "Lap data packet not correctly processed!");
        Assert.IsFalse(participantRuntimeData.IsOnNextLap, "The next lap marker should be reset once the packet reports the expected lap number!");
    }

    /// <summary>
    /// Leaving the track during a recordable lap removes the started lap and its telemetry
    /// </summary>
    [TestMethod]
    public void LapDataProcessorLeavingTrackRemovesStartedLap()
    {
        var sessionRuntimeData = CreateSessionRuntimeData(2025, SessionType.Practice1);
        var participantRuntimeData = AddParticipant(sessionRuntimeData, 450190);

        participantRuntimeData.IsOnRecordableLap = true;
        participantRuntimeData.CarIsOnTrack = true;
        participantRuntimeData.CurrentLapNumber = 3;
        participantRuntimeData.CurrentPacketLapNumber = 3;
        participantRuntimeData.LastLapSector = Sector.Sector1;

        Assert.IsTrue(participantRuntimeData.AddLap(new LapEntity
                                                    {
                                                        LapNumber = 3,
                                                        SessionId = SessionDbId,
                                                        ParticipantId = 450190
                                                    }),
                      "The started lap could not be added!");

        participantRuntimeData.AddTelemetryData(3, new CarTelemetryEntity());

        var lapData = CreateLapData2025(3, DriverStatus.InGarage, 100, 0);

        var isProcessed = CreateProcessor(lapData).Process(lapData, sessionRuntimeData);

        Assert.IsTrue(isProcessed, "Lap data packet not correctly processed!");
        Assert.IsFalse(participantRuntimeData.IsOnRecordableLap, "A car in the garage must not be on a recordable lap!");
        Assert.IsNull(participantRuntimeData.GetLap(3), "The started lap should be removed after leaving the track!");
        Assert.IsFalse(participantRuntimeData.ClearTelemetryData(3), "The buffered telemetry of the started lap should already be cleared!");
        Assert.AreEqual(Sector.Unknown, participantRuntimeData.LastLapSector, "The sector of a not recordable lap should be unknown!");
        Assert.AreEqual(0, sessionRuntimeData.CurrentSession.CurrentCarsOnTrack, "A car in the garage must not be counted as on track!");
    }

    /// <summary>
    /// Without any lap times the time table is built from the grid positions
    /// </summary>
    [TestMethod]
    public void LapDataProcessorGridPositionsBuildTimeTable()
    {
        var sessionRuntimeData = CreateSessionRuntimeData(2025, SessionType.Race);

        sessionRuntimeData.CurrentSession.Drivers.Add(new LiveDriverData
                                                      {
                                                          ArrayIndex = 0,
                                                          GridPosition = 2,
                                                          CarPosition = 1
                                                      });
        sessionRuntimeData.CurrentSession.Drivers.Add(new LiveDriverData
                                                      {
                                                          ArrayIndex = 1,
                                                          GridPosition = 1,
                                                          CarPosition = 2
                                                      });
        sessionRuntimeData.CurrentSession.Drivers.Add(new LiveDriverData
                                                      {
                                                          ArrayIndex = 2
                                                      });

        var lapData = CreateLapData2025(1, DriverStatus.OnTrack, 100, 0);

        var isProcessed = CreateProcessor(lapData).Process(lapData, sessionRuntimeData);

        Assert.IsTrue(isProcessed, "Lap data packet not correctly processed!");
        Assert.AreSequenceEqual(new List<int>
                                {
                                    1,
                                    0
                                },
                                sessionRuntimeData.CurrentSession.TimeTable,
                                "The time table should be ordered by grid position and skip drivers without one!");
    }

    /// <summary>
    /// Without lap times and grid positions the time table is built from the car positions
    /// </summary>
    [TestMethod]
    public void LapDataProcessorCarPositionsBuildTimeTable()
    {
        var sessionRuntimeData = CreateSessionRuntimeData(2025, SessionType.Practice1);

        sessionRuntimeData.CurrentSession.Drivers.Add(new LiveDriverData
                                                      {
                                                          ArrayIndex = 0,
                                                          CarPosition = 2
                                                      });
        sessionRuntimeData.CurrentSession.Drivers.Add(new LiveDriverData
                                                      {
                                                          ArrayIndex = 1,
                                                          CarPosition = 1
                                                      });
        sessionRuntimeData.CurrentSession.Drivers.Add(new LiveDriverData
                                                      {
                                                          ArrayIndex = 2
                                                      });

        var lapData = CreateLapData2025(1, DriverStatus.FlyingLap, 100, 0);

        var isProcessed = CreateProcessor(lapData).Process(lapData, sessionRuntimeData);

        Assert.IsTrue(isProcessed, "Lap data packet not correctly processed!");
        Assert.AreSequenceEqual(new List<int>
                                {
                                    1,
                                    0
                                },
                                sessionRuntimeData.CurrentSession.TimeTable,
                                "The time table should be ordered by car position and skip drivers without one!");
    }

    /// <summary>
    /// Without lap times, grid positions and car positions the time table stays empty
    /// </summary>
    [TestMethod]
    public void LapDataProcessorNoPositionsKeepTimeTableEmpty()
    {
        var sessionRuntimeData = CreateSessionRuntimeData(2025, SessionType.Practice1);

        sessionRuntimeData.CurrentSession.Drivers.Add(new LiveDriverData
                                                      {
                                                          ArrayIndex = 0
                                                      });

        var lapData = CreateLapData2025(1, DriverStatus.FlyingLap, 100, 0);

        var isProcessed = CreateProcessor(lapData).Process(lapData, sessionRuntimeData);

        Assert.IsTrue(isProcessed, "Lap data packet not correctly processed!");
        Assert.IsEmpty(sessionRuntimeData.CurrentSession.TimeTable, "The time table should stay empty without any positions!");
    }

    /// <summary>
    /// Only faster sector times replace the stored fastest sector times of the driver and the session
    /// </summary>
    /// <param name="driverFastestSector">Fastest sector time of the driver before processing</param>
    /// <param name="sessionFastestSector">Fastest sector time of the session before processing</param>
    /// <param name="expectedDriverFastestSector1">Expected fastest sector 1 time of the driver</param>
    /// <param name="expectedSessionFastestSector1">Expected fastest sector 1 time of the session</param>
    /// <param name="expectedSessionFastestDriver">Expected array index of the fastest driver of the session</param>
    [TestMethod]
    [DataRow(20000u, 20000u, 20000u, 20000u, 5)]
    [DataRow(40000u, 20000u, Sector1TimeMs, 20000u, 5)]
    [DataRow(40000u, 40000u, Sector1TimeMs, Sector1TimeMs, 0)]
    public void LapDataProcessorFasterSectorReplacesFastestSector(uint driverFastestSector, uint sessionFastestSector, uint expectedDriverFastestSector1, uint expectedSessionFastestSector1, int expectedSessionFastestDriver)
    {
        var sessionRuntimeData = CreateSessionRuntimeData(2025, SessionType.Practice1);

        sessionRuntimeData.CurrentSession.FastestSector1 = sessionFastestSector;
        sessionRuntimeData.CurrentSession.FastestSector1Driver = 5;
        sessionRuntimeData.CurrentSession.FastestSector2 = sessionFastestSector;
        sessionRuntimeData.CurrentSession.FastestSector2Driver = 5;

        var participantRuntimeData = AddParticipant(sessionRuntimeData, 450200);

        Assert.IsNotNull(participantRuntimeData.LiveData, "The live data of the participant should be available!");

        participantRuntimeData.LiveData.FastestSector1 = driverFastestSector;
        participantRuntimeData.LiveData.FastestSector2 = driverFastestSector;

        var lapData = CreateLapData2025(1, DriverStatus.FlyingLap, 100, 0);

        var isProcessed = CreateProcessor(lapData).Process(lapData, sessionRuntimeData);

        Assert.IsTrue(isProcessed, "Lap data packet not correctly processed!");
        Assert.AreEqual(expectedDriverFastestSector1, participantRuntimeData.LiveData.FastestSector1, "The fastest sector 1 of the driver is not as expected!");
        Assert.AreEqual(expectedSessionFastestSector1, sessionRuntimeData.CurrentSession.FastestSector1, "The fastest sector 1 of the session is not as expected!");
        Assert.AreEqual(expectedSessionFastestDriver, sessionRuntimeData.CurrentSession.FastestSector1Driver, "The fastest sector 1 driver of the session is not as expected!");
        Assert.AreEqual(expectedSessionFastestDriver, sessionRuntimeData.CurrentSession.FastestSector2Driver, "The fastest sector 2 driver of the session is not as expected!");
    }

    /// <summary>
    /// The lap and session data of a human driver are published to the telemetry writer
    /// </summary>
    [TestMethod]
    public void LapDataProcessorHumanDriverPublishesTelemetry()
    {
        var sessionRuntimeData = CreateSessionRuntimeData(2025, SessionType.Practice1);
        var participantRuntimeData = AddParticipant(sessionRuntimeData, 450210);

        participantRuntimeData.IsHumanDriver = true;

        var telemetryWriter = new RecordingTelemetryWriter();
        var lapData = CreateLapData2025(1, DriverStatus.FlyingLap, 100, 0);

        var isProcessed = CreateProcessor(lapData, telemetryWriter).Process(lapData, sessionRuntimeData);

        Assert.IsTrue(isProcessed, "Lap data packet not correctly processed!");
        Assert.AreEqual(1, telemetryWriter.LapDataCount, "The lap data of the human driver should be published once!");
        Assert.AreEqual(1, telemetryWriter.SessionDataCount, "The session data of the human driver should be published once!");
    }

    /// <summary>
    /// The lap and session data of an AI driver are not published to the telemetry writer
    /// </summary>
    [TestMethod]
    public void LapDataProcessorAiDriverDoesNotPublishTelemetry()
    {
        var sessionRuntimeData = CreateSessionRuntimeData(2025, SessionType.Practice1);

        AddParticipant(sessionRuntimeData, 450220);

        var telemetryWriter = new RecordingTelemetryWriter();
        var lapData = CreateLapData2025(1, DriverStatus.FlyingLap, 100, 0);

        var isProcessed = CreateProcessor(lapData, telemetryWriter).Process(lapData, sessionRuntimeData);

        Assert.IsTrue(isProcessed, "Lap data packet not correctly processed!");
        Assert.AreEqual(0, telemetryWriter.LapDataCount, "The lap data of an AI driver must not be published!");
        Assert.AreEqual(0, telemetryWriter.SessionDataCount, "The session data of an AI driver must not be published!");
    }

    #endregion // Test methods
}