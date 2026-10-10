using System.Diagnostics;

using F1Server.Core.Enumerations;
using F1Server.Core.Observability;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Data;
using F1Server.Service.Processors;
using F1Server.Service.Runtime;
using F1Server.Tests.Data;

namespace F1Server.Tests.Processors;

/// <summary>
/// Class to test the time trial processor class
/// </summary>
[TestClass]
public class TimeTrialProcessorTests
{
    #region Static methods

    /// <summary>
    /// Creates a time trial session runtime data object
    /// </summary>
    /// <param name="hasParticipants">Has the session participants?</param>
    /// <returns>Session runtime data</returns>
    private static SessionRuntimeData CreateSessionRuntimeData(bool hasParticipants)
    {
        return new SessionRuntimeData(2025, 450400, SessionType.TimeTrial)
               {
                   HasParticipants = hasParticipants
               };
    }

    /// <summary>
    /// Creates a F1 2025 time trial packet with the given team ids
    /// </summary>
    /// <param name="playerSessionTeamId">Team id of the player session best data set</param>
    /// <param name="personalTeamId">Team id of the personal best data set</param>
    /// <param name="rivalTeamId">Team id of the rival data set</param>
    /// <returns>Time trial packet</returns>
    private static TimeTrialData CreateTimeTrialData(ushort playerSessionTeamId, ushort personalTeamId, ushort rivalTeamId)
    {
        var timeTrialData = new TimeTrialData2025();

        timeTrialData.PlayerSessionBestDataSet.TeamId = playerSessionTeamId;
        timeTrialData.PersonalBestDataSet.TeamId = personalTeamId;
        timeTrialData.RivalDataSet.TeamId = rivalTeamId;

        var packetHeader = new PacketHeader
                           {
                               GameVersion = 2025,
                               PacketType = PacketTypes.TimeTrial,
                               UniqueSessionId = 450400
                           };

        return new TimeTrialData(packetHeader, timeTrialData);
    }

    /// <summary>
    /// Creates a time trial processor
    /// </summary>
    /// <returns>Time trial processor</returns>
    private static TimeTrialProcessor CreateProcessor()
    {
        return new TimeTrialProcessor(TestData.ServiceProvider,
                                      new PacketHeader(),
                                      new LiveGameData
                                      {
                                          GameVersion = 2025
                                      });
    }

    /// <summary>
    /// Attaches an <see cref="ActivityListener"/> to <see cref="AppActivity.SrvSource"/>, processes the data object and returns the recorded span
    /// </summary>
    /// <param name="dataObject">Data object to process</param>
    /// <param name="sessionRuntimeData">Session runtime data</param>
    /// <param name="isProcessed">Processing result</param>
    /// <returns>The recorded span, or null if none was recorded</returns>
    private static Activity? ProcessWithListener(object dataObject, SessionRuntimeData sessionRuntimeData, out bool isProcessed)
    {
        Activity? recordedActivity = null;

        using var listener = new ActivityListener
                             {
                                 ShouldListenTo = source => source.Name == AppActivity.SrvSource.Name,
                                 Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllData,
                                 ActivityStopped = activity =>
                                                   {
                                                       if (activity.OperationName == nameof(TimeTrialProcessor))
                                                       {
                                                           recordedActivity = activity;
                                                       }
                                                   }
                             };

        ActivitySource.AddActivityListener(listener);

        isProcessed = CreateProcessor().Process(dataObject, sessionRuntimeData);

        return recordedActivity;
    }

    #endregion // Static methods

    #region Test methods

    /// <summary>
    /// A time trial packet with team ids records the teams of all data sets at the tracing span
    /// </summary>
    [TestMethod]
    public void TimeTrialProcessorTeamIdsAddTeamTags()
    {
        var timeTrialData = CreateTimeTrialData(1, 2, 3);

        var recordedActivity = ProcessWithListener(timeTrialData, CreateSessionRuntimeData(true), out var isProcessed);

        Assert.IsTrue(isProcessed, "Time trial packet not correctly processed!");
        Assert.IsNotNull(recordedActivity, "Processing the time trial packet should record a tracing span!");
        Assert.AreEqual(ActivityStatusCode.Ok, recordedActivity.Status, "The tracing span of a processed time trial packet should be Ok!");

        var tagKeys = recordedActivity.TagObjects.Select(t => t.Key).ToList();

        Assert.Contains("f1.player_session_team", tagKeys, "The team of the player session best should be recorded!");
        Assert.Contains("f1.personal_team", tagKeys, "The team of the personal best should be recorded!");
        Assert.Contains("f1.rival_team", tagKeys, "The team of the rival should be recorded!");
    }

    /// <summary>
    /// A time trial packet of a session without participants is reported as error at the tracing span
    /// </summary>
    [TestMethod]
    public void TimeTrialProcessorWithoutParticipantsRecordsErrorStatus()
    {
        var timeTrialData = CreateTimeTrialData(1, 2, 3);

        var recordedActivity = ProcessWithListener(timeTrialData, CreateSessionRuntimeData(false), out var isProcessed);

        Assert.IsTrue(isProcessed, "A time trial packet without participants should be ignored without failing!");
        Assert.IsNotNull(recordedActivity, "Ignoring the time trial packet should record a tracing span!");
        Assert.AreEqual(ActivityStatusCode.Error, recordedActivity.Status, "The tracing span of an ignored time trial packet should report an error!");
    }

    /// <summary>
    /// A data object that is no time trial packet is ignored without recording an exception
    /// </summary>
    [TestMethod]
    public void TimeTrialProcessorUnexpectedDataObjectReturnsTrue()
    {
        var timeTrialProcessor = CreateProcessor();

        var isProcessed = timeTrialProcessor.Process(new object(), CreateSessionRuntimeData(true));

        Assert.IsTrue(isProcessed, "An unexpected data object should be ignored without an error!");
        Assert.AreEqual(string.Empty, timeTrialProcessor.LastException, "Ignoring an unexpected data object must not record an exception!");
    }

    #endregion // Test methods
}