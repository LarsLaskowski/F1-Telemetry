using F1Server.Db.Entity.Tables;
using F1Server.Service.Cache;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F1Server.Tests.Cache;

/// <summary>
/// Contains unit tests for verifying the behavior of various repository cache implementations
/// </summary>
[TestClass]
public class RepositoryCacheTests
{
    #region Methods

    /// <summary>
    /// Tests the functionality of the <see cref="DriverRepositoryCache"/> class, including adding and retrieving
    /// </summary>
    [TestMethod]
    public void DriverRepositoryCacheTest()
    {
        var driver = new DriverEntity
                     {
                         Id = 1,
                         DriverGameId = 99,
                         Name = "TestDriver"
                     };

        DriverRepositoryCache.AddOrUpdate(driver);

        Assert.AreEqual(driver, DriverRepositoryCache.GetById(1));
        Assert.AreEqual(driver, DriverRepositoryCache.GetByGameId(99));
    }

    /// <summary>
    /// Tests the functionality of the <see cref="ParticipantsRepositoryCache"/> class, including adding and retrieving
    /// </summary>
    [TestMethod]
    public void ParticipantsRepositoryCacheTest()
    {
        var part = new ParticipantEntity
                   {
                       Id = 5,
                       SessionId = 100,
                       DriverId = 200,
                       DriverName = "TestP"
                   };

        ParticipantsRepositoryCache.AddOrUpdate(part);

        Assert.AreEqual(part, ParticipantsRepositoryCache.GetById(5));
        Assert.AreEqual(part, ParticipantsRepositoryCache.GetBySessionAndDriverId(100, 200));
    }

    /// <summary>
    /// Verifies that the <see cref="SessionRepositoryCache"/> correctly handles adding and retrieving
    /// </summary>
    [TestMethod]
    public void SessionRepositoryCacheTest()
    {
        var session = new SessionEntity
                      {
                          Id = 600,
                          SessionId = 123456UL
                      };

        SessionRepositoryCache.AddOrUpdate(session);

        Assert.AreEqual(session, SessionRepositoryCache.GetById(600));
        Assert.AreEqual(session, SessionRepositoryCache.GetByUniqueSessionId(123456UL));

        var attr = new SessionAttributesEntity
                   {
                       Id = 700,
                       SessionId = 600
                   };

        SessionRepositoryCache.AddOrUpdateAttributes(attr);

        Assert.AreEqual(attr, SessionRepositoryCache.GetAttributesBySessionId(600));
    }

    /// <summary>
    /// Tests the functionality of the <see cref="LapRepositoryCache"/> class, including adding and retrieving
    /// </summary>
    [TestMethod]
    public void LapRepositoryCacheTest()
    {
        var lap = new LapEntity
                  {
                      Id = 8,
                      LapNumber = 1,
                      ParticipantId = 9
                  };

        LapRepositoryCache.AddOrUpdate(lap);

        Assert.AreEqual(lap, LapRepositoryCache.GetById(8));
        Assert.AreEqual(lap, LapRepositoryCache.GetByLapNumberParticipant(1, 9));
    }

    #endregion // Methods
}