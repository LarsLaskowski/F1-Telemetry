using F1Server.Data.ViewData;
using F1Server.Service.SessionParticipants;
using F1Server.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace F1Server.Tests.WebApi.Controllers;

/// <summary>
/// Contains unit tests verifying that the asynchronous action of the <see cref="ParticipantsController"/> resolves the
/// driver, nationality and team of every participant
/// </summary>
[TestClass]
public class ParticipantsControllerTests
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
    private static ParticipantsController CreateController()
    {
        return new ParticipantsController(NullLogger<ParticipantsController>.Instance, new ParticipantService());
    }

    #endregion // Static methods

    #region Methods

    /// <summary>
    /// Verifies that every participant of a session is returned with its resolved driver, nationality and team
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task ParticipantsControllerGetParticipantsOfSessionReturnsResolvedParticipants()
    {
        var controller = CreateController();

        var result = await controller.GetParticipantsOfSession(ControllerTestData.SessionId).ConfigureAwait(false);

        var participants = (result as OkObjectResult)?.Value as List<ParticipantViewData>;

        Assert.IsNotNull(participants, "The participants of the test session should be returned!");
        Assert.HasCount(ControllerTestData.ParticipantCount, participants, "Every participant of the test session should be returned!");

        var humanParticipant = participants.Find(p => p.ParticipantDbId == ControllerTestData.HumanParticipantId);

        Assert.IsNotNull(humanParticipant, "The human participant of the test session should be returned!");
        Assert.AreEqual(ControllerTestData.HumanDriverName, humanParticipant.DriverName, "The driver name should be resolved from the driver table!");
        Assert.AreEqual(ControllerTestData.NationalityName, humanParticipant.DriverNationality, "The nationality should be resolved from the nationality table!");
        Assert.AreEqual(ControllerTestData.TeamName, humanParticipant.TeamName, "The team name should be resolved from the team table!");
        Assert.IsTrue(humanParticipant.IsHumanControlled, "The human participant should be marked as human controlled!");
    }

    /// <summary>
    /// Verifies that a session without participants is answered without a participant list
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task ParticipantsControllerGetParticipantsOfUnknownSessionReturnsNull()
    {
        var controller = CreateController();

        var result = await controller.GetParticipantsOfSession(771999999L).ConfigureAwait(false);

        Assert.IsNull((result as OkObjectResult)?.Value, "An unknown session must not return participants!");
    }

    #endregion // Methods
}