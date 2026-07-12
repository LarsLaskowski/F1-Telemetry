using System.Diagnostics;

using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;
using F1Server.Core.Utils;
using F1Server.Data;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;
using F1Server.Service.Cache;
using F1Server.Service.Runtime;

using Microsoft.Extensions.Logging;

namespace F1Server.Service.Processors;

/// <summary>
/// Processor for session history packets
/// </summary>
internal class SessionHistoryProcessor : BaseProcessor
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="serviceProvider">Service provider</param>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="gameInfo">Runtime game information</param>
    public SessionHistoryProcessor(IServiceProvider serviceProvider, PacketHeader packetHeader, LiveGameData gameInfo)
        : base(serviceProvider, packetHeader, gameInfo)
    {
        LapRepositoryCache.Clear();

        Logger?.LogInformation("SessionHistoryProcessor created.");
    }

    #endregion // Constructors

    #region Methods

    /// <summary>
    /// Update driven laps
    /// </summary>
    /// <param name="sessionHistoryData">Session history data</param>
    /// <param name="liveDriverData">Live driver data</param>
    /// <param name="participantRuntimeData">Participant runtime data</param>
    /// <param name="liveSessionData">Live session data</param>
    /// <param name="isFinalDataReceived">Final classification received?</param>
    /// <returns>Is processed?</returns>
    private bool UpdateLaps(ISessionHistoryDataBase? sessionHistoryData, LiveDriverData? liveDriverData, ParticipantRuntimeData participantRuntimeData, LiveSessionData liveSessionData, bool isFinalDataReceived)
    {
        var isProcessed = true;

        if (sessionHistoryData?.NumberOfLaps > 0)
        {
            // The context bundles the per-packet data and its factory is only created when a lap actually needs a database write
            var context = new SessionHistoryLapContext
                          {
                              ParticipantRuntimeData = participantRuntimeData,
                              LiveDriverData = liveDriverData,
                              LiveSessionData = liveSessionData,
                              SessionHistoryData = sessionHistoryData,
                              IsFinalDataReceived = isFinalDataReceived,
                              DbFactory = new Lazy<RepositoryFactory>(RepositoryFactory.CreateInstance)
                          };

            try
            {
                for (var lap = 0; lap < sessionHistoryData.NumberOfLaps; ++lap)
                {
                    var lapData = sessionHistoryData.LapHistory[lap];

                    try
                    {
                        UpdateSingleLap(lapData, (ushort)(lap + 1), context);
                    }
                    catch
                    {
                        isProcessed = false;
                    }
                }
            }
            finally
            {
                if (context.DbFactory.IsValueCreated)
                {
                    context.DbFactory.Value.Dispose();
                }
            }
        }

        ITyreStintHistoryDataBase? currentTyre = null;

        if (sessionHistoryData?.TyreStintHistory != null)
        {
            currentTyre = Array.Find(sessionHistoryData.TyreStintHistory, t => t != null && t.EndLap == 255);
        }

        UpdateCurrentTyres(liveDriverData, currentTyre);

        UpdateLapsDriven(liveDriverData, sessionHistoryData);

        return isProcessed;
    }

    /// <summary>
    /// Update a single lap
    /// </summary>
    /// <param name="lapData">Data of lap</param>
    /// <param name="lapNumber">Number of lap</param>
    /// <param name="context">Per-packet processing context</param>
    private void UpdateSingleLap(ILapHistoryDataBase lapData, ushort lapNumber, SessionHistoryLapContext context)
    {
        if (lapData.LapTime > 0 || lapData.Sector1Time > 0 || lapData.Sector2Time > 0 || lapData.Sector3Time > 0)
        {
            var participantRuntimeData = context.ParticipantRuntimeData;

            // valid lap data information
            var lapEntity = participantRuntimeData.GetLap(lapNumber);

            if (lapEntity != null)
            {
                lapEntity.LapTime = lapData.LapTime;
                lapEntity.Sector1Time = lapData.Sector1Time;
                lapEntity.Sector2Time = lapData.Sector2Time;
                lapEntity.Sector3Time = lapData.Sector3Time;

                if (IsLapCompleted(lapEntity))
                {
                    lapEntity.IsCompleted = true;
                    lapEntity.IsFinished = true;

                    participantRuntimeData.CompleteLap(lapNumber);
                    participantRuntimeData.CompleteTelemetryData(lapNumber);
                }
                else
                {
                    lapEntity.IsCompleted = false;

                    if (context.IsFinalDataReceived)
                    {
                        lapEntity.IsInvalid = true;
                        lapEntity.IsInvalidLapTime = true;
                        lapEntity.IsFinished = true;
                        lapEntity.IsCompleted = true;

                        participantRuntimeData.CompleteTelemetryData(lapNumber);
                    }
                }

                CheckFastestLapTimes(lapEntity, context.LiveSessionData, context.LiveDriverData);
            }
            else
            {
                // Lap is finished or unknown
                UpdateFinishedLap(lapData, lapNumber, context);
            }
        }
    }

    /// <summary>
    /// Is lap completed?
    /// </summary>
    /// <param name="lapEntity">Lap entity</param>
    /// <returns>Is completed?</returns>
    private bool IsLapCompleted(LapEntity lapEntity)
    {
        var isCompleted = false;

        if (lapEntity.LapTime > 0
            && lapEntity.Sector1Time > 0
            && lapEntity.Sector2Time > 0
            && lapEntity.Sector3Time > 0
            && lapEntity.LapTime >= lapEntity.Sector1Time + lapEntity.Sector2Time + lapEntity.Sector3Time)
        {
            isCompleted = true;
        }

        return isCompleted;
    }

    /// <summary>
    /// Check and if neccessary update a finished lap
    /// </summary>
    /// <param name="lapData">Lap data</param>
    /// <param name="lapNumber">Number of lap</param>
    /// <param name="context">Per-packet processing context</param>
    private void UpdateFinishedLap(ILapHistoryDataBase lapData, ushort lapNumber, SessionHistoryLapContext context)
    {
        var participantDbId = context.ParticipantRuntimeData.ParticipantDbId;
        var lapDbData = LapRepositoryCache.GetByLapNumberParticipant(lapNumber, participantDbId);

        if (lapDbData != null)
        {
            var isInvalidLapTime = context.ParticipantRuntimeData.ValidateLapTimes(lapDbData.LapTime, lapDbData.Sector1Time, lapDbData.Sector2Time, lapDbData.Sector3Time);

            if (lapDbData.LapTime != lapData.LapTime
                || lapDbData.Sector1Time != lapData.Sector1Time
                || lapDbData.Sector2Time != lapData.Sector2Time
                || lapDbData.Sector3Time != lapData.Sector3Time
                || lapDbData.IsInvalidLapTime != isInvalidLapTime)
            {
                context.DbFactory.Value.GetRepository<LapRepository>()?.Refresh(l => l.Id == lapDbData.Id,
                                                                                obj =>
                                                                                {
                                                                                    obj.LapTime = lapData.LapTime;
                                                                                    obj.Sector1Time = lapData.Sector1Time;
                                                                                    obj.Sector2Time = lapData.Sector2Time;
                                                                                    obj.Sector3Time = lapData.Sector3Time;
                                                                                    obj.IsInvalidLapTime = isInvalidLapTime;
                                                                                });

                // The cached entity must receive the new values as well, otherwise the comparison above stays true forever
                lapDbData.LapTime = lapData.LapTime;
                lapDbData.Sector1Time = lapData.Sector1Time;
                lapDbData.Sector2Time = lapData.Sector2Time;
                lapDbData.Sector3Time = lapData.Sector3Time;
                lapDbData.IsInvalidLapTime = isInvalidLapTime;

                LapRepositoryCache.AddOrUpdate(lapDbData);
            }
        }
        else
        {
            // Lap is unknown? Insert if valid
            lapDbData = CreateLap(lapData, lapNumber, context);
        }

        CheckFastestLapTimes(lapDbData, context.LiveSessionData, context.LiveDriverData);
    }

    /// <summary>
    /// Create a lap or update an already stored lap with the same participant and lap number
    /// </summary>
    /// <param name="lapData">Data of lap</param>
    /// <param name="lapNumber">Number of lap</param>
    /// <param name="context">Per-packet processing context</param>
    /// <returns>Lap entity</returns>
    private LapEntity? CreateLap(ILapHistoryDataBase lapData, ushort lapNumber, SessionHistoryLapContext context)
    {
        LapEntity? lapDbData = null;

        if (lapData.IsLapTimeCompleteValid)
        {
            var participantDbId = context.ParticipantRuntimeData.ParticipantDbId;
            var isInvalidLapTime = context.ParticipantRuntimeData.ValidateLapTimes(lapData.LapTime, lapData.Sector1Time, lapData.Sector2Time, lapData.Sector3Time);

            // The lap can already be stored without being cached, update it instead of inserting a duplicate row
            lapDbData = context.DbFactory.Value.GetRepository<LapRepository>()
                                               ?.GetQuery()
                                               ?.FirstOrDefault(l => l.ParticipantId == participantDbId && l.LapNumber == lapNumber);

            if (lapDbData != null)
            {
                RefreshStoredLap(lapDbData, lapData, isInvalidLapTime, context);
            }
            else
            {
                lapDbData = InsertLap(lapData, lapNumber, isInvalidLapTime, context);
            }
        }

        return lapDbData;
    }

    /// <summary>
    /// Refresh an already stored lap with the values of the current history packet
    /// </summary>
    /// <param name="lapDbData">Stored lap entity</param>
    /// <param name="lapData">Data of lap</param>
    /// <param name="isInvalidLapTime">Is the lap time invalid?</param>
    /// <param name="context">Per-packet processing context</param>
    private void RefreshStoredLap(LapEntity lapDbData, ILapHistoryDataBase lapData, bool isInvalidLapTime, SessionHistoryLapContext context)
    {
        var lapDbId = lapDbData.Id;

        lapDbData.LapTime = lapData.LapTime;
        lapDbData.Sector1Time = lapData.Sector1Time;
        lapDbData.Sector2Time = lapData.Sector2Time;
        lapDbData.Sector3Time = lapData.Sector3Time;
        lapDbData.IsCompleted = true;
        lapDbData.IsInvalidLapTime = isInvalidLapTime;

        context.DbFactory.Value.GetRepository<LapRepository>()?.Refresh(l => l.Id == lapDbId,
                                                                        obj =>
                                                                        {
                                                                            obj.LapTime = lapData.LapTime;
                                                                            obj.Sector1Time = lapData.Sector1Time;
                                                                            obj.Sector2Time = lapData.Sector2Time;
                                                                            obj.Sector3Time = lapData.Sector3Time;
                                                                            obj.IsCompleted = true;
                                                                            obj.IsInvalidLapTime = isInvalidLapTime;
                                                                        });

        LapRepositoryCache.AddOrUpdate(lapDbData);
    }

    /// <summary>
    /// Insert a new lap for the current history packet
    /// </summary>
    /// <param name="lapData">Data of lap</param>
    /// <param name="lapNumber">Number of lap</param>
    /// <param name="isInvalidLapTime">Is the lap time invalid?</param>
    /// <param name="context">Per-packet processing context</param>
    /// <returns>Inserted lap entity, or null when the insert failed</returns>
    private LapEntity? InsertLap(ILapHistoryDataBase lapData, ushort lapNumber, bool isInvalidLapTime, SessionHistoryLapContext context)
    {
        LapEntity? lapDbData = new LapEntity
                               {
                                   IsCompleted = true,
                                   Sector1Time = lapData.Sector1Time,
                                   Sector2Time = lapData.Sector2Time,
                                   Sector3Time = lapData.Sector3Time,
                                   LapNumber = lapNumber,
                                   LapTime = lapData.LapTime,
                                   IsFinished = true,
                                   DriverStatus = DriverStatus.OnTrack,
                                   PitStatus = PitStatus.None,
                                   ResultStatus = ResultStatus.Active,
                                   ParticipantId = context.ParticipantRuntimeData.ParticipantDbId,
                                   SessionId = context.LiveSessionData.DbId,
                                   IsInvalidLapTime = isInvalidLapTime
                               };

        ITyreStintHistoryDataBase? tyre = null;

        if (context.SessionHistoryData.TyreStintHistory != null)
        {
            tyre = Array.Find(context.SessionHistoryData.TyreStintHistory, t => t != null && (t.EndLap == 255 || t.EndLap > lapNumber));
        }

        if (tyre != null)
        {
            lapDbData.TyreCompound = TyreCompoundMapper.MapVisualTyreCompoundToEnum(tyre.TyreVisualCompound);
        }

        if (context.DbFactory.Value.GetRepository<LapRepository>()?.Add(lapDbData) == false)
        {
            lapDbData = null;
        }
        else
        {
            LapRepositoryCache.AddOrUpdate(lapDbData);
        }

        return lapDbData;
    }

    /// <summary>
    /// Update current tyre type
    /// </summary>
    /// <param name="liveDriverData">Live driver data</param>
    /// <param name="currentTyres">Current tyre</param>
    private void UpdateCurrentTyres(LiveDriverData? liveDriverData, ITyreStintHistoryDataBase? currentTyres)
    {
        if (liveDriverData != null && currentTyres != null)
        {
            liveDriverData.CurrentUsedTyre = TyreCompoundMapper.MapVisualTyreCompoundToEnum(currentTyres.TyreVisualCompound);
        }
    }

    /// <summary>
    /// Update driven laps for driver
    /// </summary>
    /// <param name="liveDriverData">Live driver data</param>
    /// <param name="sessionHistoryData">Session history data</param>
    private void UpdateLapsDriven(LiveDriverData? liveDriverData, ISessionHistoryDataBase? sessionHistoryData)
    {
        // Count only finished laps
        if (liveDriverData != null && sessionHistoryData != null)
        {
            liveDriverData.LapsDriven = sessionHistoryData.LapHistory.Count(h => h != null && h.LapTime > 0);
        }
    }

    /// <summary>
    /// Check fastest lap times
    /// </summary>
    /// <param name="lapData">Lap database object</param>
    /// <param name="liveSession">Live session data</param>
    /// <param name="liveDriverData">Live driver data</param>
    private void CheckFastestLapTimes(LapEntity? lapData, LiveSessionData liveSession, LiveDriverData? liveDriverData)
    {
        if (liveDriverData != null && lapData != null)
        {
            if (lapData.Sector1Time > 0)
            {
                IsFastestSectorOne(lapData, liveSession, liveDriverData);
            }

            if (lapData.Sector2Time > 0)
            {
                IsFastestSectorTwo(lapData, liveSession, liveDriverData);
            }

            if (lapData.Sector3Time > 0)
            {
                IsFastestSectorThree(lapData, liveSession, liveDriverData);
            }

            if (lapData.IsCompleted && lapData.LapTime > 0)
            {
                IsFastestLap(lapData, liveSession, liveDriverData);
            }
        }
    }

    /// <summary>
    /// Is fastest sector 1?
    /// </summary>
    /// <param name="lapData">Lap data</param>
    /// <param name="liveSession">Live session data</param>
    /// <param name="liveDriverData">Live driver data</param>
    private void IsFastestSectorOne(LapEntity lapData, LiveSessionData liveSession, LiveDriverData liveDriverData)
    {
        // Fastest sector 1 for me?
        if (liveDriverData.FastestSector1 == 0 || lapData.Sector1Time < liveDriverData.FastestSector1)
        {
            liveDriverData.FastestSector1 = lapData.Sector1Time;
        }

        // Fastest sector 1 in session?
        if (liveSession.FastestSector1 == 0 || lapData.Sector1Time < liveSession.FastestSector1)
        {
            liveSession.FastestSector1 = lapData.Sector1Time;
            liveSession.FastestSector1Driver = liveDriverData.ArrayIndex;
        }
    }

    /// <summary>
    /// Is fastest sector 2?
    /// </summary>
    /// <param name="lapData">Lap data</param>
    /// <param name="liveSession">Live session data</param>
    /// <param name="liveDriverData">Live driver data</param>
    private void IsFastestSectorTwo(LapEntity lapData, LiveSessionData liveSession, LiveDriverData liveDriverData)
    {
        // Fastest sector 2 for me?
        if (liveDriverData.FastestSector2 == 0 || lapData.Sector2Time < liveDriverData.FastestSector2)
        {
            liveDriverData.FastestSector2 = lapData.Sector2Time;
        }

        // Fastest sector 2 in session?
        if (liveSession.FastestSector2 == 0 || lapData.Sector2Time < liveSession.FastestSector2)
        {
            liveSession.FastestSector2 = lapData.Sector2Time;
            liveSession.FastestSector2Driver = liveDriverData.ArrayIndex;
        }
    }

    /// <summary>
    /// Is fastest sector 3?
    /// </summary>
    /// <param name="lapData">Lap data</param>
    /// <param name="liveSession">Live session data</param>
    /// <param name="liveDriverData">Live driver data</param>
    private void IsFastestSectorThree(LapEntity lapData, LiveSessionData liveSession, LiveDriverData liveDriverData)
    {
        // Fastest sector 3 for me?
        if (liveDriverData.FastestSector3 == 0 || lapData.Sector3Time < liveDriverData.FastestSector3)
        {
            liveDriverData.FastestSector3 = lapData.Sector3Time;
        }

        // Fastest sector 3 in session?
        if (liveSession.FastestSector3 == 0 || lapData.Sector3Time < liveSession.FastestSector3)
        {
            liveSession.FastestSector3 = lapData.Sector3Time;
            liveSession.FastestSector3Driver = liveDriverData.ArrayIndex;
        }
    }

    /// <summary>
    /// Is fastest lap?
    /// </summary>
    /// <param name="lapData">Lap data</param>
    /// <param name="liveSession">Live session data</param>
    /// <param name="liveDriverData">Live driver data</param>
    private void IsFastestLap(LapEntity lapData, LiveSessionData liveSession, LiveDriverData liveDriverData)
    {
        // Fastest lap time for me?
        if (liveDriverData.FastestLapTime == 0 || lapData.LapTime < liveDriverData.FastestLapTime)
        {
            liveDriverData.FastestLapTime = lapData.LapTime;
        }

        // Fastest lap time in session?
        if (liveSession.FastestLap == 0 || lapData.LapTime < liveSession.FastestLap)
        {
            liveSession.FastestLap = lapData.LapTime;
            liveSession.FastestLapDriver = liveDriverData.ArrayIndex;
        }

        liveSession.TimeTable = liveSession.Drivers.Where(d => d.FastestLapTime > 0)
                                                   .OrderBy(d => d.FastestLapTime)
                                                   .Select(d => d.ArrayIndex)
                                                   .ToList();
    }

    #endregion // Methods

    #region BaseProcessor

    /// <inheritdoc/>
    public override bool Process(object? dataObject, SessionRuntimeData? sessionRuntimeData)
    {
        var isProcessed = true;

        LastException = string.Empty;

        var processWatch = Stopwatch.StartNew();

        if (dataObject is SessionHistoryData sessionHistory && sessionHistory.PacketData != null && sessionRuntimeData != null)
        {
            var isFinalDataReceived = sessionRuntimeData.FinalClassificationReceived == true;

            try
            {
                if (sessionRuntimeData.HasParticipants
                    && sessionRuntimeData.Participants.TryGetValue(sessionHistory.PacketData.CarIndex, out var participantData))
                {
                    var liveData = participantData.LiveData;

                    if (UpdateLaps(sessionHistory.PacketData, liveData, participantData, sessionRuntimeData.CurrentSession, isFinalDataReceived) == false)
                    {
                        isProcessed = false;
                    }
                }
            }
            catch (Exception ex)
            {
                LastException = ex.ToString();

                Logger?.LogError(ex, "Error processing SessionHistory packet!");

                isProcessed = false;
            }
        }
        else
        {
            Logger?.LogWarning("Unexpected data object or session not valid (processor: {Processor})!", nameof(SessionHistoryProcessor));
        }

        processWatch.Stop();

        RecordSlowProcessingActivity(nameof(SessionHistoryProcessor), processWatch.Elapsed, isProcessed);

        return isProcessed;
    }

    #endregion // BaseProcessor
}