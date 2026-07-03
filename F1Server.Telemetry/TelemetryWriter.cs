using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

using F1Server.Core.Interfaces;
using F1Server.Core.Observability;
using F1Server.Core.Packets.Interfaces;

using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;

using Microsoft.Extensions.DependencyInjection;

namespace F1Server.Telemetry;

/// <summary>
/// Provides functionality to write car telemetry data to an InfluxDB instance
/// </summary>
public sealed class TelemetryWriter : ITelemetryWriter, IDisposable
{
    #region Consts

    /// <summary>
    /// Represents the default write capacity limit for the system
    /// </summary>
    private const int WriteCapacity = 1_000;

    /// <summary>
    /// The default format string used for formatting floating-point numbers
    /// </summary>
    private const string DefaultFloatFormat = "0.000";

    #endregion // Consts

    #region Fields

    /// <summary>
    /// Represents a thread-safe queue used to store strings for writing operations
    /// </summary>
    private readonly ConcurrentQueue<string> _writeQueue = new ConcurrentQueue<string>();

    /// <summary>
    /// Invariant culture info for formatting numbers
    /// </summary>
    private readonly CultureInfo _cultureInfo;

    /// <summary>
    /// The InfluxDB client instance used for writing telemetry data
    /// </summary>
    private InfluxDBClient? _influxDBClient;

    private CancellationTokenSource? _ctsWrite;

    #endregion // Fields

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="TelemetryWriter"/> class
    /// </summary>
    /// <param name="serviceProvider">The service provider for resolving dependencies</param>
    public TelemetryWriter(IServiceProvider serviceProvider)
    {
        _cultureInfo = CultureInfo.InvariantCulture;

        using var currentActivty = AppActivity.SrvSource.StartActivity("TelemetryWriter");

        Configuration = serviceProvider.GetRequiredService<TelemetryConfiguration>();

        if (Configuration.IsConfigured)
        {
            try
            {
                _influxDBClient = new InfluxDBClient(Configuration.InfluxDbHost, Configuration.Token);

                var isReady = _influxDBClient.ReadyAsync().GetAwaiter().GetResult();

                IsReady = isReady.Status == Ready.StatusEnum.Ready;

                currentActivty?.SetStatus(IsReady ? ActivityStatusCode.Ok : ActivityStatusCode.Error);
            }
            catch (Exception ex)
            {
                currentActivty?.SetStatus(ActivityStatusCode.Error);
                currentActivty?.AddException(ex);
            }

            EnsureWriteTaskRunning();
        }
    }

    #endregion // Constructor

    #region Properties

    /// <summary>
    /// Gets the telemetry configuration used by this writer
    /// </summary>
    public TelemetryConfiguration Configuration { get; }

    /// <summary>
    /// Gets a value indicating whether a write task is currently running
    /// </summary>
    public bool IsWriteTaskRunning { get; private set; }

    #endregion // Properties

    #region ITelemetryWriter

    #region Properties

    /// <summary>
    /// Gets a value indicating whether the telemetry writer is ready to write data
    /// </summary>
    public bool IsReady { get; }

    #endregion // Properties

    #region Methods

    /// <inheritdoc/>
    public void WriteSessionData(ISessionRuntimeData sessionRuntimeData, ILiveDriverData liveDriverData)
    {
        using var currentActivity = AppActivity.SrvSource.StartActivity("TelemetryWriteSessionData");

        if (IsReady)
        {
            var writeData = new StringBuilder(512);

            writeData.Append($"SessionData,session={sessionRuntimeData.CurrentSessionId},type={sessionRuntimeData.CurrentSessionType.ToString()}");

            var fields = new[]
                         {
                             $"airTemperature={sessionRuntimeData.AirTemperature}",
                             $"fastestLapTime={sessionRuntimeData.FastestLap}",
                             $"fastestSector1={sessionRuntimeData.FastestSector1}",
                             $"fastestSector2={sessionRuntimeData.FastestSector2}",
                             $"fastestSector3={sessionRuntimeData.FastestSector3}",
                             $"personalFastestLapTime={liveDriverData.FastestLapTime}",
                             $"personalFastestSector1={liveDriverData.FastestSector1}",
                             $"personalFastestSector2={liveDriverData.FastestSector2}",
                             $"personalFastestSector3={liveDriverData.FastestSector3}",
                             $"trackTemperature={sessionRuntimeData.TrackTemperature}"
                         };

            writeData.Append(' ');
            writeData.AppendJoin(',', fields);

            _writeQueue.Enqueue(writeData.ToString());

            EnsureWriteTaskRunning();

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }
    }

    /// <inheritdoc/>
    public void WriteLapData(IIndependentLapData lapData, ILapDataBase lapInfo, ISessionRuntimeData sessionRuntimeData)
    {
        using var currentActivity = AppActivity.SrvSource.StartActivity("TelemetryWriteLapData");

        if (IsReady)
        {
            var writeData = new StringBuilder(512);

            var lapDistance = lapInfo.LapDistance < 0 ? 0 : lapInfo.LapDistance;

            writeData.Append($"LapData,lapNumber={lapInfo.CurrentLapNumber},session={sessionRuntimeData.CurrentSessionId},type={sessionRuntimeData.CurrentSessionType.ToString()}");

            var fields = new[]
                         {
                             $"carPosition={lapInfo.CarPosition}",
                             $"currentLapNumber={lapInfo.CurrentLapNumber}",
                             $"driverStatus=\"{lapInfo.CurrentDriverStatus.ToString()}\"",
                             $"gridPosition={lapInfo.GridPosition}",
                             $"lapDistance={lapDistance.ToString("0.00", _cultureInfo)}",
                             $"lapTime={lapData.CurrentLapTime}",
                             $"lastLapTime={lapData.LastLapTime}",
                             $"sector1Time={lapData.Sector1Time}",
                             $"sector2Time={lapData.Sector2Time}",
                             $"sector3Time={lapData.Sector3Time}"
                         };

            writeData.Append(' ');
            writeData.AppendJoin(',', fields);

            _writeQueue.Enqueue(writeData.ToString());

            EnsureWriteTaskRunning();

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }
    }

    /// <inheritdoc/>
    public void WriteCarTelemetry(ICarTelemetryDataBase carTelemetryData, ISessionRuntimeData sessionRuntimeData, int currentLapNumber)
    {
        using var currentActivity = AppActivity.SrvSource.StartActivity("TelemetryWriteCarTelemetry");

        if (IsReady)
        {
            var writeData = new StringBuilder(512);

            writeData.Append($"CarTelemetry,lapNumber={currentLapNumber},session={sessionRuntimeData.CurrentSessionId},type={sessionRuntimeData.CurrentSessionType}");

            var fields = new[]
                         {
                             $"brake={carTelemetryData.Brake.ToString("0.00", _cultureInfo)}",
                             $"brakeTempFrontLeft={carTelemetryData.BrakesTemperature.FrontLeft}",
                             $"brakeTempFrontRight={carTelemetryData.BrakesTemperature.FrontRight}",
                             $"brakeTempRearLeft={carTelemetryData.BrakesTemperature.RearLeft}",
                             $"brakeTempRearRight={carTelemetryData.BrakesTemperature.RearRight}",
                             $"engineRPM={carTelemetryData.EngineRPM}",
                             $"engineTemperature={carTelemetryData.EngineTemperature}",
                             $"gear={carTelemetryData.Gear}",
                             $"speed={carTelemetryData.Speed}",
                             $"throttle={carTelemetryData.Throttle.ToString("0.00", _cultureInfo)}",
                             $"tyreInnerTempFrontLeft={carTelemetryData.TyresInnerTemperature.FrontLeft}",
                             $"tyreInnerTempFrontRight={carTelemetryData.TyresInnerTemperature.FrontRight}",
                             $"tyreInnerTempRearLeft={carTelemetryData.TyresInnerTemperature.RearLeft}",
                             $"tyreInnerTempRearRight={carTelemetryData.TyresInnerTemperature.RearRight}",
                             $"tyrePressureFrontLeft={carTelemetryData.TyresPressure.FrontLeft.ToString(DefaultFloatFormat, _cultureInfo)}",
                             $"tyrePressureFrontRight={carTelemetryData.TyresPressure.FrontRight.ToString(DefaultFloatFormat, _cultureInfo)}",
                             $"tyrePressureRearLeft={carTelemetryData.TyresPressure.RearLeft.ToString(DefaultFloatFormat, _cultureInfo)}",
                             $"tyrePressureRearRight={carTelemetryData.TyresPressure.RearRight.ToString(DefaultFloatFormat, _cultureInfo)}",
                             $"tyreSurfaceTempFrontLeft={carTelemetryData.TyresSurfaceTemperature.FrontLeft}",
                             $"tyreSurfaceTempFrontRight={carTelemetryData.TyresSurfaceTemperature.FrontRight}",
                             $"tyreSurfaceTempRearLeft={carTelemetryData.TyresSurfaceTemperature.RearLeft}",
                             $"tyreSurfaceTempRearRight={carTelemetryData.TyresSurfaceTemperature.RearRight}"
                         };

            writeData.Append(' ');
            writeData.AppendJoin(',', fields);

            _writeQueue.Enqueue(writeData.ToString());

            EnsureWriteTaskRunning();

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }
    }

    /// <inheritdoc/>
    public void WriteCarStatus(ICarStatusDataBase carStatusData, ISessionRuntimeData sessionRuntimeData, int currentLapNumber)
    {
        using var currentActivity = AppActivity.SrvSource.StartActivity("TelemetryWriteCarStatus");

        if (IsReady)
        {
            var writeData = new StringBuilder(512);

            writeData.Append($"CarStatus,lapNumber={currentLapNumber},session={sessionRuntimeData.CurrentSessionId},type={sessionRuntimeData.CurrentSessionType.ToString()}");

            var fields = new[]
                         {
                             $"ersDeployed={carStatusData.ERSDeployedThisLap.ToString(DefaultFloatFormat, _cultureInfo)}",
                             $"ersHarvestedMGUH={carStatusData.ERSHarvestedThisLapMGUH.ToString(DefaultFloatFormat, _cultureInfo)}",
                             $"ersHarvestedMGUK={carStatusData.ERSHarvestedThisLapMGUK.ToString(DefaultFloatFormat, _cultureInfo)}",
                             $"fuelLevel={carStatusData.FuelInTank.ToString("0.00", _cultureInfo)}",
                             $"fuelRemainingLaps={carStatusData.FuelRemainingLaps.ToString("0.00", _cultureInfo)}",
                             $"tyreCompound=\"{carStatusData.VisualTyreCompound.ToString()}\""
                         };

            writeData.Append(' ');
            writeData.AppendJoin(',', fields);

            _writeQueue.Enqueue(writeData.ToString());

            EnsureWriteTaskRunning();

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }
    }

    /// <summary>
    /// Ensures that the write task is running by starting it if it is not already active
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsureWriteTaskRunning()
    {
        if (IsWriteTaskRunning == false)
        {
            IsWriteTaskRunning = true;

            _ctsWrite?.Cancel();
            _ctsWrite?.Dispose();

            _ctsWrite = new CancellationTokenSource();

            using (ExecutionContext.SuppressFlow())
            {
                Task.Run(() => WriteToInflux(_ctsWrite.Token), _ctsWrite.Token);
            }
        }
    }

    /// <summary>
    /// Writes a collection of telemetry records to an InfluxDB instance asynchronously
    /// </summary>
    /// <param name="records">A list of telemetry records to be written. Each record should be formatted according to InfluxDB's expected input</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. If the operation is canceled, the method will terminate early</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    private async Task WriteRecordsToInflux(List<string> records, CancellationToken cancellationToken)
    {
        using var currentActivity = AppActivity.SrvSource.StartActivity("TelemetryWriteToInflux", ActivityKind.Producer, null);

        try
        {
            if (IsReady)
            {
                var writeApi = _influxDBClient?.GetWriteApiAsync();

                if (writeApi != null)
                {
                    currentActivity?.SetTag("f1.telemetry_records", records.Count);

                    await writeApi.WriteRecordsAsync(records, WritePrecision.Ms, Configuration.Bucket, Configuration.Organization, cancellationToken).ConfigureAwait(true);

                    currentActivity?.SetStatus(ActivityStatusCode.Ok);
                }
            }
        }
        catch (Exception ex)
        {
            currentActivity?.SetStatus(ActivityStatusCode.Error, "Failed to write telemetry data to InfluxDB");
            currentActivity?.AddException(ex);
        }
    }

    #endregion // Methods

    #region Task methods

    /// <summary>
    /// Writes data from the internal queue to InfluxDB asynchronously
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to signal the cancellation of the write operation</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    private async Task WriteToInflux(CancellationToken cancellationToken)
    {
        var records = new List<string>(WriteCapacity);

        while (cancellationToken.IsCancellationRequested == false)
        {
            if (_writeQueue.IsEmpty == false)
            {
                while (_writeQueue.TryDequeue(out var writeData) && string.IsNullOrEmpty(writeData) == false)
                {
                    records.Add(writeData);

                    // If we have reached the write capacity, write the records to InfluxDB
                    if (records.Count == records.Capacity)
                    {
                        await WriteRecordsToInflux(records, cancellationToken).ConfigureAwait(false);

                        records.Clear();
                    }
                }

                // If there are any remaining records after the queue is empty, write them to InfluxDB
                if (records.Count > 0)
                {
                    await WriteRecordsToInflux(records, cancellationToken).ConfigureAwait(false);

                    records.Clear();
                }
            }

            cancellationToken.WaitHandle.WaitOne(500);
        }

        IsWriteTaskRunning = false;
    }

    #endregion // Task methods

    #endregion // ITelemetryWriter

    #region IDisposable

    /// <summary>
    /// Releases all resources used by the current instance of the <see cref="TelemetryWriter"/> class
    /// </summary>
    public void Dispose()
    {
        _ctsWrite?.Cancel();
        _ctsWrite?.Dispose();

        _ctsWrite = null;

        _influxDBClient?.Dispose();

        _influxDBClient = null;
    }

    #endregion // IDisposable
}