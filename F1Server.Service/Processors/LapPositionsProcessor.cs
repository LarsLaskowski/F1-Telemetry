using System.Diagnostics;

using F1Server.Core.Observability;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;
using F1Server.Data;
using F1Server.Service.Runtime;

using Microsoft.Extensions.Logging;

namespace F1Server.Service.Processors;

/// <summary>
/// Process lap positions packets
/// </summary>
internal class LapPositionsProcessor : BaseProcessor
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="serviceProvider">Service provider</param>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="gameInfo">Runtime game information</param>
    public LapPositionsProcessor(IServiceProvider serviceProvider, PacketHeader packetHeader, LiveGameData gameInfo)
        : base(serviceProvider, packetHeader, gameInfo)
    {
        Logger?.LogInformation("LapPositionsProcessor created.");
    }

    #endregion // Constructors

    #region Private methods

    /// <summary>
    /// Process lap positions packet
    /// </summary>
    /// <param name="lapPositions">Lap positions data</param>
    /// <returns>Status</returns>
    private bool ProcessLapPositionsPacket(ILapPositionsBase? lapPositions)
    {
        var isProcessed = false;

        if (lapPositions != null)
        {
            isProcessed = true;
        }

        return isProcessed;
    }

    #endregion // Private methods

    #region BaseProcessor

    /// <inheritdoc/>
    public override bool Process(object? dataObject, SessionRuntimeData? sessionRuntimeData)
    {
        var isProcessed = true;

        LastException = string.Empty;

        using var currentActivity = AppActivity.SrvSource.StartActivity("LapPositionsProcessor");

        if (dataObject is LapPositions lapPositions
            && lapPositions.PacketData != null
            && sessionRuntimeData?.HasParticipants == true)
        {
            try
            {
                isProcessed = ProcessLapPositionsPacket(lapPositions.PacketData);

                currentActivity?.SetStatus(ActivityStatusCode.Ok);
            }
            catch (Exception ex)
            {
                currentActivity?.SetStatus(ActivityStatusCode.Error, ex.ToString());
                currentActivity?.AddException(ex);

                LastException = ex.ToString();

                Logger?.LogError(ex, "Error processing LapPositions packet!");

                isProcessed = false;
            }
        }
        else
        {
            currentActivity?.SetStatus(ActivityStatusCode.Error, "Unexpected data object or session not valid!");

            Logger?.LogWarning("Unexpected data object or session not valid (processor: {Processor})!", nameof(LapPositionsProcessor));
        }

        return isProcessed;
    }

    #endregion // BaseProcessor
}