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
/// Process time trial packets
/// </summary>
internal class TimeTrialProcessor : BaseProcessor
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="serviceProvider">Service provider</param>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="gameInfo">Runtime game information</param>
    public TimeTrialProcessor(IServiceProvider serviceProvider, PacketHeader packetHeader, LiveGameData gameInfo)
        : base(serviceProvider, packetHeader, gameInfo)
    {
        Logger?.LogInformation("TimeTrialProcessor created.");
    }

    #endregion // Constructors

    #region BaseProcessor

    /// <inheritdoc/>
    public override bool Process(object? dataObject, SessionRuntimeData? sessionRuntimeData)
    {
        var isProcessed = true;

        LastException = string.Empty;

        using var currentActivity = AppActivity.SrvSource.StartActivity("TimeTrialProcessor");

        if (dataObject is TimeTrialData timeTrialData
            && timeTrialData.PacketData != null
            && sessionRuntimeData?.HasParticipants == true)
        {
            try
            {
                isProcessed = ProcessTimeTrialPacket(timeTrialData.PacketData, currentActivity);

                currentActivity?.SetStatus(ActivityStatusCode.Ok);
            }
            catch (Exception ex)
            {
                currentActivity?.SetStatus(ActivityStatusCode.Error, ex.ToString());
                currentActivity?.AddException(ex);

                LastException = ex.ToString();

                Logger?.LogError(ex, "Error processing time trial packet!");

                isProcessed = false;
            }
        }
        else
        {
            currentActivity?.SetStatus(ActivityStatusCode.Error, "Invalid data object!");

            Logger?.LogWarning("Unexpected data object or session not valid (processor: {Processor})!", nameof(TimeTrialProcessor));
        }

        return isProcessed;
    }

    #endregion // BaseProcessor

    #region Private methods

    /// <summary>
    /// Process time trial packet
    /// </summary>
    /// <param name="timeTrialData">Time trial data</param>
    /// <param name="activity">Tracing activity</param>
    /// <returns>True if processed successfully, false otherwise</returns>
    private bool ProcessTimeTrialPacket(ITimeTrialDataBase? timeTrialData, Activity? activity)
    {
        var isProcessed = false;

        if (timeTrialData != null)
        {
            _ = int.TryParse($"{GameInfo.GameVersion}{timeTrialData.PlayerSessionBestDataSet.TeamId}", out var playerSessionTeamId);
            _ = int.TryParse($"{GameInfo.GameVersion}{timeTrialData.PersonalBestDataSet.TeamId}", out var personalTeamId);
            _ = int.TryParse($"{GameInfo.GameVersion}{timeTrialData.RivalDataSet.TeamId}", out var rivalTeamId);

            using (var dbFactory = RepositoryFactory.CreateInstance())
            {
                TeamEntity? teamData = null;

                if (playerSessionTeamId > 0)
                {
                    teamData = dbFactory.GetRepository<TeamRepository>()?.GetQuery()?.FirstOrDefault(t => t.TeamGameId == playerSessionTeamId);

                    activity?.AddTag("f1.player_session_team", teamData?.Name);
                }

                if (personalTeamId > 0)
                {
                    teamData = dbFactory.GetRepository<TeamRepository>()?.GetQuery()?.FirstOrDefault(t => t.TeamGameId == playerSessionTeamId);

                    activity?.AddTag("f1.personal_team", teamData?.Name);
                }

                if (rivalTeamId > 0)
                {
                    teamData = dbFactory.GetRepository<TeamRepository>()?.GetQuery()?.FirstOrDefault(t => t.TeamGameId == playerSessionTeamId);

                    activity?.AddTag("f1.rival_team", teamData?.Name);
                }
            }

            isProcessed = true;
        }

        return isProcessed;
    }

    #endregion // Private methods
}