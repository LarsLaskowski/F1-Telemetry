using System.Diagnostics;

using F1Server.Core.Enumerations;
using F1Server.Core.Observability;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;
using F1Server.Data;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;
using F1Server.Service.Cache;
using F1Server.Service.Runtime;

using Microsoft.Extensions.Logging;

namespace F1Server.Service.Processors;

/// <summary>
/// Process session packet
/// </summary>
internal class SessionProcessor : BaseProcessor
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="serviceProvider">Service provider</param>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="gameInfo">Runtime game information</param>
    public SessionProcessor(IServiceProvider serviceProvider, PacketHeader packetHeader, LiveGameData gameInfo)
        : base(serviceProvider, packetHeader, gameInfo)
    {
        Logger?.LogInformation("SessionProcessor created.");
    }

    #endregion // Constructors

    #region Private methods

    /// <summary>
    /// Creates a new database session object
    /// </summary>
    /// <param name="sessionRuntimeData">Session runtime data</param>
    /// <param name="sessionData">Session data from received packet</param>
    /// <param name="dbFactory">Database factory</param>
    /// <param name="dbSessionData">Session entity</param>
    /// <param name="dbSessionAttributes">Attributes of session</param>
    /// <returns>Session created?</returns>
    private bool CreateNewSession(SessionRuntimeData sessionRuntimeData, ISessionDataBase sessionData, RepositoryFactory dbFactory, out SessionEntity? dbSessionData, out SessionAttributesEntity dbSessionAttributes)
    {
        var retValue = false;

        dbSessionData = new()
                        {
                            SessionId = PacketHeader.UniqueSessionId,
                            CreationTimestamp = DateTime.UtcNow,
                            FormulaType = sessionData.FormulaType,
                            IsNetworkGame = sessionData.IsNetworkGame,
                            SessionType = sessionData.SessionType,
                            GameVersionId = GameInfo.DbId
                        };

        // Set the correct track id
        var currentTrack = TrackRepositoryCache.GetByTrackNumber(sessionData.TrackId);

        if (currentTrack != null)
        {
            dbSessionData.TrackId = currentTrack.Id;

            sessionRuntimeData.CurrentTrackReferenceLapTime = currentTrack.LapReferenceTime;
            sessionRuntimeData.CurrentTrackReferenceSector1Time = currentTrack.Sector1ReferenceTime;
            sessionRuntimeData.CurrentTrackReferenceSector2Time = currentTrack.Sector2ReferenceTime;
            sessionRuntimeData.CurrentTrackReferenceSector3Time = currentTrack.Sector3ReferenceTime;
        }

        dbSessionAttributes = new SessionAttributesEntity
                              {
                                  WeatherStart = sessionData.Weather,
                                  WeatherEnd = sessionData.Weather
                              };

        ExtractSessionData(sessionData, dbSessionData, dbSessionAttributes);

        if (dbFactory.GetRepository<SessionRepository>()?.Add(dbSessionData) == true)
        {
            sessionRuntimeData.SessionDbId = dbSessionData.Id;
            sessionRuntimeData.TotalLaps = sessionData.TotalLaps;

            SessionRepositoryCache.AddOrUpdate(dbSessionData);

            retValue = true;
        }

        return retValue;
    }

    /// <summary>
    /// Extract session data
    /// </summary>
    /// <param name="sessionData">Session data</param>
    /// <param name="dbSessionData">Session entity</param>
    /// <param name="dbSessionAttributes">Attributes of session</param>
    private void ExtractSessionData(ISessionDataBase sessionData, SessionEntity dbSessionData, SessionAttributesEntity dbSessionAttributes)
    {
        if (sessionData is ISessionData2021 session21Data)
        {
            dbSessionData.AiDifficulty = session21Data.AiDifficulty;

            dbSessionAttributes.IsExtendedSession = true;
            dbSessionAttributes.GearBoxAssistFirst = session21Data.GearboxAssist;
            dbSessionAttributes.SteeringAssistFirst = session21Data.IsSteeringAssist;
            dbSessionAttributes.BrakingAssistFirst = session21Data.BrakingAssist;
        }

        if (sessionData is ISessionData2022 session22Data)
        {
            dbSessionData.SessionLength = session22Data.SessionLength;
            dbSessionData.AiDifficulty = session22Data.AiDifficulty;

            dbSessionAttributes.IsExtendedSession = true;
            dbSessionAttributes.GearBoxAssistFirst = session22Data.GearboxAssist;
            dbSessionAttributes.SteeringAssistFirst = session22Data.IsSteeringAssist;
            dbSessionAttributes.BrakingAssistFirst = session22Data.BrakingAssist;
            dbSessionAttributes.GameMode = session22Data.GameMode;
            dbSessionAttributes.RuleSet = session22Data.RuleSet;
        }

        if (sessionData is ISessionData2023 session23Data)
        {
            dbSessionData.AiDifficulty = session23Data.AiDifficulty;
            dbSessionData.SessionLength = session23Data.SessionLength;

            dbSessionAttributes.IsExtendedSession = true;
            dbSessionAttributes.GearBoxAssistFirst = session23Data.GearboxAssist;
            dbSessionAttributes.SteeringAssistFirst = session23Data.IsSteeringAssist;
            dbSessionAttributes.BrakingAssistFirst = session23Data.BrakingAssist;
            dbSessionAttributes.GameMode = session23Data.GameMode;
            dbSessionAttributes.RuleSet = session23Data.RuleSet;
            dbSessionAttributes.VirtualSafetyCarStages = session23Data.VirtualSafetyCarPeriods;
            dbSessionAttributes.SafetyCarStages = session23Data.SafetyCarPeriods;
            dbSessionAttributes.RedFlags = session23Data.RedFlags;
        }
    }

    /// <summary>
    /// Update the session
    /// </summary>
    /// <param name="sessionRuntimeData">Session runtime data</param>
    /// <param name="sessionData">Session data from received packet</param>
    /// <param name="dbFactory">Database factory</param>
    /// <param name="dbSessionData">Session entity</param>
    private void UpdateSession(SessionRuntimeData sessionRuntimeData, ISessionDataBase sessionData, RepositoryFactory dbFactory, SessionEntity dbSessionData)
    {
        var isChangedAttr = false;

        if (sessionRuntimeData.SessionDbId == 0)
        {
            sessionRuntimeData.SessionDbId = dbSessionData.Id;

            ClearPreviousSessionData(dbSessionData.Id, dbFactory);
        }

        if (sessionRuntimeData.CurrentTrackReferenceLapTime == 0)
        {
            var currentTrack = TrackRepositoryCache.GetByTrackNumber(sessionData.TrackId);

            if (currentTrack != null)
            {
                sessionRuntimeData.CurrentTrackReferenceLapTime = currentTrack.LapReferenceTime;
                sessionRuntimeData.CurrentTrackReferenceSector1Time = currentTrack.Sector1ReferenceTime;
                sessionRuntimeData.CurrentTrackReferenceSector2Time = currentTrack.Sector2ReferenceTime;
                sessionRuntimeData.CurrentTrackReferenceSector3Time = currentTrack.Sector3ReferenceTime;
            }
        }

        if (dbSessionData.IsNetworkGame == true && sessionData.IsNetworkGame == false)
        {
            dbSessionData.IsNetworkGame = false;

            dbFactory.GetRepository<SessionRepository>()?.Refresh(s => s.Id == dbSessionData.Id,
                                                                  obj => obj.DbIsNetworkGame = dbSessionData.DbIsNetworkGame);

            SessionRepositoryCache.AddOrUpdate(dbSessionData);
        }

        UpdateOpponentStrength(sessionData, dbFactory, dbSessionData);

        var dbSessionAttributes = SessionRepositoryCache.GetAttributesBySessionId(dbSessionData.Id);

        if (dbSessionAttributes != null)
        {
            // check any assistant relevant changes
            var isAssistantSettingsChanged = CheckAssistantChanges(dbSessionAttributes, sessionData);

            isChangedAttr = isAssistantSettingsChanged;

            if (sessionData.Weather != dbSessionAttributes.WeatherStart)
            {
                dbSessionAttributes.WeatherEnd = sessionData.Weather;

                isChangedAttr = true;
            }
        }

        if (isChangedAttr && dbSessionAttributes != null)
        {
            dbFactory.GetRepository<SessionAttributesRepository>()?.Refresh(s => s.SessionId == dbSessionData.Id,
                                                                            obj =>
                                                                            {
                                                                                obj.BrakingAssistChanged = dbSessionAttributes.BrakingAssistChanged;
                                                                                obj.BrakingAssistLast = dbSessionAttributes.BrakingAssistLast;
                                                                                obj.GearBoxAssistChanged = dbSessionAttributes.GearBoxAssistChanged;
                                                                                obj.GearBoxAssistLast = dbSessionAttributes.GearBoxAssistLast;
                                                                                obj.SteeringAssistChanged = dbSessionAttributes.SteeringAssistChanged;
                                                                                obj.SteeringAssistLast = dbSessionAttributes.SteeringAssistLast;
                                                                                obj.WeatherEnd = dbSessionAttributes.WeatherEnd;
                                                                            });
        }

        if (CheckSessionAttributes(sessionRuntimeData, sessionData, dbSessionData.Id)
            || isChangedAttr)
        {
            SessionRepositoryCache.AddOrUpdateAttributes(dbSessionAttributes);
        }

        sessionRuntimeData.TotalLaps = sessionData.TotalLaps;
    }

    /// <summary>
    /// Update AI strength of opponents
    /// </summary>
    /// <param name="sessionData">Session data</param>
    /// <param name="dbFactory">Database access object</param>
    /// <param name="dbSessionData">Session entity</param>
    private void UpdateOpponentStrength(ISessionDataBase sessionData, RepositoryFactory dbFactory, SessionEntity dbSessionData)
    {
        if (dbSessionData.AiDifficulty == 0 && GameInfo.GameVersion >= 2021)
        {
            ushort aiDiff = 0;

            if (sessionData is ISessionData2021 sessionData2021)
            {
                aiDiff = sessionData2021.AiDifficulty;
            }

            if (sessionData is ISessionData2022 sessionData2022)
            {
                aiDiff = sessionData2022.AiDifficulty;
            }

            if (sessionData is ISessionData2023 sessionData2023)
            {
                aiDiff = sessionData2023.AiDifficulty;
            }

            if (aiDiff > 0)
            {
                dbFactory.GetRepository<SessionRepository>()?.Refresh(s => s.Id == dbSessionData.Id,
                                                                      obj => obj.AiDifficulty = aiDiff);

                SessionRepositoryCache.AddOrUpdate(dbSessionData);
            }
        }
    }

    /// <summary>
    /// Refresh live session data
    /// </summary>
    /// <param name="sessionRuntimeData">Session runtime data</param>
    /// <param name="sessionData">Session data</param>
    /// <param name="dbSessionData">Session enity</param>
    private void RefreshLiveSessionData(SessionRuntimeData sessionRuntimeData, SessionData sessionData, SessionEntity? dbSessionData)
    {
        if (sessionRuntimeData.CurrentSession == null && dbSessionData != null && sessionData.PacketData?.IsRecordable == true)
        {
            sessionRuntimeData.CurrentSession = new LiveSessionData
                                                {
                                                    DbId = dbSessionData.Id,
                                                    SessionGameId = dbSessionData.SessionId,
                                                    SessionType = dbSessionData.SessionType
                                                };

            sessionRuntimeData.IsRecordable = true;

            CheckTelemetryRecording(sessionRuntimeData, sessionData);
        }

        if (sessionRuntimeData.CurrentSession != null && sessionData.PacketData != null)
        {
            sessionRuntimeData.CurrentSession.SessionDuration = sessionData.PacketData.SessionDuration;
            sessionRuntimeData.CurrentSession.SessionTimeLeft = sessionData.PacketData.SessionTimeLeft;
            sessionRuntimeData.CurrentSession.AirTemperature = sessionData.PacketData.AirTemperature;
            sessionRuntimeData.CurrentSession.TrackTemperature = sessionData.PacketData.TrackTemperature;
            sessionRuntimeData.CurrentSession.Weather = sessionData.PacketData.Weather;

            sessionRuntimeData.CurrentSession.IsSafetyCar = sessionData.PacketData.SafetyCar == SafetyCarStatus.SafetyCar;
        }
    }

    /// <summary>
    /// Check further session attributes
    /// </summary>
    /// <param name="sessionRuntimeData">Session runtime data</param>
    /// <param name="sessionData">Session data</param>
    /// <param name="dbSessionId">Database id of session</param>
    /// <returns>Is something changed?</returns>
    private bool CheckSessionAttributes(SessionRuntimeData sessionRuntimeData, ISessionDataBase sessionData, long dbSessionId)
    {
        var hasChanges = false;

        if (sessionData is ISessionData2023 session23Data
            && (sessionRuntimeData.UpdateVirtualSafetyCarStages(session23Data.VirtualSafetyCarPeriods, dbSessionId)
                || sessionRuntimeData.UpdateSafetyCarStages(session23Data.SafetyCarPeriods, dbSessionId)
                || sessionRuntimeData.UpdateRedFlagStages(session23Data.RedFlags, dbSessionId)))
        {
            hasChanges = true;
        }

        return hasChanges;
    }

    /// <summary>
    /// Check assistant setting changes
    /// </summary>
    /// <param name="dbSessionAttributes">Database session attributes object</param>
    /// <param name="sessionData">Session data</param>
    /// <returns>Is something changed?</returns>
    private bool CheckAssistantChanges(SessionAttributesEntity dbSessionAttributes, ISessionDataBase sessionData)
    {
        var isChanged = false;

        if (dbSessionAttributes.IsExtendedSession == true)
        {
            if (sessionData is ISessionData2021 session21Data)
            {
                isChanged = CheckAssistantChanges2021(dbSessionAttributes, session21Data);
            }

            if (sessionData is ISessionData2022 session22Data)
            {
                isChanged = CheckAssistantChanges2022(dbSessionAttributes, session22Data);
            }

            if (sessionData is ISessionData2023 session23Data)
            {
                isChanged = CheckAssistantChanges2023(dbSessionAttributes, session23Data);
            }
        }

        return isChanged;
    }

    /// <summary>
    /// Check assistant setting changes in F1 2021
    /// </summary>
    /// <param name="dbSessionAttributesData">Database session attributes object</param>
    /// <param name="sessionData">Session data</param>
    /// <returns>Is something changed?</returns>
    private bool CheckAssistantChanges2021(SessionAttributesEntity dbSessionAttributesData, ISessionDataBase sessionData)
    {
        var isChanged = false;

        if (dbSessionAttributesData.IsExtendedSession == true && sessionData is ISessionData2021 session21Data)
        {
            if (dbSessionAttributesData.GearBoxAssistFirst != session21Data.GearboxAssist)
            {
                dbSessionAttributesData.GearBoxAssistLast = session21Data.GearboxAssist;
                dbSessionAttributesData.GearBoxAssistChanged = true;
                isChanged = true;
            }

            if (dbSessionAttributesData.BrakingAssistFirst != session21Data.BrakingAssist)
            {
                dbSessionAttributesData.BrakingAssistLast = session21Data.BrakingAssist;
                dbSessionAttributesData.BrakingAssistChanged = true;
                isChanged = true;
            }

            if (dbSessionAttributesData.SteeringAssistFirst != session21Data.IsSteeringAssist)
            {
                dbSessionAttributesData.SteeringAssistLast = session21Data.IsSteeringAssist;
                dbSessionAttributesData.SteeringAssistChanged = true;
                isChanged = true;
            }
        }

        return isChanged;
    }

    /// <summary>
    /// Check assistant setting changes in F1 2022
    /// </summary>
    /// <param name="dbSessionAttributesData">Database session object</param>
    /// <param name="sessionData">Session data</param>
    /// <returns>Is something changed?</returns>
    private bool CheckAssistantChanges2022(SessionAttributesEntity dbSessionAttributesData, ISessionData2022 sessionData)
    {
        var isChanged = false;

        if (dbSessionAttributesData.GearBoxAssistFirst != sessionData.GearboxAssist)
        {
            dbSessionAttributesData.GearBoxAssistLast = sessionData.GearboxAssist;
            dbSessionAttributesData.GearBoxAssistChanged = true;
            isChanged = true;
        }

        if (dbSessionAttributesData.BrakingAssistFirst != sessionData.BrakingAssist)
        {
            dbSessionAttributesData.BrakingAssistLast = sessionData.BrakingAssist;
            dbSessionAttributesData.BrakingAssistChanged = true;
            isChanged = true;
        }

        if (dbSessionAttributesData.SteeringAssistFirst != sessionData.IsSteeringAssist)
        {
            dbSessionAttributesData.SteeringAssistLast = sessionData.IsSteeringAssist;
            dbSessionAttributesData.SteeringAssistChanged = true;
            isChanged = true;
        }

        return isChanged;
    }

    /// <summary>
    /// Check assistant setting changes in F1 2023
    /// </summary>
    /// <param name="dbSessionAttributesData">Database session object</param>
    /// <param name="sessionData">Session data</param>
    /// <returns>Is something changed?</returns>
    private bool CheckAssistantChanges2023(SessionAttributesEntity dbSessionAttributesData, ISessionData2023 sessionData)
    {
        var isChanged = false;

        if (dbSessionAttributesData.GearBoxAssistFirst != sessionData.GearboxAssist)
        {
            dbSessionAttributesData.GearBoxAssistLast = sessionData.GearboxAssist;
            dbSessionAttributesData.GearBoxAssistChanged = true;
            isChanged = true;
        }

        if (dbSessionAttributesData.BrakingAssistFirst != sessionData.BrakingAssist)
        {
            dbSessionAttributesData.BrakingAssistLast = sessionData.BrakingAssist;
            dbSessionAttributesData.BrakingAssistChanged = true;
            isChanged = true;
        }

        if (dbSessionAttributesData.SteeringAssistFirst != sessionData.IsSteeringAssist)
        {
            dbSessionAttributesData.SteeringAssistLast = sessionData.IsSteeringAssist;
            dbSessionAttributesData.SteeringAssistChanged = true;
            isChanged = true;
        }

        return isChanged;
    }

    /// <summary>
    /// Deletes previous session relevant data, like laps, car telemetry
    /// </summary>
    /// <param name="sessionId">Database id of session</param>
    /// <param name="dbFactory">Database factory</param>
    private void ClearPreviousSessionData(long sessionId, RepositoryFactory dbFactory)
    {
        // All pending telemetry batches must be written before laps and telemetry of the reused session id are deleted
        DatabaseWriter.Flush();

        // Remove laps
        try
        {
            var lapIds = dbFactory.GetRepository<LapRepository>()
                                  ?.GetQuery()
                                  ?.Where(l => l.SessionId == sessionId)
                                  .Select(lap => lap.Id)
                                  .ToList() ?? [];

            if (lapIds.Count > 0)
            {
                foreach (var lapId in lapIds)
                {
                    // Remove car telemetry
                    try
                    {
                        dbFactory.GetRepository<CarTelemetryRepository>()?.ExecuteRawSql("DELETE FROM CarTelemetries WHERE LapNumberId = @p0", lapId);
                    }
                    catch (Exception ex)
                    {
                        Logger?.LogError(ex, "Error removing car telemetry for lap {LapId} in session {SessionId}.", lapId, sessionId);
                    }
                }

                dbFactory.GetRepository<LapRepository>()?.RemoveRange(l => lapIds.Contains(l.Id));
            }
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error clearing previous session data for session {SessionId}.", sessionId);
        }
    }

    /// <summary>
    /// Check whether telemetry data should be recorded
    /// </summary>
    /// <param name="sessionRuntimeData">Session runtime data</param>
    /// <param name="sessionData">Session data</param>
    private void CheckTelemetryRecording(SessionRuntimeData sessionRuntimeData, SessionData sessionData)
    {
        if (sessionData.PacketData != null)
        {
            sessionRuntimeData.IsTelemetryRecording = sessionData.PacketData.SessionType switch
                                                      {
                                                          SessionType.Qualifying3 or SessionType.ShortQualifying or SessionType.OneShotQualifying or SessionType.Race or SessionType.Race2 or SessionType.Race3 => true,
                                                          SessionType.SprintShootout1 or SessionType.SprintShootout2 or SessionType.SprintShootout3 or SessionType.ShortSprintShootout or SessionType.OneShotSprintShootout => true,
                                                          _ => false,
                                                      };
        }
    }

    #endregion // Private methods

    #region BaseProcessor

    /// <inheritdoc/>
    public override bool Process(object? dataObject, SessionRuntimeData? sessionRuntimeData)
    {
        var isProcessed = true;

        LastException = string.Empty;

        using var currentActivity = AppActivity.SrvSource.StartActivity("SessionProcessor");

        if (dataObject is SessionData sessionData
            && sessionData.PacketData != null
            && sessionRuntimeData != null)
        {
            try
            {
                using (var dbFactory = RepositoryFactory.CreateInstance())
                {
                    var dbSessionData = SessionRepositoryCache.GetByUniqueSessionId(PacketHeader.UniqueSessionId);

                    // Actual is no session set, it must be a new session
                    if (dbSessionData == null)
                    {
                        isProcessed = CreateNewSession(sessionRuntimeData, sessionData.PacketData, dbFactory, out dbSessionData, out var dbSessionAttributes);

                        if (dbSessionAttributes != null)
                        {
                            dbSessionAttributes.SessionId = sessionRuntimeData.SessionDbId;

                            // Add entry for session attributes
                            dbFactory.GetRepository<SessionAttributesRepository>()?.Add(dbSessionAttributes);

                            SessionRepositoryCache.AddOrUpdateAttributes(dbSessionAttributes);
                        }
                    }
                    else
                    {
                        UpdateSession(sessionRuntimeData, sessionData.PacketData, dbFactory, dbSessionData);
                    }

                    currentActivity?.AddTag("f1.session_id", dbSessionData?.SessionId ?? 0);
                    currentActivity?.AddTag("f1.session_db_id", dbSessionData?.Id ?? 0);
                    currentActivity?.AddTag("f1.session_type", dbSessionData?.SessionType ?? 0);

                    RefreshLiveSessionData(sessionRuntimeData, sessionData, dbSessionData);
                }

                currentActivity?.SetStatus(ActivityStatusCode.Ok);
            }
            catch (Exception ex)
            {
                currentActivity?.SetStatus(ActivityStatusCode.Error, ex.ToString());
                currentActivity?.AddException(ex);

                LastException = ex.ToString();

                Logger?.LogError(ex, "Error processing Session packet!");

                isProcessed = false;
            }
        }
        else
        {
            currentActivity?.SetStatus(ActivityStatusCode.Error, "Unexpected data object or session not valid!");

            Logger?.LogWarning("Unexpected data object or session not valid (processor: {Processor})!", nameof(SessionProcessor));

            isProcessed = false;
        }

        return isProcessed;
    }

    #endregion // BaseProcessor
}