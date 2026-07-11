using System.Text.RegularExpressions;

using F1Server.Core.Enumerations;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;

using Microsoft.EntityFrameworkCore;

namespace F1Server.Tests.Repositories;

/// <summary>
/// Tests verifying that hot lap and telemetry queries run without auto-included navigations
/// </summary>
[TestClass]
public class AutoIncludeTests
{
    #region Constants

    /// <summary>
    /// Unique game session id used by the tests in this class
    /// </summary>
    private const ulong TestSessionUniqueId = 424424424424UL;

    #endregion // Constants

    #region Fields

    /// <summary>
    /// Database id of the participant created for the tests in this class
    /// </summary>
    private static long _participantDbId;

    /// <summary>
    /// Database id of the lap created for the tests in this class
    /// </summary>
    private static long _lapDbId;

    #endregion // Fields

    #region Static methods

    /// <summary>
    /// Creates the session, participant, lap and telemetry graph used by all tests in this class
    /// </summary>
    /// <param name="context">Test context</param>
    [ClassInitialize]
    public static void ClassInit(TestContext context)
    {
        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var driverEntity = new DriverEntity
                               {
                                   DriverGameId = 424001,
                                   Name = "AutoInclude Test Driver"
                               };

            Assert.IsTrue(dbFactory.GetRepository<DriverRepository>()?.Add(driverEntity), "Driver entity could not be added to the database!");

            var nationalityEntity = new NationalityEntity
                                    {
                                        NationalityGameId = 424,
                                        Name = "AutoInclude Test Nationality"
                                    };

            Assert.IsTrue(dbFactory.GetRepository<NationalityRepository>()?.Add(nationalityEntity), "Nationality entity could not be added to the database!");

            var teamEntity = new TeamEntity
                             {
                                 TeamGameId = 424002,
                                 Name = "AutoInclude Test Team"
                             };

            Assert.IsTrue(dbFactory.GetRepository<TeamRepository>()?.Add(teamEntity), "Team entity could not be added to the database!");

            var sessionEntity = new SessionEntity
                                {
                                    SessionId = TestSessionUniqueId,
                                    CreationTimestamp = DateTime.UtcNow,
                                    SessionType = SessionType.Race,
                                    TrackId = 1,
                                    GameVersionId = 1
                                };

            Assert.IsTrue(dbFactory.GetRepository<SessionRepository>()?.Add(sessionEntity), "Session entity could not be added to the database!");

            var participantEntity = new ParticipantEntity
                                    {
                                        SessionId = sessionEntity.Id,
                                        DriverId = driverEntity.Id,
                                        NationalityId = nationalityEntity.Id,
                                        TeamId = teamEntity.Id,
                                        DriverName = "AutoInclude Test Driver",
                                        ArrayIndex = 1
                                    };

            Assert.IsTrue(dbFactory.GetRepository<ParticipantRepository>()?.Add(participantEntity), "Participant entity could not be added to the database!");

            var lapEntity = new LapEntity
                            {
                                LapNumber = 1,
                                ParticipantId = participantEntity.Id,
                                SessionId = sessionEntity.Id,
                                DriverStatus = DriverStatus.FlyingLap,
                                PitStatus = PitStatus.None,
                                ResultStatus = ResultStatus.Active
                            };

            Assert.IsTrue(dbFactory.GetRepository<LapRepository>()?.Add(lapEntity), "Lap entity could not be added to the database!");

            var telemetryEntity = new CarTelemetryEntity
                                  {
                                      LapNumberId = lapEntity.Id,
                                      PacketNumber = 1,
                                      Speed = 250
                                  };

            Assert.IsTrue(dbFactory.GetRepository<CarTelemetryRepository>()?.Add(telemetryEntity), "Telemetry entity could not be added to the database!");

            _participantDbId = participantEntity.Id;
            _lapDbId = lapEntity.Id;
        }
    }

    #endregion // Static methods

    #region Methods

    /// <summary>
    /// Verifies that a plain lap query no longer loads the participant navigation automatically
    /// </summary>
    [TestMethod]
    public void LapRepositoryGetQueryDoesNotAutoIncludeParticipant()
    {
        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var storedLap = dbFactory.GetRepository<LapRepository>()
                                     ?.GetQuery()
                                     ?.FirstOrDefault(l => l.Id == _lapDbId);

            Assert.IsNotNull(storedLap, "The test lap should be found by a plain query!");
            Assert.IsNull(storedLap.Participant, "The participant navigation must not be loaded without an explicit include!");
        }
    }

    /// <summary>
    /// Verifies that an explicit include still loads the participant with its auto-included driver
    /// </summary>
    [TestMethod]
    public void LapRepositoryGetQueryIncludeParticipantLoadsDriver()
    {
        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var storedLap = dbFactory.GetRepository<LapRepository>()
                                     ?.GetQuery()
                                     ?.Include(l => l.Participant)
                                     .FirstOrDefault(l => l.Id == _lapDbId);

            Assert.IsNotNull(storedLap, "The test lap should be found by the including query!");
            Assert.IsNotNull(storedLap.Participant, "The participant navigation should be loaded by the explicit include!");
            Assert.AreEqual(_participantDbId, storedLap.Participant.Id, "The included participant should be the participant of the lap!");
            Assert.IsNotNull(storedLap.Participant.Driver, "The driver should still be loaded through the auto-included participant navigations!");
            Assert.AreEqual("AutoInclude Test Driver", storedLap.Participant.Driver.Name, "The driver of the included participant should be the test driver!");
        }
    }

    /// <summary>
    /// Verifies that a plain telemetry query no longer loads the lap navigation automatically
    /// </summary>
    [TestMethod]
    public void CarTelemetryRepositoryGetQueryDoesNotAutoIncludeLap()
    {
        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var storedRow = dbFactory.GetRepository<CarTelemetryRepository>()
                                     ?.GetQuery()
                                     ?.FirstOrDefault(t => t.LapNumberId == _lapDbId);

            Assert.IsNotNull(storedRow, "The test telemetry row should be found by a plain query!");
            Assert.IsNull(storedRow.Lap, "The lap navigation must not be loaded without an explicit include!");
        }
    }

    /// <summary>
    /// Verifies that the SQL of a lap lookup by participant and lap number contains no join
    /// </summary>
    [TestMethod]
    public void F1ServerDbContextLapLookupQueryContainsNoJoin()
    {
        var optionsBuilder = new DbContextOptionsBuilder<F1ServerDbContext>();

        optionsBuilder.UseSqlServer("Server=localhost;Database=F1TelemetryQueryString;Integrated Security=True;");

        using (var dbContext = new F1ServerDbContext(optionsBuilder.Options))
        {
            var lapSql = dbContext.Set<LapEntity>()
                                  .Where(l => l.ParticipantId == 1 && l.LapNumber == 1)
                                  .ToQueryString();

            StringAssert.DoesNotMatch(lapSql, new Regex("JOIN"), "The lap lookup must not join participant, driver, team or nationality tables!");
        }
    }

    /// <summary>
    /// Verifies that the SQL of a telemetry query by lap id contains no join
    /// </summary>
    [TestMethod]
    public void F1ServerDbContextTelemetryQueryContainsNoJoin()
    {
        var optionsBuilder = new DbContextOptionsBuilder<F1ServerDbContext>();

        optionsBuilder.UseSqlServer("Server=localhost;Database=F1TelemetryQueryString;Integrated Security=True;");

        using (var dbContext = new F1ServerDbContext(optionsBuilder.Options))
        {
            var telemetrySql = dbContext.Set<CarTelemetryEntity>()
                                        .Where(t => t.LapNumberId == 1)
                                        .ToQueryString();

            StringAssert.DoesNotMatch(telemetrySql, new Regex("JOIN"), "The telemetry query must not join the lap table!");
        }
    }

    #endregion // Methods
}