using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;
using F1Server.Service.Cache;

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
    public void DriverRepositoryCacheAddOrUpdateReturnsCachedDriver()
    {
        var driver = new DriverEntity
                     {
                         Id = 1,
                         DriverGameId = 99,
                         Name = "TestDriver"
                     };

        DriverRepositoryCache.AddOrUpdate(driver);

        Assert.AreEqual(driver, DriverRepositoryCache.GetById(1), "The cached driver should be returned by id!");
        Assert.AreEqual(driver, DriverRepositoryCache.GetByGameId(99), "The cached driver should be returned by game id!");
    }

    /// <summary>
    /// Tests the functionality of the <see cref="ParticipantsRepositoryCache"/> class, including adding and retrieving
    /// </summary>
    [TestMethod]
    public void ParticipantsRepositoryCacheAddOrUpdateReturnsCachedParticipant()
    {
        var part = new ParticipantEntity
                   {
                       Id = 5,
                       SessionId = 100,
                       DriverId = 200,
                       DriverName = "TestP"
                   };

        ParticipantsRepositoryCache.AddOrUpdate(part);

        Assert.AreEqual(part, ParticipantsRepositoryCache.GetById(5), "The cached participant should be returned by id!");
        Assert.AreEqual(part, ParticipantsRepositoryCache.GetBySessionAndDriverId(100, 200), "The cached participant should be returned by session and driver id!");
    }

    /// <summary>
    /// Verifies that the <see cref="SessionRepositoryCache"/> correctly handles adding and retrieving
    /// </summary>
    [TestMethod]
    public void SessionRepositoryCacheAddOrUpdateReturnsCachedSessionAndAttributes()
    {
        var session = new SessionEntity
                      {
                          Id = 600,
                          SessionId = 123456UL
                      };

        SessionRepositoryCache.AddOrUpdate(session);

        Assert.AreEqual(session, SessionRepositoryCache.GetById(600), "The cached session should be returned by id!");
        Assert.AreEqual(session, SessionRepositoryCache.GetByUniqueSessionId(123456UL), "The cached session should be returned by unique session id!");

        var attr = new SessionAttributesEntity
                   {
                       Id = 700,
                       SessionId = 600
                   };

        SessionRepositoryCache.AddOrUpdateAttributes(attr);

        Assert.AreEqual(attr, SessionRepositoryCache.GetAttributesBySessionId(600), "The cached session attributes should be returned by session id!");
    }

    /// <summary>
    /// Tests the functionality of the <see cref="LapRepositoryCache"/> class, including adding and retrieving
    /// </summary>
    [TestMethod]
    public void LapRepositoryCacheAddOrUpdateReturnsCachedLap()
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

    /// <summary>
    /// Verifies that <see cref="TeamRepositoryCache"/> lazily loads teams from the database
    /// </summary>
    [TestMethod]
    public void TeamRepositoryCacheLoadTeamsReturnsAddedTeam()
    {
        TeamEntity teamEntity;

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            teamEntity = new TeamEntity
                         {
                             TeamGameId = 999001,
                             Name = "RepositoryCacheTests Team"
                         };

            Assert.IsTrue(dbFactory.GetRepository<TeamRepository>()?.Add(teamEntity), "Team entity could not be added to the database!");
        }

        TeamRepositoryCache.Clear();

        var isLoaded = TeamRepositoryCache.LoadTeams();

        Assert.IsTrue(isLoaded, "Teams should be loaded from the database!");

        var team = TeamRepositoryCache.GetByGameId(999001);

        Assert.IsNotNull(team, "The added team should be present in the cache!");
        Assert.AreEqual(teamEntity.Id, team.Id, "The cached team should match the added entity!");
        Assert.AreEqual(team, TeamRepositoryCache.GetById(team.Id), "The same entity should be returned by id!");
    }

    /// <summary>
    /// Verifies that <see cref="TrackRepositoryCache"/> lazily loads tracks from the database
    /// </summary>
    [TestMethod]
    public void TrackRepositoryCacheLoadTracksReturnsAddedTrack()
    {
        TrackEntity trackEntity;

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            trackEntity = new TrackEntity
                          {
                              TrackNumber = 999,
                              Name = "RepositoryCacheTests Track"
                          };

            Assert.IsTrue(dbFactory.GetRepository<TrackRepository>()?.Add(trackEntity), "Track entity could not be added to the database!");
        }

        TrackRepositoryCache.Clear();

        var isLoaded = TrackRepositoryCache.LoadTracks();

        Assert.IsTrue(isLoaded, "Tracks should be loaded from the database!");

        var track = TrackRepositoryCache.GetByTrackNumber(999);

        Assert.IsNotNull(track, "The added track should be present in the cache!");
        Assert.AreEqual(trackEntity.Id, track.Id, "The cached track should match the added entity!");
        Assert.AreEqual(track, TrackRepositoryCache.GetById(track.Id), "The same entity should be returned by id!");
    }

    /// <summary>
    /// Verifies that <see cref="NationalityRepositoryCache"/> lazily loads nationalities from the database
    /// </summary>
    [TestMethod]
    public void NationalityRepositoryCacheLoadNationalitiesReturnsAddedNationality()
    {
        NationalityEntity nationalityEntity;

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            nationalityEntity = new NationalityEntity
                                {
                                    NationalityGameId = 999,
                                    Name = "RepositoryCacheTests Nationality"
                                };

            Assert.IsTrue(dbFactory.GetRepository<NationalityRepository>()?.Add(nationalityEntity), "Nationality entity could not be added to the database!");
        }

        NationalityRepositoryCache.Clear();

        var isLoaded = NationalityRepositoryCache.LoadNationalities();

        Assert.IsTrue(isLoaded, "Nationalities should be loaded from the database!");

        var nationality = NationalityRepositoryCache.GetByGameId(999);

        Assert.IsNotNull(nationality, "The added nationality should be present in the cache!");
        Assert.AreEqual(nationalityEntity.Id, nationality.Id, "The cached nationality should match the added entity!");
        Assert.AreEqual(nationality, NationalityRepositoryCache.GetById(nationality.Id), "The same entity should be returned by id!");
    }

    #endregion // Methods
}