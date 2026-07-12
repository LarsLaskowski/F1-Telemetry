using System.Diagnostics;

using F1Server.Core.Data;
using F1Server.Core.Observability;
using F1Server.Core.Packets.Data;
using F1Server.Data;
using F1Server.Tests.Data;

namespace F1Server.Tests.Processors;

/// <summary>
/// Class to test the shared <see cref="F1Server.Service.Processors.BaseProcessor"/> tracing behavior
/// </summary>
[TestClass]
public class BaseProcessorTests
{
    #region Test methods

    /// <summary>
    /// A fast, successfully processed packet must not record a tracing span
    /// </summary>
    [TestMethod]
    public void BaseProcessorRecordSlowProcessingActivityFastSuccessDoesNotRecordActivity()
    {
        var recordedActivity = InvokeWithListener(processor => processor.InvokeRecordSlowProcessingActivity("TestProcessor", TimeSpan.FromMilliseconds(1), true));

        Assert.IsNull(recordedActivity, "A fast, successfully processed packet recorded a tracing span!");
    }

    /// <summary>
    /// A slow but successfully processed packet must record a tracing span with an Ok status
    /// </summary>
    [TestMethod]
    public void BaseProcessorRecordSlowProcessingActivitySlowSuccessRecordsOkActivity()
    {
        var recordedActivity = InvokeWithListener(processor => processor.InvokeRecordSlowProcessingActivity("TestProcessor", TimeSpan.FromMilliseconds(ConstData.SlowPacketProcessingThresholdMs + 1), true));

        Assert.IsNotNull(recordedActivity, "A slow, successfully processed packet did not record a tracing span!");
        Assert.AreEqual(ActivityStatusCode.Ok, recordedActivity.Status, "The recorded span status was not Ok!");
    }

    /// <summary>
    /// A fast but failed packet must still record a tracing span with an Error status
    /// </summary>
    [TestMethod]
    public void BaseProcessorRecordSlowProcessingActivityFastFailureRecordsErrorActivity()
    {
        var recordedActivity = InvokeWithListener(processor => processor.InvokeRecordSlowProcessingActivity("TestProcessor", TimeSpan.FromMilliseconds(1), false));

        Assert.IsNotNull(recordedActivity, "A failed packet did not record a tracing span!");
        Assert.AreEqual(ActivityStatusCode.Error, recordedActivity.Status, "The recorded span status was not Error!");
    }

    #endregion // Test methods

    #region Private methods

    /// <summary>
    /// Attaches an <see cref="ActivityListener"/> to <see cref="AppActivity.SrvSource"/>, runs <paramref name="invoke"/>
    /// against a fresh <see cref="TestableProcessor"/>, and returns the span it recorded, if any
    /// </summary>
    /// <param name="invoke">Callback that triggers the code under test</param>
    /// <returns>The recorded span, or null if none was recorded</returns>
    private static Activity? InvokeWithListener(Action<TestableProcessor> invoke)
    {
        Activity? recordedActivity = null;

        using var listener = new ActivityListener
                             {
                                 ShouldListenTo = source => source.Name == AppActivity.SrvSource.Name,
                                 Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllData,
                                 ActivityStopped = activity => recordedActivity = activity
                             };

        ActivitySource.AddActivityListener(listener);

        var processor = new TestableProcessor(TestData.ServiceProvider,
                                              new PacketHeader(),
                                              new LiveGameData
                                              {
                                                  GameVersion = 2025
                                              });

        invoke(processor);

        return recordedActivity;
    }

    #endregion // Private methods
}