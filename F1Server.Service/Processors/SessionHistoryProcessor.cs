using System.Diagnostics;

using F1Server.Core.Enumerations;
using F1Server.Core.Observability;
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

    #region BaseProcessor

    /// <inheritdoc/>
    public override bool Process(object? dataObject, SessionRuntimeData? sessionRuntimeData)
    {
        var isProcessed = true;

        LastException = string.Empty;

        using var currentActivity = AppActivity.SrvSource.StartActivity("SessionHistoryProcessor");

        if (dataObject is SessionHistoryData sessionHistory && sessionHistory.PacketData != null && sessionRuntimeData != null)
        {
            var isFinalDataReceived = sessionRuntimeData.FinalClassificationReceived == true;

            try
            {
                if (sessionRuntimeData.HasParticipants
                    && sessionRuntimeData.Participants.TryGetValue(sessionHistory.PacketData.CarIndex, out var participantData))
                {
                    var liveData = sessionRuntimeData.CurrentSession.Drivers?.Find(p => p.DbId == participantData.ParticipantDbId) as LiveDriverData;

                    if (UpdateLaps(sessionHistory.PacketData, liveData, participantData, sessionRuntimeData.CurrentSession, isFinalDataReceived) == false)
                    {
                        isProcessed = false;
                    }
                }

                currentActivity?.SetStatus(ActivityStatusCode.Ok);
            }
            catch (Exception ex)
            {
                currentActivity?.SetStatus(ActivityStatusCode.Error, ex.ToString());
                currentActivity?.AddException(ex);

                LastException = ex.ToString();

                Logger?.LogError(ex, "Error processing SessionHistory packet!");

                isProcessed = false;
            }
        }
        else
        {
            currentActivity?.SetStatus(ActivityStatusCode.Error, "Unexpected data object or session not finished!");

            Logger?.LogWarning("Unexpected data object or session not valid (processor: {Processor})!", nameof(SessionHistoryProcessor));
        }

        return isProcessed;
    }

    #endregion // BaseProcessor

    #region Private methods

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
            for (var lap = 0; lap < sessionHistoryData.NumberOfLaps; ++lap)
            {
                var lapData = sessionHistoryData.LapHistory[lap];

                try
                {
                    UpdateSingleLap(lapData, (ushort)(lap + 1), participantRuntimeData, liveDriverData, liveSessionData, isFinalDataReceived, sessionHistoryData);
                }
                catch
                {
                    isProcessed = false;
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
    /// <param name="participantRuntimeData">Participant runtime data</param>
    /// <param name="liveDriverData">Live driver data</param>
    /// <param name="liveSessionData">Live session data</param>
    /// <param name="isFinalDataReceived">Final classification received?</param>
    /// <param name="sessionHistoryData">History data of session</param>
    private void UpdateSingleLap(ILapHistoryDataBase lapData, ushort lapNumber, ParticipantRuntimeData participantRuntimeData, LiveDriverData? liveDriverData, LiveSessionData liveSessionData, bool isFinalDataReceived, ISessionHistoryDataBase sessionHistoryData)
    {
        if (lapData.LapTime > 0 || lapData.Sector1Time > 0 || lapData.Sector2Time > 0 || lapData.Sector3Time > 0)
        {
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
                    participantRuntimeData.CompleteTelemetryData(lapNumber).ConfigureAwait(false);
                }
                else
                {
                    lapEntity.IsCompleted = false;

                    if (isFinalDataReceived)
                    {
                        lapEntity.IsInvalid = true;
                        lapEntity.IsInvalidLapTime = true;
                        lapEntity.IsFinished = true;
                        lapEntity.IsCompleted = true;

                        participantRuntimeData.CompleteTelemetryData(lapNumber).ConfigureAwait(false);
                    }
                }

                CheckFastestLapTimes(lapEntity, liveSessionData, liveDriverData);
            }
            else
            {
                // Lap is finished or unknown
                UpdateFinishedLap(lapData, lapNumber, participantRuntimeData.ParticipantDbId, liveDriverData, liveSessionData, sessionHistoryData, participantRuntimeData);
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
    /// <param name="participantDbId">Database id of participant</param>
    /// <param name="liveDriverData">Live driver data</param>
    /// <param name="liveSessionData">Live session data</param>
    /// <param name="sessionHistoryData">History data of session</param>
    /// <param name="participantRuntimeData">Participant runtime data</param>
    private void UpdateFinishedLap(ILapHistoryDataBase lapData, ushort lapNumber, long participantDbId, LiveDriverData? liveDriverData, LiveSessionData liveSessionData, ISessionHistoryDataBase sessionHistoryData, ParticipantRuntimeData? participantRuntimeData)
    {
        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var lapDbData = LapRepositoryCache.GetByLapNumberParticipant(lapNumber, participantDbId);

            if (lapDbData != null)
            {
                var isInvalidLapTime = participantRuntimeData?.ValidateLapTimes(lapDbData.LapTime, lapDbData.Sector1Time, lapDbData.Sector2Time, lapDbData.Sector3Time);

                if (lapDbData.LapTime != lapData.LapTime
                    || lapDbData.Sector1Time != lapData.Sector1Time
                    || lapDbData.Sector2Time != lapData.Sector2Time
                    || lapDbData.Sector3Time != lapData.Sector3Time
                    || lapDbData.IsInvalidLapTime != isInvalidLapTime)
                {
                    dbFactory.GetRepository<LapRepository>()?.Refresh(l => l.Id == lapDbData.Id,
                                                                      obj =>
                                                                      {
                                                                          obj.LapTime = lapData.LapTime;
                                                                          obj.Sector1Time = lapData.Sector1Time;
                                                                          obj.Sector2Time = lapData.Sector2Time;
                                                                          obj.Sector3Time = lapData.Sector3Time;
                                                                          obj.IsInvalidLapTime = isInvalidLapTime ?? false;
                                                                      });

                    LapRepositoryCache.AddOrUpdate(lapDbData);
                }
            }
            else
            {
                // Lap is unknown? Insert if valid
                lapDbData = CreateLap(dbFactory, lapData, lapNumber, participantDbId, sessionHistoryData, liveSessionData.DbId, participantRuntimeData);
            }

            CheckFastestLapTimes(lapDbData, liveSessionData, liveDriverData);
        }
    }

    /// <summary>
    /// Create a lap
    /// </summary>
    /// <param name="dbFactory">Database factory object</param>
    /// <param name="lapData">Data of lap</param>
    /// <param name="lapNumber">Number of lap</param>
    /// <param name="participantDbId">Participant database id</param>
    /// <param name="sessionHistoryData">Session history data</param>
    /// <param name="sessionDbId">Session database id</param>
    /// <param name="participantRuntimeData">Participant runtime data</param>
    /// <returns>Lap entity</returns>
    private LapEntity? CreateLap(RepositoryFactory dbFactory, ILapHistoryDataBase lapData, ushort lapNumber, long participantDbId, ISessionHistoryDataBase sessionHistoryData, long sessionDbId, ParticipantRuntimeData? participantRuntimeData)
    {
        LapEntity? lapDbData = null;

        if (lapData.IsLapTimeCompleteValid)
        {
            lapDbData = new LapEntity
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
                            ParticipantId = participantDbId,
                            SessionId = sessionDbId,
                            IsInvalidLapTime = participantRuntimeData?.ValidateLapTimes(lapData.LapTime, lapData.Sector1Time, lapData.Sector2Time, lapData.Sector3Time) ?? false
                        };

            ITyreStintHistoryDataBase? tyre = null;

            if (sessionHistoryData?.TyreStintHistory != null)
            {
                tyre = Array.Find(sessionHistoryData.TyreStintHistory, t => t != null && (t.EndLap == 255 || t.EndLap > lapNumber));
            }

            if (tyre != null)
            {
                lapDbData.TyreCompound = TyreCompoundMapper.MapVisualTyreCompoundToEnum(tyre.TyreVisualCompound);
            }

            if (dbFactory.GetRepository<LapRepository>()?.Add(lapDbData) == false)
            {
                lapDbData = null;
            }
            else
            {
                LapRepositoryCache.AddOrUpdate(lapDbData);
            }
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

    #endregion // Private methods
}