using System.Collections.Concurrent;

using F1Server.Core.Enumerations;
using F1Server.Core.Interfaces;
using F1Server.Data;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;

namespace F1Server.Service.Runtime;

/// <summary>
/// Data class with information of current running session
/// </summary>
public class SessionRuntimeData : ISessionRuntimeData
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="gameVersion">Current game version</param>
    /// <param name="sessionId">Current game session id</param>
    /// <param name="sessionType">Type of current session</param>
    public SessionRuntimeData(int gameVersion, ulong sessionId, SessionType sessionType)
    {
        Participants = new ConcurrentDictionary<ushort, ParticipantRuntimeData>();
        GameVersion = gameVersion;
        CurrentSessionId = sessionId;
        CurrentSessionType = sessionType;
    }

    #endregion // Constructors

    #region Properties

    /// <summary>
    /// Session information
    /// </summary>
    public LiveSessionData CurrentSession { get; set; }

    /// <summary>
    /// Number of players (human or ai controlled) in this session
    /// </summary>
    public ushort Players
    {
        get;
        set
        {
            // When players retired from session, value will be lower
            if (value > field)
            {
                field = value;

                UpdateSessionPlayers();
            }
        }
    }

    /// <summary>
    /// Sollen die Daten für diese Session aufgezeichnet werden?
    /// </summary>
    public bool IsRecordable { get; set; }

    /// <summary>
    /// Flag to remove session from database
    /// </summary>
    public bool IsInvalidSession { get; set; }

    /// <summary>
    /// Dictionary of participants
    /// </summary>
    public ConcurrentDictionary<ushort, ParticipantRuntimeData> Participants { get; }

    /// <summary>
    /// Valid session?
    /// </summary>
    public bool IsValid => CurrentSession?.DbId > 0 && IsRecordable && CurrentSession.IsFinished == false;

    /// <summary>
    /// Session is finished
    /// </summary>
    public bool IsFinished => CurrentSession?.IsFinished == true;

    /// <summary>
    /// Flag if the final classification packet was received
    /// </summary>
    public bool FinalClassificationReceived { get; internal set; }

    /// <summary>
    /// Number of flashbacks
    /// </summary>
    public int FlashbacksUsed
    {
        get;
        set
        {
            field = value;

            UpdateFlashbacksUsed();
        }
    }

    /// <summary>
    /// Number of virtual safety car phases
    /// </summary>
    public uint VirtualSafetyCarStages { get; private set; }

    /// <summary>
    /// Number of safety car phases
    /// </summary>
    public uint SafetyCarStages { get; private set; }

    /// <summary>
    /// Number of red flags
    /// </summary>
    public uint RedFlags { get; private set; }

    /// <summary>
    /// Last flashback frame identifier
    /// </summary>
    public uint LastFlashbackFrame { get; set; }

    /// <summary>
    /// Total laps from session packet
    /// </summary>
    public ushort TotalLaps { get; set; }

    /// <summary>
    /// Current track reference lap time
    /// </summary>
    public uint CurrentTrackReferenceLapTime { get; set; }

    /// <summary>
    /// Current track sector 1 reference time
    /// </summary>
    public uint CurrentTrackReferenceSector1Time { get; set; }

    /// <summary>
    /// Current track sector 2 reference time
    /// </summary>
    public uint CurrentTrackReferenceSector2Time { get; set; }

    /// <summary>
    /// Current track sector 3 reference time
    /// </summary>
    public uint CurrentTrackReferenceSector3Time { get; set; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Finish the session
    /// </summary>
    public void FinishSession()
    {
        if (CurrentSession?.DbId > 0)
        {
            try
            {
                using (var dbFactory = RepositoryFactory.CreateInstance())
                {
                    if (dbFactory.GetRepository<SessionRepository>()?.Refresh(s => s.Id == CurrentSession.DbId, (obj) => obj.IsFinished = true) == true)
                    {
                        CurrentSession.IsFinished = true;
                    }
                }
            }
            catch
            {
                // Ignore exceptions in this step
            }
        }
    }

    /// <summary>
    /// Update number of virtual safety car phases
    /// </summary>
    /// <param name="virtualSafetyCarStages">Value of virtual safety car stages in session</param>
    /// <param name="dbSessionId">Current database id of session</param>
    /// <returns>Indicates if the number of virtual safety car stages has changed</returns>
    public bool UpdateVirtualSafetyCarStages(uint virtualSafetyCarStages, long dbSessionId)
    {
        var isChanged = false;

        if (virtualSafetyCarStages != VirtualSafetyCarStages)
        {
            VirtualSafetyCarStages = virtualSafetyCarStages;

            if (dbSessionId > 0)
            {
                using (var dbFactory = RepositoryFactory.CreateInstance())
                {
                    dbFactory.GetRepository<SessionAttributesRepository>()?.Refresh(s => s.SessionId == dbSessionId, obj => obj.VirtualSafetyCarStages = VirtualSafetyCarStages);

                    isChanged = true;
                }
            }
        }

        return isChanged;
    }

    /// <summary>
    /// Update number of safety car phases
    /// </summary>
    /// <param name="safetyCarStages">Value of safety car stages in session</param>
    /// <param name="dbSessionId">Current database id of session</param>
    /// <returns>Indicates if the number of safety car stages has changed</returns>
    public bool UpdateSafetyCarStages(uint safetyCarStages, long dbSessionId)
    {
        var isChanged = false;

        if (safetyCarStages != SafetyCarStages)
        {
            SafetyCarStages = safetyCarStages;

            if (dbSessionId > 0)
            {
                using (var dbFactory = RepositoryFactory.CreateInstance())
                {
                    dbFactory.GetRepository<SessionAttributesRepository>()?.Refresh(s => s.SessionId == dbSessionId, obj => obj.SafetyCarStages = SafetyCarStages);

                    isChanged = true;
                }
            }
        }

        return isChanged;
    }

    /// <summary>
    /// Update number of red flags
    /// </summary>
    /// <param name="redFlagStages">Value of red flags stages in session</param>
    /// <param name="dbSessionId">Current database id of session</param>
    /// <returns>Indicates if the number of red flags has changed</returns>
    public bool UpdateRedFlagStages(uint redFlagStages, long dbSessionId)
    {
        var isChanged = false;

        if (redFlagStages != RedFlags)
        {
            RedFlags = redFlagStages;

            if (dbSessionId > 0)
            {
                using (var dbFactory = RepositoryFactory.CreateInstance())
                {
                    dbFactory.GetRepository<SessionAttributesRepository>()?.Refresh(s => s.SessionId == dbSessionId, obj => obj.RedFlags = RedFlags);

                    isChanged = true;
                }
            }
        }

        return isChanged;
    }

    #endregion // Methods

    #region Private methods

    /// <summary>
    /// Update number of current players in this session
    /// </summary>
    private void UpdateSessionPlayers()
    {
        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            dbFactory.GetRepository<SessionRepository>()?.Refresh(s => s.Id == CurrentSession.DbId, obj => obj.ActiveCars = Players);
        }
    }

    /// <summary>
    /// Update number of used flashbacks
    /// </summary>
    private void UpdateFlashbacksUsed()
    {
        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            dbFactory.GetRepository<SessionAttributesRepository>()?.Refresh(s => s.SessionId == CurrentSession.DbId, obj => obj.FlashbacksUsed = FlashbacksUsed);
        }
    }

    #endregion // Private methods

    #region ISessionRuntimeData

    /// <inheritdoc/>
    public long SessionDbId { get; set; }

    /// <inheritdoc/>
    public ulong CurrentSessionId { get; }

    /// <inheritdoc/>
    public int GameVersion { get; }

    /// <inheritdoc/>
    public SessionType CurrentSessionType { get; }

    /// <inheritdoc/>
    public bool HasParticipants { get; set; }

    /// <inheritdoc/>
    public bool IsTelemetryRecording { get; set; }

    /// <inheritdoc/>
    public int AirTemperature => CurrentSession?.AirTemperature ?? 0;

    /// <inheritdoc/>
    public int TrackTemperature => CurrentSession?.TrackTemperature ?? 0;

    /// <inheritdoc/>
    public uint FastestSector1 => CurrentSession?.FastestSector1 ?? 0;

    /// <inheritdoc/>
    public uint FastestSector2 => CurrentSession?.FastestSector2 ?? 0;

    /// <inheritdoc/>
    public uint FastestSector3 => CurrentSession?.FastestSector3 ?? 0;

    /// <inheritdoc/>
    public uint FastestLap => CurrentSession?.FastestLap ?? 0;

    #endregion // ISessionRuntimeData
}