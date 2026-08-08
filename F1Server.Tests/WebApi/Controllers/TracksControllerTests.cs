using F1Server.Data.ViewData;
using F1Server.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace F1Server.Tests.WebApi.Controllers;

/// <summary>
/// Contains unit tests verifying that the asynchronous actions of the <see cref="TracksController"/> return the tracks
/// together with their session information
/// </summary>
[TestClass]
public class TracksControllerTests
{
    #region Static methods

    /// <summary>
    /// Creates the entity graph read by the tests in this class
    /// </summary>
    /// <param name="context">Test context</param>
    /// <returns>Task</returns>
    [ClassInitialize]
    public static async Task ClassInit(TestContext context)
    {
        await ControllerTestData.EnsureCreatedAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a controller instance
    /// </summary>
    /// <returns>Controller</returns>
    private static TracksController CreateController()
    {
        return new TracksController(NullLogger<TracksController>.Instance);
    }

    #endregion // Static methods

    #region Methods

    /// <summary>
    /// Verifies that a track with sessions is returned with its session information and reference lap time
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task TracksControllerGetReturnsTrackWithSessionInformation()
    {
        var controller = CreateController();

        var tracks = await controller.Get().ConfigureAwait(false);

        Assert.IsNotNull(tracks, "The tracks should be returned!");

        var testTrack = tracks.ToList()
                              .Find(t => t.TrackId == ControllerTestData.TrackId);

        Assert.IsNotNull(testTrack, "The track of the test session should be returned!");
        Assert.AreEqual(ControllerTestData.TrackName, testTrack.TrackName, "The name of the track should be returned!");
        Assert.AreEqual(ControllerTestData.TrackNumber, testTrack.TrackNumber, "The track number should be returned!");
        Assert.IsTrue(testTrack.HasSession, "A track with sessions should be marked as used!");
        Assert.IsGreaterThan(0, testTrack.Sessions, "The sessions of the track should be counted!");
        Assert.AreEqual(TimeSpan.FromMilliseconds(ControllerTestData.ReferenceLapTime).ToString(@"mm\:ss\.fff"),
                        testTrack.ReferenceLapTime,
                        "The formatted reference lap time of the track should be returned!");
    }

    /// <summary>
    /// Verifies that a track without sessions is not marked as used
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task TracksControllerGetMarksTrackWithoutSessionsAsUnused()
    {
        var trackNumbers = ControllerTestData.AddTracks(1);

        var controller = CreateController();

        var tracks = await controller.Get().ConfigureAwait(false);

        Assert.IsNotNull(tracks, "The tracks should be returned!");

        var unusedTrack = tracks.ToList()
                                .Find(t => t.TrackNumber == trackNumbers[0]);

        Assert.IsNotNull(unusedTrack, "The created track should be returned!");
        Assert.IsFalse(unusedTrack.HasSession, "A track without sessions must not be marked as used!");
        Assert.AreEqual(0, unusedTrack.Sessions, "A track without sessions must not count sessions!");
    }

    /// <summary>
    /// Verifies that a single track is returned by its database id
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task TracksControllerGetTrackReturnsTrackData()
    {
        var controller = CreateController();

        var result = await controller.GetTrack(ControllerTestData.TrackId).ConfigureAwait(false);

        var trackData = (result as OkObjectResult)?.Value as TrackViewData;

        Assert.IsNotNull(trackData, "The requested track should be returned!");
        Assert.AreEqual(ControllerTestData.TrackId, trackData.TrackId, "The returned track should be the requested track!");
        Assert.AreEqual(ControllerTestData.TrackName, trackData.TrackName, "The name of the track should be returned!");
    }

    /// <summary>
    /// Verifies that an unknown track id is answered without track data
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task TracksControllerGetUnknownTrackReturnsNull()
    {
        var controller = CreateController();

        var result = await controller.GetTrack(771999999L).ConfigureAwait(false);

        Assert.IsNull((result as OkObjectResult)?.Value, "An unknown track must not return track data!");
    }

    #endregion // Methods
}