using System.Diagnostics;
using System.Runtime.CompilerServices;

using F1Server.Core.Enumerations;
using F1Server.Core.Interfaces;
using F1Server.Core.Observability;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;
using F1Server.Data;
using F1Server.Service.Runtime;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace F1Server.Service.Processors;

/// <summary>
/// Process car status packets
/// </summary>
internal class CarStatusProcessor : BaseProcessor
{
    #region Fields

    /// <summary>
    /// Telemetry writer to write telemetry data
    /// </summary>
    private readonly ITelemetryWriter? _telemetryWriter;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="serviceProvider">Service provider</param>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="gameInfo">Runtime game information</param>
    public CarStatusProcessor(IServiceProvider serviceProvider, PacketHeader packetHeader, LiveGameData gameInfo)
        : base(serviceProvider, packetHeader, gameInfo)
    {
        var applicationData = serviceProvider.GetRequiredService<F1ServerApplicationData>();

        _telemetryWriter = applicationData.TelemetryWriter;

        Logger?.LogInformation("CarStatusProcessor created.");
    }

    #endregion // Constructors

    #region BaseProcessor

    /// <inheritdoc/>
    public override bool Process(object? dataObject, SessionRuntimeData? sessionRuntimeData)
    {
        var isProcessed = true;

        LastException = string.Empty;

        using var currentActivity = AppActivity.SrvSource.StartActivity("CarStatusProcessor");

        if (dataObject is CarStatus carStatusPacket && carStatusPacket.PacketData != null && sessionRuntimeData?.IsValid == true)
        {
            try
            {
                if (sessionRuntimeData.HasParticipants)
                {
                    for (ushort carIndex = 0; carIndex < carStatusPacket.PacketData.CarStatusData.Length; ++carIndex)
                    {
                        var carStatusData = carStatusPacket.PacketData.CarStatusData[carIndex];

                        ProcessCarStatusData(sessionRuntimeData, carStatusData, carIndex);
                    }
                }

                currentActivity?.SetStatus(ActivityStatusCode.Ok);
            }
            catch (Exception ex)
            {
                currentActivity?.SetStatus(ActivityStatusCode.Error, ex.ToString());
                currentActivity?.AddException(ex);

                LastException = ex.ToString();

                isProcessed = false;

                Logger?.LogError(ex, "Error processing CarStatus packet!");
            }
        }
        else
        {
            currentActivity?.SetStatus(ActivityStatusCode.Error, "Unexpected data object or session not valid!");

            Logger?.LogWarning("Unexpected data object or session not valid (processor: {Processor})!", nameof(CarStatusProcessor));
        }

        return isProcessed;
    }

    #endregion // BaseProcessor

    #region Private methods

    /// <summary>
    /// Process car status data for a specific car
    /// </summary>
    /// <param name="sessionRuntimeData">Session runtime data</param>
    /// <param name="carStatusData">Car status data</param>
    /// <param name="carIndex">Current car index</param>
    private void ProcessCarStatusData(SessionRuntimeData sessionRuntimeData, ICarStatusDataBase carStatusData, ushort carIndex)
    {
        if (sessionRuntimeData.Participants.TryGetValue(carIndex, out var participantData) && participantData != null)
        {
            SetCurrentTyres(sessionRuntimeData, carStatusData, participantData);

            if (participantData.IsHumanDriver)
            {
                PublishTelemteryMetrics(carStatusData, sessionRuntimeData, participantData.CurrentLapNumber);
            }
        }
    }

    /// <summary>
    /// Get current tyres
    /// </summary>
    /// <param name="sessionRuntimeData">Session runtime data</param>
    /// <param name="carStatusData">Car status data</param>
    /// <param name="participantData">Participant data</param>
    private void SetCurrentTyres(SessionRuntimeData sessionRuntimeData, ICarStatusDataBase carStatusData, ParticipantRuntimeData participantData)
    {
        var currentLap = participantData.GetCurrentLap();

        currentLap?.TyreCompound = carStatusData?.VisualTyreCompound ?? VisualTyreCompound.Unknown;

        if (sessionRuntimeData.CurrentSession.Drivers?.Find(p => p.DbId == participantData.ParticipantDbId) is LiveDriverData liveData)
        {
            liveData.CurrentUsedTyre = carStatusData?.VisualTyreCompound ?? VisualTyreCompound.Unknown;
        }
    }

    /// <summary>
    /// Publishes telemetry metrics related to the car's status
    /// </summary>
    /// <param name="carStatusData">The car status data containing telemetry information</param>
    /// <param name="sessionRuntimeData">Session runtime data</param>
    /// <param name="currentLapNumber">Current lap number of the participant</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void PublishTelemteryMetrics(ICarStatusDataBase carStatusData, ISessionRuntimeData sessionRuntimeData, int currentLapNumber)
    {
        if (carStatusData != null
            && _telemetryWriter?.IsReady == true)
        {
            _telemetryWriter.WriteCarStatus(carStatusData, sessionRuntimeData, currentLapNumber);
        }
    }

    #endregion // Private methods
}