using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Tests;

/// <summary>
/// Tests for the type hierarchy of the packet data classes, verifying that every game year builds on the previous one
/// instead of repeating its members
/// </summary>
[TestClass]
public class PacketDataHierarchyTests
{
    #region Methods

    /// <summary>
    /// The 2025 session layout is unchanged compared to 2024, so the session data has to inherit the 2024 data
    /// </summary>
    [TestMethod]
    public void SessionData2025InheritsSessionData2024()
    {
        Assert.IsInstanceOfType<SessionData2024>(new SessionData2025(), "The F1 2025 session data must inherit the F1 2024 session data!");
    }

    /// <summary>
    /// The 2026 session data has to keep inheriting the 2025 session data
    /// </summary>
    [TestMethod]
    public void SessionData2026InheritsSessionData2025()
    {
        Assert.IsInstanceOfType<SessionData2025>(new SessionData2026(), "The F1 2026 session data must inherit the F1 2025 session data!");
    }

    /// <summary>
    /// The inherited constructor has to keep creating the weekend structure of the 2025 session data
    /// </summary>
    [TestMethod]
    public void SessionData2025WeekendStructureIsCreated()
    {
        var sessionData = new SessionData2025();

        Assert.HasCount(12, sessionData.WeekendStructure, "The weekend structure of the F1 2025 session data must offer twelve sessions!");
    }

    /// <summary>
    /// The inherited constructor has to keep creating the 2025 number of weather forecast samples
    /// </summary>
    [TestMethod]
    public void SessionData2025WeatherForecastSamplesAreCreated()
    {
        var sessionData = new SessionData2025();

        Assert.HasCount(64, sessionData.WeatherForecastSamples, "The F1 2025 session data must offer 64 weather forecast samples!");
    }

    /// <summary>
    /// The 2025 time trial data set interface describes a single data set, so it has to derive from the data set base
    /// interface and not from the container interface
    /// </summary>
    [TestMethod]
    public void TimeTrialDataSet2025InterfaceDerivesFromDataSetBase()
    {
        var dataSetInterfaces = typeof(ITimeTrialDataSet2025).GetInterfaces();

        Assert.Contains(typeof(ITimeTrialDataSetBase), dataSetInterfaces, "The F1 2025 time trial data set interface must derive from the data set base interface!");
        Assert.DoesNotContain(typeof(ITimeTrialDataBase), dataSetInterfaces, "The F1 2025 time trial data set interface must not derive from the time trial container interface!");
    }

    /// <summary>
    /// The 2025 time trial data set has to implement the interface of its game year
    /// </summary>
    [TestMethod]
    public void TimeTrialDataSet2025ImplementsGameYearInterface()
    {
        Assert.IsInstanceOfType<ITimeTrialDataSet2025>(new TimeTrialDataSet2025(), "The F1 2025 time trial data set must implement the F1 2025 data set interface!");
    }

    /// <summary>
    /// The 2026 time trial data set has to implement the interface of its game year
    /// </summary>
    [TestMethod]
    public void TimeTrialDataSet2026ImplementsGameYearInterface()
    {
        Assert.IsInstanceOfType<ITimeTrialDataSet2026>(new TimeTrialDataSet2026(), "The F1 2026 time trial data set must implement the F1 2026 data set interface!");
    }

    #endregion // Methods
}