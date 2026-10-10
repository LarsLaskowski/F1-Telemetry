using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Data;
using F1Server.Db.Entity.Tables;
using F1Server.Service.Processors;
using F1Server.Service.Runtime;
using F1Server.Tests.Data;

using Microsoft.Extensions.DependencyInjection;

namespace F1Server.Tests.Processors;

/// <summary>
/// Class to test the car status processor class
/// </summary>
[TestClass]
public class CarStatusProcessorTests
{
    #region Constants

    /// <summary>
    /// Database id of the test session
    /// </summary>
    private const long SessionDbId = 450300;

    #endregion // Constants

    #region Static methods

    /// <summary>
    /// Creates a recordable session runtime data object
    /// </summary>
    /// <returns>Session runtime data</returns>
    private static SessionRuntimeData CreateSessionRuntimeData()
    {
        return new SessionRuntimeData(2025, (ulong)SessionDbId, SessionType.Race)
               {
                   HasParticipants = true,
                   IsRecordable = true,
                   SessionDbId = SessionDbId,
                   CurrentSession = new LiveSessionData
                                    {
                                        DbId = SessionDbId,
                                        SessionGameId = (ulong)SessionDbId,
                                        SessionType = SessionType.Race
                                    }
               };
    }

    /// <summary>
    /// Creates a participant on lap 2 with live data and registers it at car index 0
    /// </summary>
    /// <param name="sessionRuntimeData">Session runtime data</param>
    /// <param name="currentLap">Started current lap of the participant</param>
    /// <returns>Participant runtime data</returns>
    private static ParticipantRuntimeData AddParticipant(SessionRuntimeData sessionRuntimeData, out LapEntity currentLap)
    {
        var participantRuntimeData = new ParticipantRuntimeData(sessionRuntimeData)
                                     {
                                         CurrentLapNumber = 2,
                                         LiveData = new LiveDriverData
                                                    {
                                                        ArrayIndex = 0
                                                    }
                                     };

        currentLap = new LapEntity
                     {
                         LapNumber = 2
                     };

        Assert.IsTrue(participantRuntimeData.AddLap(currentLap), "The current lap could not be added!");
        Assert.IsTrue(sessionRuntimeData.Participants.TryAdd(0, participantRuntimeData), "Participant runtime data could not be registered!");

        return participantRuntimeData;
    }

    /// <summary>
    /// Creates a F1 2025 car status packet with the given visual tyre compound on car index 0
    /// </summary>
    /// <param name="visualTyreCompound">Visual tyre compound</param>
    /// <returns>Car status packet</returns>
    private static CarStatus CreateCarStatus(VisualTyreCompound visualTyreCompound)
    {
        var carStatus = new CarStatus2025();

        carStatus.CarStatusData[0] = new CarStatusData2025
                                     {
                                         VisualTyreCompound = visualTyreCompound
                                     };

        var packetHeader = new PacketHeader
                           {
                               GameVersion = 2025,
                               PacketType = PacketTypes.CarStatus,
                               UniqueSessionId = (ulong)SessionDbId
                           };

        return new CarStatus(packetHeader, carStatus);
    }

    /// <summary>
    /// Creates a car status processor
    /// </summary>
    /// <param name="carStatus">Car status packet</param>
    /// <param name="serviceProvider">Service provider</param>
    /// <returns>Car status processor</returns>
    private static CarStatusProcessor CreateProcessor(CarStatus carStatus, IServiceProvider serviceProvider)
    {
        return new CarStatusProcessor(serviceProvider,
                                      carStatus.PacketHeader,
                                      new LiveGameData
                                      {
                                          GameVersion = 2025
                                      });
    }

    /// <summary>
    /// Creates a service provider using the given telemetry writer
    /// </summary>
    /// <param name="telemetryWriter">Telemetry writer</param>
    /// <returns>Service provider</returns>
    private static ServiceProvider CreateServiceProvider(RecordingTelemetryWriter telemetryWriter)
    {
        var services = new ServiceCollection();

        services.AddSingleton(new F1ServerApplicationData
                              {
                                  TelemetryWriter = telemetryWriter
                              });

        return services.BuildServiceProvider();
    }

    #endregion // Static methods

    #region Test methods

    /// <summary>
    /// The visual tyre compound of the packet is stored at the current lap and the live data of the participant
    /// </summary>
    [TestMethod]
    public void CarStatusProcessorVisualTyreCompoundUpdatesCurrentLapAndLiveData()
    {
        var sessionRuntimeData = CreateSessionRuntimeData();
        var participantRuntimeData = AddParticipant(sessionRuntimeData, out var currentLap);
        var carStatus = CreateCarStatus(VisualTyreCompound.Soft);

        var isProcessed = CreateProcessor(carStatus, TestData.ServiceProvider).Process(carStatus, sessionRuntimeData);

        Assert.IsTrue(isProcessed, "Car status packet not correctly processed!");
        Assert.AreEqual(VisualTyreCompound.Soft, currentLap.TyreCompound, "The tyre compound should be stored at the current lap!");
        Assert.IsNotNull(participantRuntimeData.LiveData, "The live data of the participant should still be available!");
        Assert.AreEqual(VisualTyreCompound.Soft, participantRuntimeData.LiveData.CurrentUsedTyre, "The tyre compound should be stored at the live data!");
    }

    /// <summary>
    /// The car status of a human driver is published to the telemetry writer
    /// </summary>
    [TestMethod]
    public void CarStatusProcessorHumanDriverPublishesCarStatus()
    {
        var sessionRuntimeData = CreateSessionRuntimeData();
        var participantRuntimeData = AddParticipant(sessionRuntimeData, out _);

        participantRuntimeData.IsHumanDriver = true;

        var telemetryWriter = new RecordingTelemetryWriter();
        var carStatus = CreateCarStatus(VisualTyreCompound.Medium);

        using (var serviceProvider = CreateServiceProvider(telemetryWriter))
        {
            var isProcessed = CreateProcessor(carStatus, serviceProvider).Process(carStatus, sessionRuntimeData);

            Assert.IsTrue(isProcessed, "Car status packet not correctly processed!");
        }

        Assert.AreEqual(1, telemetryWriter.CarStatusCount, "The car status of the human driver should be published once!");
    }

    /// <summary>
    /// The car status of an AI driver is not published to the telemetry writer
    /// </summary>
    [TestMethod]
    public void CarStatusProcessorAiDriverDoesNotPublishCarStatus()
    {
        var sessionRuntimeData = CreateSessionRuntimeData();

        AddParticipant(sessionRuntimeData, out _);

        var telemetryWriter = new RecordingTelemetryWriter();
        var carStatus = CreateCarStatus(VisualTyreCompound.Medium);

        using (var serviceProvider = CreateServiceProvider(telemetryWriter))
        {
            var isProcessed = CreateProcessor(carStatus, serviceProvider).Process(carStatus, sessionRuntimeData);

            Assert.IsTrue(isProcessed, "Car status packet not correctly processed!");
        }

        Assert.AreEqual(0, telemetryWriter.CarStatusCount, "The car status of an AI driver must not be published!");
    }

    /// <summary>
    /// A car status packet without registered participants does not change any participant data
    /// </summary>
    [TestMethod]
    public void CarStatusProcessorWithoutParticipantsDoesNotUpdateTyres()
    {
        var sessionRuntimeData = CreateSessionRuntimeData();
        var participantRuntimeData = AddParticipant(sessionRuntimeData, out var currentLap);

        sessionRuntimeData.HasParticipants = false;

        var carStatus = CreateCarStatus(VisualTyreCompound.Hard);

        var isProcessed = CreateProcessor(carStatus, TestData.ServiceProvider).Process(carStatus, sessionRuntimeData);

        Assert.IsTrue(isProcessed, "Car status packet without participants should be processed without an error!");
        Assert.AreEqual(VisualTyreCompound.Unknown, currentLap.TyreCompound, "The tyre compound of the lap must not be changed without participants!");
        Assert.IsNotNull(participantRuntimeData.LiveData, "The live data of the participant should still be available!");
        Assert.AreEqual(VisualTyreCompound.Unknown, participantRuntimeData.LiveData.CurrentUsedTyre, "The tyre compound of the live data must not be changed without participants!");
    }

    /// <summary>
    /// A car status packet of a not recordable session is ignored
    /// </summary>
    [TestMethod]
    public void CarStatusProcessorNotRecordableSessionDoesNotUpdateTyres()
    {
        var sessionRuntimeData = CreateSessionRuntimeData();

        AddParticipant(sessionRuntimeData, out var currentLap);

        sessionRuntimeData.IsRecordable = false;

        var carStatus = CreateCarStatus(VisualTyreCompound.Hard);

        var isProcessed = CreateProcessor(carStatus, TestData.ServiceProvider).Process(carStatus, sessionRuntimeData);

        Assert.IsTrue(isProcessed, "Car status packet of a not recordable session should be ignored without an error!");
        Assert.AreEqual(VisualTyreCompound.Unknown, currentLap.TyreCompound, "The tyre compound must not be changed for a not recordable session!");
    }

    /// <summary>
    /// A data object that is no car status packet is ignored without reporting an error
    /// </summary>
    [TestMethod]
    public void CarStatusProcessorUnexpectedDataObjectReturnsTrue()
    {
        var carStatus = CreateCarStatus(VisualTyreCompound.Hard);
        var carStatusProcessor = CreateProcessor(carStatus, TestData.ServiceProvider);

        var isProcessed = carStatusProcessor.Process(new object(), CreateSessionRuntimeData());

        Assert.IsTrue(isProcessed, "An unexpected data object should be ignored without an error!");
        Assert.AreEqual(string.Empty, carStatusProcessor.LastException, "Ignoring an unexpected data object must not record an exception!");
    }

    #endregion // Test methods
}