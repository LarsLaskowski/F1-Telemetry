using System.Diagnostics;

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
/// Process participants packet
/// </summary>
internal class ParticipantsProcessor : BaseProcessor
{
    #region Fields

    private bool _isCreated = true;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="serviceProvider">Service provider</param>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="gameInfo">Runtime game information</param>
    public ParticipantsProcessor(IServiceProvider serviceProvider, PacketHeader packetHeader, LiveGameData gameInfo)
        : base(serviceProvider, packetHeader, gameInfo)
    {
        Logger?.LogInformation("ParticipantsProcessor created.");
    }

    #endregion // Constructors

    #region BaseProcessor

    /// <inheritdoc/>
    public override bool Process(object? dataObject, SessionRuntimeData? sessionRuntimeData)
    {
        var isProcessed = true;

        LastException = string.Empty;

        using var currentActivity = AppActivity.SrvSource.StartActivity("ParticipantsProcessor");

        if (dataObject is Participants participantsData && participantsData.PacketData != null && sessionRuntimeData?.IsValid == true)
        {
            sessionRuntimeData.HasParticipants = true;
            sessionRuntimeData.Players = participantsData.PacketData.ActiveCars;

            try
            {
                isProcessed = ProcessParticipants(sessionRuntimeData, participantsData);

                currentActivity?.SetStatus(ActivityStatusCode.Ok);
            }
            catch (Exception ex)
            {
                currentActivity?.SetStatus(ActivityStatusCode.Error, ex.ToString());
                currentActivity?.AddException(ex);

                LastException = ex.ToString();

                Logger?.LogError(ex, "Error processing Participants packet!");

                isProcessed = false;
            }
        }
        else
        {
            currentActivity?.SetStatus(ActivityStatusCode.Error, "No participants data or session not valid!");

            Logger?.LogWarning("Unexpected data object or session not valid (processor: {Processor})!", nameof(ParticipantsProcessor));
        }

        return isProcessed;
    }

    #endregion // BaseProcessor

    #region Private methods

    /// <summary>
    /// Process participants data packet
    /// </summary>
    /// <param name="sessionRuntimeData">Session runtime data</param>
    /// <param name="participantsData">Participants data</param>
    /// <returns>Processed?</returns>
    private bool ProcessParticipants(SessionRuntimeData sessionRuntimeData, Participants participantsData)
    {
        var retValue = false;

        if (participantsData.PacketData != null)
        {
            using (var dbFactory = RepositoryFactory.CreateInstance())
            {
                for (var car = 0; car < participantsData.PacketData.ActiveCars; car++)
                {
                    var participant = participantsData.PacketData.Participants[car];

                    // Teams with ID 41 will be ignored - is an online game
                    if (participant != null && participant.TeamId != 41)
                    {
                        retValue = CreateOrUpdateParticipant(sessionRuntimeData, dbFactory, (ushort)car, participant);
                    }
                    else
                    {
                        if (CheckNetworkSession(sessionRuntimeData, dbFactory, participant))
                        {
                            break;
                        }
                    }
                }
            }
        }

        return retValue;
    }

    /// <summary>
    /// Check whether the current session is a network game
    /// </summary>
    /// <param name="sessionRuntimeData">Session runtime data</param>
    /// <param name="dbFactory">Database factory object</param>
    /// <param name="participant">Current participant</param>
    /// <returns>Network game or not?</returns>
    private bool CheckNetworkSession(SessionRuntimeData sessionRuntimeData, RepositoryFactory dbFactory, IParticipantDataBase? participant)
    {
        bool isNetworkSession = false;

        if (participant?.TeamId == 41 && sessionRuntimeData.CurrentSession != null)
        {
            // change session to network?
            var sessionData = dbFactory.GetRepository<SessionRepository>()?.GetQuery()?.FirstOrDefault(s => s.Id == sessionRuntimeData.CurrentSession.DbId && s.DbIsNetworkGame == 0);

            if (sessionData != null)
            {
                dbFactory.GetRepository<SessionRepository>()?.Refresh(s => s.Id == sessionRuntimeData.CurrentSession.DbId, obj => obj.IsNetworkGame = true);

                sessionRuntimeData.IsRecordable = false;

                sessionRuntimeData.IsInvalidSession = true;

                isNetworkSession = true;
            }
        }

        return isNetworkSession;
    }

    /// <summary>
    /// Create or update participant data
    /// </summary>
    /// <param name="sessionRuntimeData">Session runtime data</param>
    /// <param name="dbFactory">Database factory object</param>
    /// <param name="playerIndex">Players index</param>
    /// <param name="participant">Participant dats</param>
    /// <returns>Successfully?</returns>
    private bool CreateOrUpdateParticipant(SessionRuntimeData sessionRuntimeData, RepositoryFactory dbFactory, ushort playerIndex, IParticipantDataBase participant)
    {
        bool retValue = false;
        var driverDbId = MatchGameDriverToDbDriverId(participant.DriverId, participant.DriverName, sessionRuntimeData.GameVersion, out var driverFullName, out var isHumanDriver);

        if (driverDbId > 0 && sessionRuntimeData.CurrentSession != null)
        {
            var isNewParticipant = false;
            ParticipantRuntimeData? participantRuntimeData = null;

            if (_isCreated)
            {
                _isCreated = false;

                using var currentActivity = AppActivity.SrvSource.StartActivity("ClearParticipantsRepositoryCache");

                currentActivity?.SetTag("DbId", sessionRuntimeData.CurrentSession.DbId);

                ParticipantsRepositoryCache.Clear(sessionRuntimeData.CurrentSession.DbId);
            }

            var dbParticipant = ParticipantsRepositoryCache.GetBySessionAndDriverId(sessionRuntimeData.CurrentSession.DbId, driverDbId);

            if (dbParticipant == null)
            {
                participantRuntimeData = new ParticipantRuntimeData(sessionRuntimeData);

                retValue = isNewParticipant = CreateNewParticipant(sessionRuntimeData, dbFactory, playerIndex, participant, driverDbId, participantRuntimeData, out dbParticipant);
            }
            else
            {
                isNewParticipant = UpdateParticipant(sessionRuntimeData, playerIndex, dbParticipant, out participantRuntimeData);

                retValue = true;
            }

            if (participantRuntimeData != null)
            {
                participantRuntimeData.CarNumber = participant.RaceNumber;
                participantRuntimeData.DriverDbId = driverDbId;
                participantRuntimeData.DriverName = driverFullName;
                participantRuntimeData.ArrayIndex = playerIndex;
                participantRuntimeData.ParticipantDbId = dbParticipant?.Id ?? 0;
                participantRuntimeData.IsValidObject = participantRuntimeData.ParticipantDbId > 0;
                participantRuntimeData.IsHumanDriver = isHumanDriver;

                if (isNewParticipant || participantRuntimeData.LiveData is null)
                {
                    CreateLiveData(sessionRuntimeData, participantRuntimeData);
                }
            }
        }

        return retValue;
    }

    /// <summary>
    /// Update participant
    /// </summary>
    /// <param name="sessionRuntimeData">Session runtime data</param>
    /// <param name="playerIndex">Players index</param>
    /// <param name="dbParticipant">Participant entity</param>
    /// <param name="participantRuntimeData">Participant runtime data</param>
    /// <returns>Created?</returns>
    private bool UpdateParticipant(SessionRuntimeData sessionRuntimeData, ushort playerIndex, ParticipantEntity dbParticipant, out ParticipantRuntimeData? participantRuntimeData)
    {
        var addParticipant = false;

        if (sessionRuntimeData.Participants.ContainsKey(playerIndex) == false)
        {
            participantRuntimeData = new ParticipantRuntimeData(sessionRuntimeData);

            addParticipant = true;
        }
        else
        {
            sessionRuntimeData.Participants.TryGetValue(playerIndex, out participantRuntimeData);
        }

        if (participantRuntimeData != null)
        {
            UpdateTeam(dbParticipant, participantRuntimeData, dbParticipant.TeamId);

            UpdateNationality(dbParticipant, participantRuntimeData, dbParticipant.NationalityId);

            if (addParticipant)
            {
                sessionRuntimeData.Participants.TryAdd(playerIndex, participantRuntimeData);
            }
        }

        return addParticipant;
    }

    /// <summary>
    /// Create new participant
    /// </summary>
    /// <param name="sessionRuntimeData">Session runtime data</param>
    /// <param name="dbFactory">Database factory object</param>
    /// <param name="playerIndex">Index of participant</param>
    /// <param name="participant">Participant data</param>
    /// <param name="driverDbId">Drivers database id</param>
    /// <param name="participantRuntimeData">Participant runtime data</param>
    /// <param name="dbParticipant">Participant entity</param>
    /// <returns>Created?</returns>
    private bool CreateNewParticipant(SessionRuntimeData sessionRuntimeData, RepositoryFactory dbFactory, ushort playerIndex, IParticipantDataBase participant, long driverDbId, ParticipantRuntimeData participantRuntimeData, out ParticipantEntity? dbParticipant)
    {
        dbParticipant = new ParticipantEntity
                        {
                            IsHumanControlled = participant.IsAIControlled == false,
                            ArrayIndex = playerIndex,
                            CarRaceNumber = participant.RaceNumber,
                            SessionId = sessionRuntimeData.CurrentSession.DbId,
                            DriverName = participant.DriverName,
                            DriverId = driverDbId
                        };

        GetTeamAndNationality(participant, participantRuntimeData, dbParticipant);

        SetMyTeamMode(participant, dbParticipant);

        var isCreated = AddParticipant(sessionRuntimeData, dbFactory, playerIndex, participantRuntimeData, dbParticipant);

        return isCreated;
    }

    /// <summary>
    /// Add participant into database
    /// </summary>
    /// <param name="sessionRuntimeData">Session runtime data</param>
    /// <param name="dbFactory">Database factory object</param>
    /// <param name="playerIndex">Index of participant</param>
    /// <param name="participantRuntimeData">Participant runtime data</param>
    /// <param name="dbParticipant">Participant entity</param>
    /// <returns>Added?</returns>
    private bool AddParticipant(SessionRuntimeData sessionRuntimeData, RepositoryFactory dbFactory, ushort playerIndex, ParticipantRuntimeData participantRuntimeData, ParticipantEntity dbParticipant)
    {
        bool retValue;

        if (dbFactory.GetRepository<ParticipantRepository>()?.Add(dbParticipant) == true)
        {
            participantRuntimeData.ParticipantDbId = dbParticipant.Id;

            sessionRuntimeData.Participants.TryAdd(playerIndex, new ParticipantRuntimeData(sessionRuntimeData));

            participantRuntimeData.IsValidObject = dbParticipant.Id > 0;

            ParticipantsRepositoryCache.AddOrUpdate(dbParticipant);

            retValue = true;
        }
        else
        {
            retValue = false;
        }

        return retValue;
    }

    /// <summary>
    /// Check whether is myTeam mode
    /// </summary>
    /// <param name="participant">Participant</param>
    /// <param name="dbParticipant">Participant entity</param>
    private void SetMyTeamMode(IParticipantDataBase participant, ParticipantEntity dbParticipant)
    {
        if (participant is IParticipantData2021 data2021)
        {
            dbParticipant.IsMyTeam = data2021.IsMyTeam;
        }

        if (participant is IParticipantData2022 data2022)
        {
            dbParticipant.IsMyTeam = data2022.IsMyTeam;
        }

        if (participant is IParticipantData2023 data2023)
        {
            dbParticipant.IsMyTeam = data2023.IsMyTeam;
        }
    }

    /// <summary>
    /// Get drivers team and nationality
    /// </summary>
    /// <param name="participant">Participant</param>
    /// <param name="participantRuntimeData">Participant runtime data</param>
    /// <param name="dbParticipant">Participant entity</param>
    private void GetTeamAndNationality(IParticipantDataBase participant, ParticipantRuntimeData participantRuntimeData, ParticipantEntity dbParticipant)
    {
        _ = long.TryParse($"{GameInfo.GameVersion}{participant.TeamId}", out var teamId);

        UpdateTeam(dbParticipant, participantRuntimeData, teamId, true);

        UpdateNationality(dbParticipant, participantRuntimeData, participant.Nationality, true);
    }

    /// <summary>
    /// Update drivers team
    /// </summary>
    /// <param name="dbParticipant">Participant entity</param>
    /// <param name="participantRuntimeData">Participant runtime data</param>
    /// <param name="teamId">Id of team from game</param>
    /// <param name="isGameId">Game or database id?</param>
    private void UpdateTeam(ParticipantEntity dbParticipant, ParticipantRuntimeData? participantRuntimeData, long teamId, bool isGameId = false)
    {
        if (participantRuntimeData?.TeamDbId == 0 && teamId > 0)
        {
            var team = isGameId ? TeamRepositoryCache.GetByGameId((int)teamId) : TeamRepositoryCache.GetById(teamId);

            if (team != null)
            {
                dbParticipant.TeamId = team.Id;

                participantRuntimeData.TeamDbId = team.Id;
                participantRuntimeData.TeamName = team.Name;
            }
        }
    }

    /// <summary>
    /// Update nationality
    /// </summary>
    /// <param name="dbParticipant">Participant entity</param>
    /// <param name="participantRuntimeData">Participant runtime data</param>
    /// <param name="nationality">Nationality of driver from game</param>
    /// <param name="isGameId">Game or database id?</param>
    private void UpdateNationality(ParticipantEntity dbParticipant, ParticipantRuntimeData? participantRuntimeData, long nationality, bool isGameId = false)
    {
        if (participantRuntimeData != null && string.IsNullOrWhiteSpace(participantRuntimeData.Nationality))
        {
            var nationalityData = isGameId ? NationalityRepositoryCache.GetByGameId((int)nationality) : NationalityRepositoryCache.GetById(nationality);

            if (nationalityData != null)
            {
                dbParticipant.NationalityId = nationalityData.Id;

                participantRuntimeData.Nationality = nationalityData.Name;
            }
        }
    }

    /// <summary>
    /// Create live data and cache the reference on the participant runtime data
    /// </summary>
    /// <param name="sessionRuntimeData">Session runtime data</param>
    /// <param name="participantRuntimeData">Participant runtime data</param>
    private void CreateLiveData(SessionRuntimeData sessionRuntimeData, ParticipantRuntimeData participantRuntimeData)
    {
        var participantLiveData = sessionRuntimeData.CurrentSession.Drivers.Find(p => p.DbId == participantRuntimeData.ParticipantDbId);

        if (participantLiveData == null)
        {
            participantLiveData = new LiveDriverData
                                  {
                                      DbId = participantRuntimeData.ParticipantDbId,
                                      ArrayIndex = participantRuntimeData.ArrayIndex,
                                      CarNumber = participantRuntimeData.CarNumber,
                                      DriverName = participantRuntimeData.DriverName,
                                      TeamName = participantRuntimeData.TeamName,
                                      Nationality = participantRuntimeData.Nationality
                                  };

            sessionRuntimeData.CurrentSession.Drivers.Add(participantLiveData);
        }

        participantRuntimeData.LiveData = participantLiveData as LiveDriverData;
    }

    /// <summary>
    /// Get the correct driver id
    /// </summary>
    /// <param name="gameDriverId">Driver id from the game</param>
    /// <param name="gameDriverName">Name of the driver</param>
    /// <param name="gameVersion">Version of the game</param>
    /// <param name="driverFullName">Full name of driver</param>
    /// <param name="isHumanDriver">Human driver?</param>
    /// <returns>Id of the driver</returns>
    private long MatchGameDriverToDbDriverId(int gameDriverId, string gameDriverName, int gameVersion, out string driverFullName, out bool isHumanDriver)
    {
        long driverId = 0;

        driverFullName = gameDriverName;

        isHumanDriver = false;

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            if ((gameVersion <= 2020 && gameDriverId >= 100) || gameDriverId == 255)
            {
                isHumanDriver = true;
            }

            if (isHumanDriver)
            {
                _ = int.TryParse($"{gameVersion}{gameDriverId}", out var dbDriverId);

                var driverData = DriverRepositoryCache.GetByGameId(dbDriverId);

                if (driverData != null)
                {
                    driverId = driverData.Id;

                    driverFullName = driverData.Name;
                }
                else
                {
                    driverData = new DriverEntity
                                 {
                                     DriverGameId = dbDriverId,
                                     IsHumanDriver = true,
                                     Name = gameDriverName
                                 };

                    if (dbFactory.GetRepository<DriverRepository>()?.Add(driverData) == true)
                    {
                        driverId = driverData.Id;

                        DriverRepositoryCache.AddOrUpdate(driverData);
                    }
                }
            }
            else
            {
                gameDriverId = FixDriverId(gameDriverId, gameVersion);

                var driverData = DriverRepositoryCache.GetByGameId(gameDriverId);

                if (driverData != null)
                {
                    driverId = driverData.Id;

                    driverFullName = driverData.Name;
                }
            }
        }

        return driverId;
    }

    /// <summary>
    /// Fix driver id starting with F1 2023 and newer
    /// </summary>
    /// <param name="gameDriverId">Driver id from game</param>
    /// <param name="gameVersion">Version of game</param>
    /// <returns>Recalculated driver id</returns>
    private int FixDriverId(int gameDriverId, int gameVersion)
    {
        var newGameDriverId = gameDriverId;

        if (gameVersion >= 2023 && gameVersion < 2025 && gameDriverId > 126)
        {
            newGameDriverId = gameDriverId + 12;
        }

        if (gameVersion >= 2025)
        {
            var dbDriverId = $"{gameVersion}{gameDriverId}";

            _ = int.TryParse(dbDriverId, out newGameDriverId);
        }

        return newGameDriverId;
    }

    #endregion // Private methods
}