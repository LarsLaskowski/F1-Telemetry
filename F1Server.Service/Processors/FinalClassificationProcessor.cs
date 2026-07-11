using System.Diagnostics;

using F1Server.Core.Observability;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;
using F1Server.Data;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;
using F1Server.Service.Runtime;

using Microsoft.Extensions.Logging;

namespace F1Server.Service.Processors;

/// <summary>
/// Process final classification packets
/// </summary>
internal class FinalClassificationProcessor : BaseProcessor
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="serviceProvider">Service provider</param>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="gameInfo">Runtime game information</param>
    public FinalClassificationProcessor(IServiceProvider serviceProvider, PacketHeader packetHeader, LiveGameData gameInfo)
        : base(serviceProvider, packetHeader, gameInfo)
    {
        Logger?.LogInformation("FinalClassificationProcessor created.");
    }

    #endregion // Constructors

    #region Private methods

    /// <summary>
    /// Process final classification packet
    /// </summary>
    /// <param name="sessionRuntimeData">Session runtime data</param>
    /// <param name="finalClassificationData">Final classification data</param>
    /// <returns>True if processed successfully, false otherwise</returns>
    private bool ProcessFinalClassificationPacket(SessionRuntimeData sessionRuntimeData, FinalClassificationData finalClassificationData)
    {
        var isProcessed = true;

        // All pending telemetry batches must be written before laps and telemetry of this session are read and cleaned up
        DatabaseWriter.Flush();

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            for (ushort carIndex = 0; carIndex < finalClassificationData.PacketData!.NumberOfCars; carIndex++)
            {
                try
                {
                    var finalData = finalClassificationData.PacketData.FinalClassifications[carIndex];

                    sessionRuntimeData.Participants.TryGetValue(carIndex, out var participant);

                    if (finalData != null
                        && participant != null
                        && AddOrRefreshFinalClassification(sessionRuntimeData.SessionDbId, participant.ParticipantDbId, finalData, dbFactory) == false)
                    {
                        isProcessed = false;
                    }
                }
                catch (Exception ex)
                {
                    Logger?.LogError(ex, "Error processing final classification data for car index {CarIndex}.", carIndex);
                }
            }

            // Cleanup invalid laps and their telemetry with set-based statements
            dbFactory.GetRepository<CarTelemetryRepository>()?.RemoveBySessionId(sessionRuntimeData.SessionDbId, true);

            dbFactory.GetRepository<LapRepository>()?.RemoveWhere(l => l.SessionId == sessionRuntimeData.SessionDbId && l.DbIsInvalidLapTime == 1);
        }

        return isProcessed;
    }

    /// <summary>
    /// Add or refresh final classification database entity
    /// </summary>
    /// <param name="sessionDbId">Database id of session</param>
    /// <param name="participantId">Database id of participant</param>
    /// <param name="finalCarData">Packet data</param>
    /// <param name="dbFactory">Database object</param>
    /// <returns>Status</returns>
    private bool AddOrRefreshFinalClassification(long sessionDbId, long participantId, IFinalClassificationCarBase finalCarData, RepositoryFactory dbFactory)
    {
        var isProcessed = false;

        var finalClassification = dbFactory.GetRepository<FinalClassificationRepository>()
                                           ?.GetQuery()
                                           ?.FirstOrDefault(s => s.SessionId == sessionDbId && s.ParticipantId == participantId);

        finalClassification ??= new FinalClassificationEntity();

        isProcessed = dbFactory.GetRepository<FinalClassificationRepository>()
                               ?.AddOrRefresh(f => f.Id == finalClassification.Id,
                                              obj =>
                                              {
                                                  obj.ParticipantId = participantId;
                                                  obj.SessionId = sessionDbId;
                                                  obj.FinishPosition = finalCarData.Position;
                                                  obj.GridPosition = finalCarData.GridPosition;
                                                  obj.FastestLapTime = finalCarData.BestLapTimeInMs;
                                                  obj.LapsDriven = finalCarData.LapsCompleted;
                                                  obj.NumberOfPenalties = finalCarData.NumPenalties;
                                                  obj.PenaltiesTime = finalCarData.PenaltiesTime;
                                                  obj.PitStops = finalCarData.PitStops;
                                                  obj.ResultStatus = finalCarData.ResultStatus;
                                                  obj.TotalRaceTime = finalCarData.TotalRaceTime;
                                              }) == true;

        return isProcessed;
    }

    #endregion // Private methods

    #region BaseProcessor

    /// <inheritdoc/>
    public override bool Process(object? dataObject, SessionRuntimeData? sessionRuntimeData)
    {
        var isProcessed = true;

        LastException = string.Empty;

        using var currentActivity = AppActivity.SrvSource.StartActivity("FinalClassificationProcessor");

        if (dataObject is FinalClassificationData finalClassificationData
            && finalClassificationData.PacketData != null
            && sessionRuntimeData?.IsFinished == true
            && sessionRuntimeData.HasParticipants)
        {
            try
            {
                isProcessed = ProcessFinalClassificationPacket(sessionRuntimeData, finalClassificationData);

                currentActivity?.SetStatus(ActivityStatusCode.Ok);
            }
            catch (Exception ex)
            {
                currentActivity?.SetStatus(ActivityStatusCode.Error, ex.ToString());
                currentActivity?.AddException(ex);

                LastException = ex.ToString();

                Logger?.LogError(ex, "Error processing final classification packet!");

                isProcessed = false;
            }
        }
        else
        {
            currentActivity?.SetStatus(ActivityStatusCode.Error, "Unexpected data object or session not finished!");

            Logger?.LogWarning("Unexpected data object or session not valid (processor: {Processor})!", nameof(FinalClassificationProcessor));
        }

        return isProcessed;
    }

    #endregion // BaseProcessor
}