using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;

namespace F1Server.Tests.Repositories;

/// <summary>
/// Class to test the car telemetry repository
/// </summary>
[TestClass]
public class CarTelemetryRepositoryTests
{
    #region Static methods

    /// <summary>
    /// Adds a lap with one telemetry row for the given session
    /// </summary>
    /// <param name="dbFactory">Database factory</param>
    /// <param name="sessionId">Database id of the session</param>
    /// <param name="participantId">Database id of the participant</param>
    /// <param name="isInvalidLapTime">Mark the lap time as invalid?</param>
    /// <returns>Database id of the added lap</returns>
    private static long AddLapWithTelemetry(RepositoryFactory dbFactory, long sessionId, long participantId, bool isInvalidLapTime)
    {
        var lapRepository = dbFactory.GetRepository<LapRepository>();

        Assert.IsNotNull(lapRepository, "Lap repository should be resolvable!");

        var lapEntity = new LapEntity
                        {
                            SessionId = sessionId,
                            ParticipantId = participantId,
                            LapNumber = 1,
                            IsInvalidLapTime = isInvalidLapTime
                        };

        Assert.IsTrue(lapRepository.Add(lapEntity), "Adding the test lap should succeed!");

        var carTelemetryRepository = dbFactory.GetRepository<CarTelemetryRepository>();

        Assert.IsNotNull(carTelemetryRepository, "Car telemetry repository should be resolvable!");

        var telemetryEntity = new CarTelemetryEntity
                              {
                                  LapNumberId = lapEntity.Id,
                                  PacketNumber = 1
                              };

        Assert.IsTrue(carTelemetryRepository.Add(telemetryEntity), "Adding the test telemetry row should succeed!");

        return lapEntity.Id;
    }

    #endregion // Static methods

    #region Test methods

    /// <summary>
    /// Verifies that RemoveBySessionId removes the telemetry of all laps of the session
    /// </summary>
    [TestMethod]
    public void CarTelemetryRepositoryRemoveBySessionIdRemovesTelemetryOfSessionLaps()
    {
        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var sessionLapId = AddLapWithTelemetry(dbFactory, 430100, 430101, false);
            var otherSessionLapId = AddLapWithTelemetry(dbFactory, 430110, 430111, false);

            var carTelemetryRepository = dbFactory.GetRepository<CarTelemetryRepository>();

            Assert.IsNotNull(carTelemetryRepository, "Car telemetry repository should be resolvable!");
            Assert.IsTrue(carTelemetryRepository.RemoveBySessionId(430100), "Removing telemetry by session id should succeed!");

            var remainingLapIds = carTelemetryRepository.GetQuery()
                                                        ?.Where(t => t.LapNumberId == sessionLapId || t.LapNumberId == otherSessionLapId)
                                                        .Select(t => t.LapNumberId)
                                                        .ToList();

            Assert.IsNotNull(remainingLapIds, "Telemetry query should be resolvable!");
            Assert.DoesNotContain(sessionLapId, remainingLapIds, "Telemetry of the cleaned session should be removed!");
            Assert.Contains(otherSessionLapId, remainingLapIds, "Telemetry of other sessions should still be present!");
        }
    }

    /// <summary>
    /// Verifies that RemoveBySessionId restricted to invalid lap times keeps the telemetry of valid laps
    /// </summary>
    [TestMethod]
    public void CarTelemetryRepositoryRemoveBySessionIdInvalidLapsOnlyKeepsValidLapTelemetry()
    {
        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var invalidLapId = AddLapWithTelemetry(dbFactory, 430120, 430121, true);
            var validLapId = AddLapWithTelemetry(dbFactory, 430120, 430122, false);

            var carTelemetryRepository = dbFactory.GetRepository<CarTelemetryRepository>();

            Assert.IsNotNull(carTelemetryRepository, "Car telemetry repository should be resolvable!");
            Assert.IsTrue(carTelemetryRepository.RemoveBySessionId(430120, true), "Removing telemetry of invalid laps by session id should succeed!");

            var remainingLapIds = carTelemetryRepository.GetQuery()
                                                        ?.Where(t => t.LapNumberId == invalidLapId || t.LapNumberId == validLapId)
                                                        .Select(t => t.LapNumberId)
                                                        .ToList();

            Assert.IsNotNull(remainingLapIds, "Telemetry query should be resolvable!");
            Assert.DoesNotContain(invalidLapId, remainingLapIds, "Telemetry of the invalid lap should be removed!");
            Assert.Contains(validLapId, remainingLapIds, "Telemetry of the valid lap should still be present!");
        }
    }

    #endregion // Test methods
}