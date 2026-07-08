using System.Collections.Concurrent;

using F1Server.Core.Enumerations;
using F1Server.Data;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;

namespace F1Server.Service.Runtime;

/// <summary>
/// Runtime data of one participant in current session
/// </summary>
public class ParticipantRuntimeData : IDisposable
{
    #region Fields

    private readonly ConcurrentDictionary<int, LapEntity> _unfinishedLaps;
    private readonly ConcurrentDictionary<int, TelemetryRuntimeData> _telemetryPoints;
    private readonly SessionRuntimeData _sessionRuntimeData;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="sessionRuntimeData">Session runtime data</param>
    public ParticipantRuntimeData(SessionRuntimeData sessionRuntimeData)
    {
        _unfinishedLaps = new ConcurrentDictionary<int, LapEntity>();
        _telemetryPoints = new ConcurrentDictionary<int, TelemetryRuntimeData>();
        _sessionRuntimeData = sessionRuntimeData;
    }

    #endregion // Constructors

    #region Properties

    /// <summary>
    /// Is this object valid?
    /// </summary>
    public bool IsValidObject { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether the driver is a human
    /// </summary>
    public bool IsHumanDriver { get; set; } = false;

    /// <summary>
    /// Database id of current participant
    /// </summary>
    public long ParticipantDbId { get; set; }

    /// <summary>
    /// Cached live driver data of this participant in the current live session
    /// </summary>
    public LiveDriverData? LiveData { get; set; }

    /// <summary>
    /// Index in game packet
    /// </summary>
    public int ArrayIndex { get; set; }

    /// <summary>
    /// Car number
    /// </summary>
    public int CarNumber { get; set; }

    /// <summary>
    /// Name of team
    /// </summary>
    public string TeamName { get; set; }

    /// <summary>
    /// Database id of team
    /// </summary>
    public long TeamDbId { get; set; }

    /// <summary>
    /// Name of driver
    /// </summary>
    public string DriverName { get; set; }

    /// <summary>
    /// Database id of driver
    /// </summary>
    public long DriverDbId { get; set; }

    /// <summary>
    /// Nationality
    /// </summary>
    public string Nationality { get; set; }

    /// <summary>
    /// Current lap number
    /// </summary>
    public ushort CurrentLapNumber { get; set; }

    /// <summary>
    /// Last lap time
    /// </summary>
    public uint LastLapTime { get; set; }

    /// <summary>
    /// Lap number of last lap time
    /// </summary>
    public int LastLapTimeNumber { get; set; }

    /// <summary>
    /// Lap number from game packet is wrong, driver is one lap ahead
    /// </summary>
    public bool IsOnNextLap { get; set; }

    /// <summary>
    /// Current lap number from packet (could be lower than expexted!)
    /// </summary>
    public ushort CurrentPacketLapNumber { get; set; }

    /// <summary>
    /// Current driver status
    /// </summary>
    public DriverStatus CurrentStatus { get; set; }

    /// <summary>
    /// Last lap sector
    /// </summary>
    public Sector LastLapSector { get; set; }

    /// <summary>
    /// Last known lap distance
    /// </summary>
    public float LastLapDistance { get; set; }

    /// <summary>
    /// Last lap frame identifiert sent on
    /// </summary>
    public uint LastLapFrameIdentifier { get; set; }

    /// <summary>
    /// Last lap session timestamp
    /// </summary>
    public uint LastLapSessionTimestamp { get; set; }

    /// <summary>
    /// Is new telemetry start point?
    /// </summary>
    public bool IsNewTelemetry { get; set; }

    /// <summary>
    /// Car is on track?
    /// </summary>
    public bool CarIsOnTrack
    {
        get;
        set
        {
            if (field != value)
            {
                CarWasOnTrack = field;
            }

            field = value;
        }
    }

    /// <summary>
    /// Car was before on track?
    /// </summary>
    public bool CarWasOnTrack { get; private set; }

    /// <summary>
    /// Car is on a recordable lap
    /// </summary>
    public bool IsOnRecordableLap
    {
        get;
        set
        {
            if (field && value == false)
            {
                WasOnRecordableLap = true;
            }

            field = value;
        }
    }

    /// <summary>
    /// Car was on recordable lap
    /// </summary>
    public bool WasOnRecordableLap { get; private set; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Adds a new lap
    /// </summary>
    /// <param name="lapData">Data of lap</param>
    /// <returns>Status</returns>
    public bool AddLap(LapEntity lapData)
    {
        var lapAdded = false;

        if (lapData != null && _unfinishedLaps.ContainsKey(lapData.LapNumber) == false)
        {
            lapAdded = _unfinishedLaps.TryAdd(lapData.LapNumber, lapData);

            if (IsValidObject)
            {
                using (var dbFactory = RepositoryFactory.CreateInstance())
                {
                    dbFactory.GetRepository<LapRepository>()?.AddOrRefresh(l => l.ParticipantId == ParticipantDbId && l.LapNumber == lapData.LapNumber,
                                                                           obj =>
                                                                           {
                                                                               obj.LapNumber = lapData.LapNumber;
                                                                               obj.ParticipantId = ParticipantDbId;
                                                                               obj.SessionId = lapData.SessionId;
                                                                               obj.CarPosition = 0;
                                                                               obj.PitStatus = PitStatus.Unknown;
                                                                               obj.DriverStatus = DriverStatus.FlyingLap;
                                                                               obj.ResultStatus = ResultStatus.Active;
                                                                               obj.IsInvalid = false;
                                                                               obj.IsInvalidLapTime = true;
                                                                               obj.Sector1Time = 0;
                                                                               obj.Sector2Time = 0;
                                                                               obj.Sector3Time = 0;
                                                                               obj.LapTime = 0;
                                                                               obj.IsCompleted = false;
                                                                           });

                    var lapId = dbFactory.GetRepository<LapRepository>()?.GetQuery()?.FirstOrDefault(l => l.ParticipantId == ParticipantDbId && l.LapNumber == lapData.LapNumber)?.Id;

                    if (lapId.HasValue)
                    {
                        lapData.Id = lapId.Value;
                    }
                }
            }
        }

        return lapAdded;
    }

    /// <summary>
    /// Remove an uncompleted lap
    /// </summary>
    /// <param name="lapNumber">Number of lap</param>
    /// <returns>Lap removed?</returns>
    public bool RemoveLap(int lapNumber)
    {
        var isRemoved = false;

        if (lapNumber > 0 && _unfinishedLaps.TryRemove(lapNumber, out var lapEntity) && IsValidObject)
        {
            using (var dbFactory = RepositoryFactory.CreateInstance())
            {
                // Remove telemetry data
                if (ClearTelemetryData(lapNumber) && lapEntity?.Id > 0)
                {
                    dbFactory.GetRepository<CarTelemetryRepository>()?.ExecuteRawSql("DELETE FROM CarTelemetries WHERE LapNumberId = @p0", lapEntity.Id);
                }

                // Remove the lap
                if (dbFactory.GetRepository<LapRepository>()?.Remove(l => l.ParticipantId == ParticipantDbId && l.LapNumber == lapNumber) == true)
                {
                    isRemoved = true;
                }
            }
        }

        return isRemoved;
    }

    /// <summary>
    /// Mark lap as completed
    /// </summary>
    /// <param name="lapNumber">Number of lap</param>
    /// <param name="lapData">Data of lap</param>
    /// <returns>Marked as completed?</returns>
    public bool PreCompleteLap(int lapNumber, IndependentLapData lapData)
    {
        var isPreCompleted = false;

        if (IsValidObject && lapNumber > 0 && _unfinishedLaps.TryGetValue(lapNumber, out var lapEntity))
        {
            lapEntity.IsFinished = true;

            isPreCompleted = true;
        }

        return isPreCompleted;
    }

    /// <summary>
    /// Lap is completed
    /// </summary>
    /// <param name="lapNumber">Number of lap</param>
    /// <returns>Lap in db completed?</returns>
    public bool CompleteLap(int lapNumber)
    {
        var isFinished = false;

        if (IsValidObject && lapNumber > 0 && _unfinishedLaps.TryRemove(lapNumber, out var lapEntity))
        {
            var isInvalidLapTime = ValidateLapTimes(lapEntity.LapTime, lapEntity.Sector1Time, lapEntity.Sector2Time, lapEntity.Sector3Time);

            using (var dbFactory = RepositoryFactory.CreateInstance())
            {
                isFinished = dbFactory.GetRepository<LapRepository>()?.AddOrRefresh(l => l.ParticipantId == ParticipantDbId && l.LapNumber == lapNumber,
                                                                                    obj =>
                                                                                    {
                                                                                        obj.LapNumber = lapEntity.LapNumber;
                                                                                        obj.ParticipantId = ParticipantDbId;
                                                                                        obj.SessionId = lapEntity.SessionId;
                                                                                        obj.CarPosition = lapEntity.CarPosition;
                                                                                        obj.PitStatus = lapEntity.PitStatus;
                                                                                        obj.DriverStatus = lapEntity.DriverStatus;
                                                                                        obj.ResultStatus = lapEntity.ResultStatus;
                                                                                        obj.IsInvalid = lapEntity.IsInvalid;
                                                                                        obj.IsInvalidLapTime = isInvalidLapTime;
                                                                                        obj.Sector1Time = lapEntity.Sector1Time;
                                                                                        obj.Sector2Time = lapEntity.Sector2Time;
                                                                                        obj.Sector3Time = lapEntity.Sector3Time;
                                                                                        obj.LapTime = lapEntity.LapTime;
                                                                                        obj.IsCompleted = lapEntity.IsCompleted;
                                                                                    }) ?? false;
            }
        }

        return isFinished;
    }

    /// <summary>
    /// Gets a lap
    /// </summary>
    /// <param name="lapNumber">Number of lap</param>
    /// <returns>Lap entity</returns>
    public LapEntity? GetLap(int lapNumber)
    {
        _unfinishedLaps.TryGetValue(lapNumber, out var lap);

        return lap;
    }

    /// <summary>
    /// Get current lap
    /// </summary>
    /// <returns>Lap entity</returns>
    public LapEntity? GetCurrentLap()
    {
        _unfinishedLaps.TryGetValue(CurrentLapNumber, out var lap);

        return lap;
    }

    /// <summary>
    /// Add telemetry data
    /// </summary>
    /// <param name="lapNumber">Lap number</param>
    /// <param name="telemetryEntity">Telemetry entity</param>
    public void AddTelemetryData(int lapNumber, CarTelemetryEntity telemetryEntity)
    {
        if (lapNumber > 0)
        {
            if (_telemetryPoints.ContainsKey(lapNumber) == false)
            {
                _telemetryPoints.TryAdd(lapNumber, new TelemetryRuntimeData());
            }

            if (_telemetryPoints.TryGetValue(lapNumber, out var telemetryData))
            {
                telemetryData.TelemetryQueue.Enqueue(telemetryEntity);
            }
        }
    }

    /// <summary>
    /// Complete telemetry data of given lap number
    /// </summary>
    /// <param name="lapNumber">Lap number</param>
    /// <returns>Task</returns>
    public async Task CompleteTelemetryData(int lapNumber)
    {
        if (_telemetryPoints.TryGetValue(lapNumber, out var telemetryData))
        {
            telemetryData.IsCompleted = true;

            if (IsValidObject)
            {
                using (var dbFactory = RepositoryFactory.CreateInstance())
                {
                    var lapEntity = dbFactory.GetRepository<LapRepository>()
                                             ?.GetQuery()
                                             ?.FirstOrDefault(l => l.ParticipantId == ParticipantDbId && l.LapNumber == lapNumber);

                    if (lapEntity != null)
                    {
                        var telemetryEntities = telemetryData.TelemetryQueue.ToList();

                        foreach (var telemetryEntity in telemetryEntities)
                        {
                            telemetryEntity.LapNumberId = lapEntity.Id;
                        }

                        _telemetryPoints.TryRemove(lapNumber, out _);

                        await (dbFactory.GetRepository<CarTelemetryRepository>()?.UpdateRangeAsync(telemetryEntities) ?? Task.FromResult(false)).ConfigureAwait(false);
                    }
                }
            }
        }

        IsNewTelemetry = false;
    }

    /// <summary>
    /// Clear telemetry data of given lap number
    /// </summary>
    /// <param name="lapNumber">Number of lap</param>
    /// <returns>Lap exists?</returns>
    public bool ClearTelemetryData(int lapNumber)
    {
        var lapExists = false;

        if (_telemetryPoints.TryRemove(lapNumber, out var telemetryData))
        {
            telemetryData.TelemetryQueue.Clear();

            lapExists = true;
        }

        return lapExists;
    }

    /// <summary>
    /// Check current time against reference time of current track
    /// </summary>
    /// <param name="lapTime">Total lap time</param>
    /// <param name="sector1Time">Time of sector 1</param>
    /// <param name="sector2Time">Time of sector 1</param>
    /// <param name="sector3Time">Time of sector 1</param>
    /// <returns>Is valid lap time?</returns>
    public bool ValidateLapTimes(uint lapTime, uint sector1Time, uint sector2Time, uint sector3Time)
    {
        var isInvalidLapTime = true;

        if ((lapTime > _sessionRuntimeData.CurrentTrackReferenceLapTime
             && sector1Time > _sessionRuntimeData.CurrentTrackReferenceSector1Time
             && sector2Time > _sessionRuntimeData.CurrentTrackReferenceSector2Time
             && sector3Time > _sessionRuntimeData.CurrentTrackReferenceSector3Time)
            || (lapTime > _sessionRuntimeData.CurrentTrackReferenceLapTime * 0.90
                && sector1Time > _sessionRuntimeData.CurrentTrackReferenceSector1Time * 0.90
                && sector2Time > _sessionRuntimeData.CurrentTrackReferenceSector2Time * 0.90
                && sector3Time > _sessionRuntimeData.CurrentTrackReferenceSector3Time * 0.90))
        {
            isInvalidLapTime = false;
        }

        return isInvalidLapTime;
    }

    #endregion // Methods

    #region IDisposable

    /// <summary>
    /// Internal dispose method
    /// </summary>
    /// <param name="disposing">Dispose flag</param>
    protected virtual void Dispose(bool disposing)
    {
        _unfinishedLaps.Clear();
        _telemetryPoints.Clear();
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    #endregion // IDisposable
}