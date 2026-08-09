using F1Server.Core.Enumerations;
using F1Server.Data;
using F1Server.Service.Championships;

using Microsoft.Extensions.Logging.Abstractions;

namespace F1Server.Tests.Championships;

/// <summary>
/// Contains unit tests verifying that the championship business logic rejects unusable request data before it touches
/// the database
/// </summary>
[TestClass]
public class ChampionshipServiceTests
{
    #region Static methods

    /// <summary>
    /// Creates a service instance
    /// </summary>
    /// <returns>Service</returns>
    private static ChampionshipService CreateService()
    {
        return new ChampionshipService(NullLogger<ChampionshipService>.Instance);
    }

    #endregion // Static methods

    #region Methods

    /// <summary>
    /// Verifies that championship data without enough tracks is rejected
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task ChampionshipServiceCreateChampionshipWithTooFewTracksReturnsInvalidData()
    {
        var service = CreateService();

        var result = await service.CreateChampionshipAsync(new ChampionshipCreateData
                                                           {
                                                               GameVersionId = 1,
                                                               Mode = (int)ChampionshipMode.Career,
                                                               Tracks = [1, 2, 3]
                                                           })
                                  .ConfigureAwait(false);

        Assert.AreEqual(ChampionshipCreateResult.InvalidData, result, "A championship with less than six tracks should be rejected!");
    }

    /// <summary>
    /// Verifies that championship data without a game version is rejected
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task ChampionshipServiceCreateChampionshipWithoutGameVersionReturnsInvalidData()
    {
        var service = CreateService();

        var result = await service.CreateChampionshipAsync(new ChampionshipCreateData
                                                           {
                                                               GameVersionId = 0,
                                                               Mode = (int)ChampionshipMode.Career,
                                                               Tracks = [1, 2, 3, 4, 5, 6]
                                                           })
                                  .ConfigureAwait(false);

        Assert.AreEqual(ChampionshipCreateResult.InvalidData, result, "A championship without a game version should be rejected!");
    }

    /// <summary>
    /// Verifies that a missing request body is rejected
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task ChampionshipServiceCreateChampionshipWithoutDataReturnsInvalidData()
    {
        var service = CreateService();

        var result = await service.CreateChampionshipAsync(null).ConfigureAwait(false);

        Assert.AreEqual(ChampionshipCreateResult.InvalidData, result, "A championship without data should be rejected!");
    }

    /// <summary>
    /// Verifies that an invalid session id is rejected before the database is queried
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task ChampionshipServiceAddSessionToChampionshipWithInvalidSessionReturnsInvalidSession()
    {
        var service = CreateService();

        var result = await service.AddSessionToChampionshipAsync(0).ConfigureAwait(false);

        Assert.AreEqual(ChampionshipSessionResult.InvalidSession, result, "A session id of zero should be rejected!");
    }

    #endregion // Methods
}