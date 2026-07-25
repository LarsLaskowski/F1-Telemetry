using F1Server.WebApi.Core;

namespace F1Server.Tests.WebApi;

/// <summary>
/// Contains unit tests verifying the single start behaviour and the resource handling of the
/// <see cref="TimerManager"/>
/// </summary>
[TestClass]
public class TimerManagerTests
{
    #region Constants

    /// <summary>
    /// Number of repeated <see cref="TimerManager.PrepareTimer"/> calls issued by the tests in this class
    /// </summary>
    private const int RepeatedPrepareCount = 5;

    /// <summary>
    /// Maximum time in milliseconds a test waits for the first timer tick
    /// </summary>
    private const int FirstTickTimeout = 10000;

    /// <summary>
    /// Time in milliseconds the tick counting test observes the timer after the first tick
    /// </summary>
    private const int TickObservationTime = 2000;

    /// <summary>
    /// Maximum number of ticks expected within the observation window when only a single timer is running
    /// </summary>
    private const int MaxExpectedTickCount = 8;

    #endregion // Constants

    #region Static methods

    /// <summary>
    /// Timer action doing nothing, used where the executed action is irrelevant for the test
    /// </summary>
    private static void NoOperation()
    {
    }

    #endregion // Static methods

    #region Methods

    /// <summary>
    /// Verifies that the first call starts the timer and publishes the start state
    /// </summary>
    [TestMethod]
    public void TimerManagerPrepareTimerStartsTimerAndSetsState()
    {
        using (var timerManager = new TimerManager())
        {
            timerManager.PrepareTimer(NoOperation);

            Assert.IsTrue(timerManager.IsTimerStarted, "The timer should be marked as started after the first call.");
            Assert.AreNotEqual(default, timerManager.TimerStarted, "The start timestamp should be set after the first call.");
        }
    }

    /// <summary>
    /// Verifies that repeated calls neither restart the timer nor replace the registered action
    /// </summary>
    [TestMethod]
    public void TimerManagerPrepareTimerCalledTwiceKeepsFirstTimer()
    {
        using (var timerManager = new TimerManager())
        {
            var secondActionCallCount = 0;

            timerManager.PrepareTimer(NoOperation);

            var firstStart = timerManager.TimerStarted;

            timerManager.PrepareTimer(() => Interlocked.Increment(ref secondActionCallCount));

            timerManager.Execute(null);

            Assert.AreEqual(firstStart, timerManager.TimerStarted, "The start timestamp should not be overwritten by a repeated call.");
            Assert.AreEqual(0, Volatile.Read(ref secondActionCallCount), "The action of a repeated call should not be registered.");
        }
    }

    /// <summary>
    /// Verifies that repeated calls do not create additional timers, which would multiply the tick rate
    /// </summary>
    [TestMethod]
    public void TimerManagerPrepareTimerCalledRepeatedlyCreatesOnlyOneTimer()
    {
        using (var firstTick = new ManualResetEventSlim(false))
        {
            using (var timerManager = new TimerManager())
            {
                var tickCount = 0;

                for (var index = 0; index < RepeatedPrepareCount; index++)
                {
                    timerManager.PrepareTimer(() =>
                                              {
                                                  Interlocked.Increment(ref tickCount);

                                                  firstTick.Set();
                                              });
                }

                Assert.IsTrue(firstTick.Wait(FirstTickTimeout), "The timer should have ticked at least once.");

                Thread.Sleep(TickObservationTime);

                Assert.IsLessThanOrEqualTo(MaxExpectedTickCount, Volatile.Read(ref tickCount), "Repeated calls should not start additional timers.");
            }
        }
    }

    /// <summary>
    /// Verifies that concurrent calls start the timer exactly once
    /// </summary>
    [TestMethod]
    public void TimerManagerPrepareTimerCalledConcurrentlyStartsTimerOnce()
    {
        using (var timerManager = new TimerManager())
        {
            var startedActionCount = 0;

            Parallel.For(0,
                         RepeatedPrepareCount,
                         _ => timerManager.PrepareTimer(NoOperation));

            timerManager.PrepareTimer(() => Interlocked.Increment(ref startedActionCount));

            timerManager.Execute(null);

            Assert.IsTrue(timerManager.IsTimerStarted, "The timer should be marked as started after concurrent calls.");
            Assert.AreEqual(0, Volatile.Read(ref startedActionCount), "A call after the timer has been started should not register another action.");
        }
    }

    /// <summary>
    /// Verifies that disposing releases the timer resources and resets the start state
    /// </summary>
    [TestMethod]
    public void TimerManagerDisposeResetsTimerState()
    {
        var timerManager = new TimerManager();

        timerManager.PrepareTimer(NoOperation);
        timerManager.Dispose();

        Assert.IsFalse(timerManager.IsTimerStarted, "The timer should not be marked as started after disposing.");
    }

    /// <summary>
    /// Verifies that disposing more than once does not throw
    /// </summary>
    [TestMethod]
    public void TimerManagerDisposeCalledTwiceDoesNotThrow()
    {
        var timerManager = new TimerManager();

        timerManager.PrepareTimer(NoOperation);
        timerManager.Dispose();
        timerManager.Dispose();

        Assert.IsFalse(timerManager.IsTimerStarted, "The timer should stay stopped after disposing twice.");
    }

    /// <summary>
    /// Verifies that the timer cannot be started again after the manager has been disposed
    /// </summary>
    [TestMethod]
    public void TimerManagerPrepareTimerAfterDisposeThrowsObjectDisposedException()
    {
        var timerManager = new TimerManager();

        timerManager.Dispose();

        Assert.ThrowsExactly<ObjectDisposedException>(() => timerManager.PrepareTimer(NoOperation), "A disposed timer manager should not start a timer.");
    }

    #endregion // Methods
}