using System.Diagnostics;
using System.Runtime.CompilerServices;

using F1Server.Core.Data;
using F1Server.Core.Interfaces;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;
using F1Server.Data;
using F1Server.Db.Entity.Tables;
using F1Server.Service.Runtime;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace F1Server.Service.Processors;

/// <summary>
/// Process car telemetry packet
/// </summary>
internal class CarTelemetryProcessor : BaseProcessor
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
    /// <param name="packetHeader">Header of packet</param>
    /// <param name="gameInfo">Live game data</param>
    public CarTelemetryProcessor(IServiceProvider serviceProvider, PacketHeader packetHeader, LiveGameData gameInfo)
        : base(serviceProvider, packetHeader, gameInfo)
    {
        var applicationData = serviceProvider.GetRequiredService<F1ServerApplicationData>();

        _telemetryWriter = applicationData.TelemetryWriter;

        Logger?.LogInformation("CarTelemetryProcessor created.");
    }

    #endregion // Constructors

    #region Private methods

    /// <summary>
    /// Process car telemetry data
    /// </summary>
    /// <param name="sessionRuntimeData">Session runtime data</param>
    /// <param name="carIndex">Current car index</param>
    /// <param name="carTelemetryData">Car telemetry data</param>
    /// <param name="isTelemetryRecording">Is telemetry recording activated?</param>
    private void ProcessCarTelemetryData(SessionRuntimeData sessionRuntimeData, ushort carIndex, ICarTelemetryDataBase carTelemetryData, bool isTelemetryRecording)
    {
        if (sessionRuntimeData.Participants.TryGetValue(carIndex, out var participantData))
        {
            if (participantData.IsOnRecordableLap
                && isTelemetryRecording)
            {
                AnalyzeTelemetryData(carTelemetryData, participantData);
            }

            if (participantData.IsHumanDriver)
            {
                // Publish car telemetry
                PublishCarTelemetry(carTelemetryData, sessionRuntimeData, participantData.CurrentLapNumber);
            }
        }
    }

    /// <summary>
    /// Analyze telemetry data
    /// </summary>
    /// <param name="carTelemetryData">Car telemetry data</param>
    /// <param name="participantRuntimeData">Participant runtime data</param>
    private void AnalyzeTelemetryData(ICarTelemetryDataBase carTelemetryData, ParticipantRuntimeData participantRuntimeData)
    {
        try
        {
            if (participantRuntimeData.IsNewTelemetry)
            {
                // save all received telemetry data
                participantRuntimeData.CompleteTelemetryData(participantRuntimeData.CurrentLapNumber - 1);
            }

            // add new telemetry point
            var telemetryData = new CarTelemetryEntity
                                {
                                    PacketNumber = (int)CurrentFrameIdentifier,
                                    LapDistance = participantRuntimeData.LastLapDistance,
                                    Throttle = carTelemetryData.Throttle,
                                    Brake = carTelemetryData.Brake,
                                    Clutch = carTelemetryData.Clutch,
                                    Steer = carTelemetryData.Steer,
                                    EngineRPM = carTelemetryData.EngineRPM,
                                    EngineTemperature = carTelemetryData.EngineTemperature,
                                    Gear = carTelemetryData.Gear,
                                    Speed = carTelemetryData.Speed,
                                    IsDRS = carTelemetryData.IsDRS,
                                    RevLightsIndicator = carTelemetryData.RevLightsIndicator,
                                    BrakesTempFrontLeft = carTelemetryData.BrakesTemperature.FrontLeft,
                                    BrakesTempFrontRight = carTelemetryData.BrakesTemperature.FrontRight,
                                    BrakesTempRearLeft = carTelemetryData.BrakesTemperature.RearLeft,
                                    BrakesTempRearRight = carTelemetryData.BrakesTemperature.RearRight,
                                    TyresSurfaceTempFrontLeft = carTelemetryData.TyresSurfaceTemperature.FrontLeft,
                                    TyresSurfaceTempFrontRight = carTelemetryData.TyresSurfaceTemperature.FrontRight,
                                    TyresSurfaceTempRearLeft = carTelemetryData.TyresSurfaceTemperature.RearLeft,
                                    TyresSurfaceTempRearRight = carTelemetryData.TyresSurfaceTemperature.RearRight,
                                    TyresInnerTempFrontLeft = carTelemetryData.TyresInnerTemperature.FrontLeft,
                                    TyresInnerTempFrontRight = carTelemetryData.TyresInnerTemperature.FrontRight,
                                    TyresInnerTempRearLeft = carTelemetryData.TyresInnerTemperature.RearLeft,
                                    TyresInnerTempRearRight = carTelemetryData.TyresInnerTemperature.RearRight,
                                    TyresPressureFrontLeft = carTelemetryData.TyresPressure.FrontLeft,
                                    TyresPressureFrontRight = carTelemetryData.TyresPressure.FrontRight,
                                    TyresPressureRearLeft = carTelemetryData.TyresPressure.RearLeft,
                                    TyresPressureRearRight = carTelemetryData.TyresPressure.RearRight
                                };

            participantRuntimeData.AddTelemetryData(participantRuntimeData.CurrentLapNumber, telemetryData);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error while analyzing telemetry data for car number {CarNumber}.", participantRuntimeData.CarNumber);
        }
    }

    /// <summary>
    /// Publish telemetry metrics based on the car telemetry data
    /// </summary>
    /// <param name="carTelemetryData">Current car telemetry data</param>
    /// <param name="sessionRuntimeData">Session runtime data</param>
    /// <param name="currentLapNumber">Current lap number</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void PublishCarTelemetry(ICarTelemetryDataBase carTelemetryData, ISessionRuntimeData sessionRuntimeData, int currentLapNumber)
    {
        if (carTelemetryData != null
            && sessionRuntimeData != null
            && _telemetryWriter?.IsReady == true)
        {
            _telemetryWriter.WriteCarTelemetry(carTelemetryData, sessionRuntimeData, currentLapNumber);
        }
    }

    #endregion // Private methods

    #region BaseProcessor

    /// <inheritdoc/>
    public override bool Process(object? dataObject, SessionRuntimeData? sessionRuntimeData)
    {
        var isProcessed = true;

        LastException = string.Empty;

        var processWatch = Stopwatch.StartNew();

        if (dataObject is CarTelemetry carTelemetryPacket
            && carTelemetryPacket.PacketData != null
            && sessionRuntimeData?.IsValid == true)
        {
            try
            {
                if (sessionRuntimeData.HasParticipants)
                {
                    for (ushort currentCar = 0; currentCar < carTelemetryPacket.PacketData.CarTelemetryData.Length; ++currentCar)
                    {
                        var carTelemetryData = carTelemetryPacket.PacketData.CarTelemetryData[currentCar];

                        ProcessCarTelemetryData(sessionRuntimeData, currentCar, carTelemetryData, sessionRuntimeData.IsTelemetryRecording);
                    }
                }
            }
            catch (Exception ex)
            {
                LastException = ex.ToString();

                Logger?.LogError(ex, "Error processing CarTelemetry packet!");

                isProcessed = false;
            }
        }
        else
        {
            Logger?.LogWarning("Unexpected data object or session not valid (processor: {Processor})!", nameof(CarTelemetryProcessor));
        }

        processWatch.Stop();

        RecordSlowProcessingActivity(nameof(CarTelemetryProcessor), processWatch.Elapsed, isProcessed);

        return isProcessed;
    }

    #endregion // BaseProcessor
}