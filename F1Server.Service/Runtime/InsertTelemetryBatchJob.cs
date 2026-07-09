using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;

using Microsoft.Extensions.Logging;

namespace F1Server.Service.Runtime;

/// <summary>
/// Database writer job inserting the buffered car telemetry rows of a completed lap
/// </summary>
internal class InsertTelemetryBatchJob : IDatabaseWriterJob
{
    #region Properties

    /// <summary>
    /// Database id of the lap; when 0 the id is resolved by participant and lap number inside the job
    /// </summary>
    public long LapDbId { get; set; }

    /// <summary>
    /// Number of the lap, used to resolve the lap database id when <see cref="LapDbId"/> is 0
    /// </summary>
    public ushort LapNumber { get; set; }

    /// <summary>
    /// Database id of the participant the lap belongs to
    /// </summary>
    public long ParticipantDbId { get; set; }

    /// <summary>
    /// Buffered telemetry rows of the lap
    /// </summary>
    public List<CarTelemetryEntity> Rows { get; set; } = [];

    #endregion // Properties

    #region IDatabaseWriterJob

    /// <inheritdoc/>
    public async Task ExecuteAsync()
    {
        if (Rows.Count > 0)
        {
            using (var dbFactory = RepositoryFactory.CreateInstance())
            {
                var lapDbId = LapDbId;

                if (lapDbId == 0)
                {
                    var participantDbId = ParticipantDbId;
                    var lapNumber = LapNumber;

                    lapDbId = dbFactory.GetRepository<LapRepository>()
                                       ?.GetQuery()
                                       ?.FirstOrDefault(l => l.ParticipantId == participantDbId && l.LapNumber == lapNumber)
                                       ?.Id ?? 0;
                }

                if (lapDbId > 0)
                {
                    foreach (var telemetryEntity in Rows)
                    {
                        telemetryEntity.LapNumberId = lapDbId;
                    }

                    await (dbFactory.GetRepository<CarTelemetryRepository>()?.UpdateRangeAsync(Rows) ?? Task.FromResult(false)).ConfigureAwait(false);
                }
                else
                {
                    dbFactory.GetRepository<LapRepository>()?.Logger?.LogWarning("Discarding telemetry batch, lap {LapNumber} of participant {ParticipantDbId} was not found!", LapNumber, ParticipantDbId);
                }
            }
        }
    }

    #endregion // IDatabaseWriterJob
}