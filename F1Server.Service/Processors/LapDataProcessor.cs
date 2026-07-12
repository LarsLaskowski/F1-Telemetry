using System.Diagnostics;
using System.Runtime.CompilerServices;

using F1Server.Core.Enumerations;
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
/// Process lap data packet
/// </summary>
internal class LapDataProcessor : BaseProcessor
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
    public LapDataProcessor(IServiceProvider serviceProvider, PacketHeader packetHeader, LiveGameData gameInfo)
        : base(serviceProvider, packetHeader, gameInfo)
    {
        var applicationData = serviceProvider.GetRequiredService<F1ServerApplicationData>();

        _telemetryWriter = applicationData.TelemetryWriter;

        Logger?.LogInformation("LapDataProcessor created.");
    }

    #endregion // Constructors

    #region Private methods

    /// <summary>
    /// Process laps
    /// </summary>
    /// <param name="sessionRuntimeData">Session runtime data</param>
    /// <param name="lapDataPacket">Lap datas from received packet</param>
    /// <returns>Number of cars on track</returns>
    private int ProcessLaps(SessionRuntimeData sessionRuntimeData, LapData lapDataPacket)
    {
        var carsOnTrack = 0;

        if (lapDataPacket.PacketData != null)
        {
            for (ushort lap = 0; lap < lapDataPacket.PacketData.LapData.Length; lap++)
            {
                var lapInfo = lapDataPacket.PacketData.LapData[lap];

                if (sessionRuntimeData.Participants.TryGetValue(lap, out var participantData)
                    && ProcessLap(participantData, lapInfo, sessionRuntimeData))
                {
                    ++carsOnTrack;
                }
            }
        }

        return carsOnTrack;
    }

    /// <summary>
    /// Update session runtime information
    /// </summary>
    /// <param name="sessionRuntimeData">Session runtime data</param>
    /// <param name="carsOnTrack">Cars on track</param>
    private void UpdateSessionInformation(SessionRuntimeData sessionRuntimeData, int carsOnTrack)
    {
        sessionRuntimeData.CurrentSession.CurrentCarsOnTrack = carsOnTrack;

        // No one with a time? Use grid position or car position
        if (sessionRuntimeData.CurrentSession.TimeTable.Count == 0 && sessionRuntimeData.CurrentSession.Drivers?.Count > 0)
        {
            var hasGridPosition = false;
            var hasCarPosition = false;

            // Cheap scan first, so the LINQ chains only run when they can produce a non-empty result
            foreach (var driver in sessionRuntimeData.CurrentSession.Drivers)
            {
                hasGridPosition |= driver.GridPosition > 0;
                hasCarPosition |= driver.CarPosition > 0;
            }

            // Grid postion?
            if (hasGridPosition)
            {
                sessionRuntimeData.CurrentSession.TimeTable = sessionRuntimeData.CurrentSession.Drivers.Where(d => d.GridPosition > 0).OrderBy(d => d.GridPosition).Select(d => d.ArrayIndex).ToList();
            }
            else if (hasCarPosition)
            {
                // Car position
                sessionRuntimeData.CurrentSession.TimeTable = sessionRuntimeData.CurrentSession.Drivers.Where(d => d.CarPosition > 0).OrderBy(d => d.CarPosition).Select(d => d.ArrayIndex).ToList();
            }
        }
    }

    /// <summary>
    /// Process one lap
    /// </summary>
    /// <param name="participantRuntimeData">Participant runtime data</param>
    /// <param name="lapInfo">Lap information</param>
    /// <param name="sessionRuntimeData">Session runtime data</param>
    /// <returns>Cars is on track?</returns>
    private bool ProcessLap(ParticipantRuntimeData participantRuntimeData, ILapDataBase lapInfo, SessionRuntimeData sessionRuntimeData)
    {
        var isNewLapEntity = false;
        var lastRecordableLapState = participantRuntimeData.IsOnRecordableLap;

        participantRuntimeData.CurrentStatus = lapInfo.CurrentDriverStatus;
        participantRuntimeData.IsOnRecordableLap = IsRecordableLap(lapInfo, participantRuntimeData, sessionRuntimeData);

        var isRecordableLapStateChanged = lastRecordableLapState != participantRuntimeData.IsOnRecordableLap;

        var iLapData = GetIndependentLapData(lapInfo, sessionRuntimeData.GameVersion);

        // Is a recordable lap or last packet was a recordable lap
        if (participantRuntimeData.IsOnRecordableLap || isRecordableLapStateChanged)
        {
            // Do we have a lap that needs to be completed?
            FinishLastLap(participantRuntimeData, iLapData, lapInfo);

            if (participantRuntimeData.IsOnRecordableLap)
            {
                isNewLapEntity = UpdateCurrentLap(participantRuntimeData, lapInfo, sessionRuntimeData, iLapData);
            }
        }

        // last lap packet was a recordable lap, now we are on a non-recordable lap
        var isRecordableStateChanged = isRecordableLapStateChanged && participantRuntimeData.IsOnRecordableLap == false;
        var isClearTelemetry = isRecordableStateChanged && participantRuntimeData.CarIsOnTrack == false && participantRuntimeData.CarWasOnTrack;

        // Driver state changed, this lap packet is not relevant
        if (isClearTelemetry)
        {
            // Car is currently not on track, but was before on track
            participantRuntimeData.ClearTelemetryData(participantRuntimeData.CurrentLapNumber);
        }

        // State changed from recordable to non recordable
        if (isRecordableStateChanged)
        {
            // Remove started lap
            participantRuntimeData.RemoveLap(participantRuntimeData.CurrentLapNumber);
        }

        // Current lap number in packet data is now correct?
        if (participantRuntimeData.IsOnNextLap && participantRuntimeData.CurrentLapNumber == lapInfo.CurrentLapNumber && participantRuntimeData.CurrentPacketLapNumber < lapInfo.CurrentLapNumber)
        {
            participantRuntimeData.IsOnNextLap = false;
        }

        participantRuntimeData.LastLapDistance = lapInfo.LapDistance;
        participantRuntimeData.LastLapFrameIdentifier = CurrentFrameIdentifier > participantRuntimeData.LastLapFrameIdentifier ? CurrentFrameIdentifier : participantRuntimeData.LastLapFrameIdentifier;
        participantRuntimeData.LastLapSessionTimestamp = SessionTimestampNum > participantRuntimeData.LastLapSessionTimestamp ? SessionTimestampNum : participantRuntimeData.LastLapSessionTimestamp;
        participantRuntimeData.LastLapSector = participantRuntimeData.IsOnRecordableLap ? lapInfo.CurrentSector : Sector.Unknown;

        if (participantRuntimeData.IsHumanDriver)
        {
            PublishLapData(iLapData, lapInfo, sessionRuntimeData);
        }

        UpdateLiveData(lapInfo, iLapData, sessionRuntimeData, participantRuntimeData, isNewLapEntity);

        return participantRuntimeData.CarIsOnTrack;
    }

    /// <summary>
    /// Update current lap data, finish last lap if neccessary
    /// </summary>
    /// <param name="participantRuntimeData">Runtime data of participant</param>
    /// <param name="lapInfo">Lap data</param>
    /// <param name="sessionRuntimeData">Session runtime data</param>
    /// <param name="iLapData">Independent lap data</param>
    /// <returns>Is new lap entity?</returns>
    private bool UpdateCurrentLap(ParticipantRuntimeData participantRuntimeData, ILapDataBase lapInfo, SessionRuntimeData sessionRuntimeData, IndependentLapData iLapData)
    {
        var lapData = GetCurrentLapEntity(participantRuntimeData, sessionRuntimeData, out var isNewLapEntity);

        // Update lapdata, lap must be incomplete!
        if (lapData.IsCompleted == false)
        {
            lapData.CarPosition = lapInfo.CarPosition;
            lapData.PitStatus = lapInfo.CurrentPitStatus;
            lapData.ResultStatus = lapInfo.CurrentResultStatus;
            lapData.DriverStatus = lapInfo.CurrentDriverStatus;
            lapData.IsInvalid = lapInfo.IsCurrentLapInvalid;
            lapData.Sector1Time = iLapData.Sector1Time;
            lapData.Sector2Time = iLapData.Sector2Time;

            if (isNewLapEntity)
            {
                participantRuntimeData.AddLap(lapData);
            }
        }

        participantRuntimeData.CurrentPacketLapNumber = lapInfo.CurrentLapNumber;

        return isNewLapEntity;
    }

    /// <summary>
    /// Finish data of last lap
    /// </summary>
    /// <param name="participantRuntimeData">Runtime data of participant</param>
    /// <param name="iLapData">Independent lap data</param>
    /// <param name="lapInfo">Lap data</param>
    private void FinishLastLap(ParticipantRuntimeData participantRuntimeData, IndependentLapData iLapData, ILapDataBase lapInfo)
    {
        var isLapChanged = lapInfo.CurrentLapNumber > participantRuntimeData.CurrentPacketLapNumber;
        var isLapCurrentlyChanged = participantRuntimeData.LastLapDistance > lapInfo.LapDistance
                                    || lapInfo.CurrentSector < participantRuntimeData.LastLapSector;
        var isLapFinished = false;

        // Lap number has changed?
        if (isLapChanged || isLapCurrentlyChanged)
        {
            var lapEntity = participantRuntimeData.GetLap(participantRuntimeData.CurrentLapNumber);

            // Is this an unfinished lap and we have a last lap time in our data?
            if (lapEntity != null && iLapData.LastLapTime > 0 && participantRuntimeData.LastLapTime != iLapData.LastLapTime)
            {
                if (participantRuntimeData.PreCompleteLap(participantRuntimeData.CurrentLapNumber, iLapData))
                {
                    participantRuntimeData.LastLapTime = iLapData.LastLapTime;
                    participantRuntimeData.LastLapTimeNumber = participantRuntimeData.CurrentLapNumber;

                    isLapFinished = true;
                }

                CheckLastLap(iLapData, participantRuntimeData);
            }

            participantRuntimeData.IsNewTelemetry = true;

            participantRuntimeData.CurrentLapNumber = lapInfo.CurrentLapNumber;
        }

        // Last lap finished, lap is changed, lap number is equal, but should be a higher one
        if (isLapFinished
            && isLapCurrentlyChanged
            && lapInfo.CurrentLapNumber == participantRuntimeData.CurrentLapNumber
            && isLapChanged == false)
        {
            participantRuntimeData.CurrentLapNumber++;

            participantRuntimeData.IsOnNextLap = true;
        }
    }

    /// <summary>
    /// Check last lap
    /// </summary>
    /// <param name="lapData">Lap data</param>
    /// <param name="participantRuntimeData">Runtime data of participant</param>
    private void CheckLastLap(IndependentLapData lapData, ParticipantRuntimeData participantRuntimeData)
    {
        if (lapData.LastLapTime > 0 && participantRuntimeData.CurrentLapNumber > 0)
        {
            // set complete lap time for previous lap
            var lastLapNumber = participantRuntimeData.CurrentLapNumber;
            var prevLapData = participantRuntimeData.GetLap(lastLapNumber);

            if (prevLapData?.IsCompleted == false)
            {
                prevLapData.LapTime = lapData.LastLapTime;

                if (CheckLapIsCompleted(prevLapData))
                {
                    participantRuntimeData.CompleteLap(lastLapNumber);
                }
            }
        }
    }

    /// <summary>
    /// Updates live data
    /// </summary>
    /// <param name="lapInfo">Lap base data</param>
    /// <param name="lapData">Data of lap</param>
    /// <param name="sessionRuntimeData">Session runtime data</param>
    /// <param name="participantRuntimeData">Participant runtime data</param>
    /// <param name="isNewLapEntity">Is new lap?</param>
    private void UpdateLiveData(ILapDataBase lapInfo, IndependentLapData lapData, SessionRuntimeData sessionRuntimeData, ParticipantRuntimeData participantRuntimeData, bool isNewLapEntity)
    {
        if (sessionRuntimeData?.CurrentSession is not null && participantRuntimeData.LiveData is LiveDriverData liveData)
        {
            liveData.CurrentDriverStatus = lapInfo.CurrentDriverStatus;
            liveData.GridPosition = lapInfo.GridPosition;
            liveData.CarPosition = lapInfo.CarPosition;
            liveData.CurrentLapTime = lapData.CurrentLapTime;

            CheckFastestLapTimes(lapData, sessionRuntimeData.CurrentSession, liveData);

            if (isNewLapEntity && sessionRuntimeData.GameVersion < 2021)
            {
                liveData.LapsDriven++;
            }

            if (participantRuntimeData.IsHumanDriver)
            {
                _telemetryWriter?.WriteSessionData(sessionRuntimeData, liveData);
            }
        }
    }

    /// <summary>
    /// Is this lap recordable?
    /// </summary>
    /// <param name="lapDataBase">Base lap information</param>
    /// <param name="participantRuntimeData">Participant runtime data</param>
    /// <param name="sessionRuntimeData">Session runtime data</param>
    /// <returns>Is recordable?</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsRecordableLap(ILapDataBase lapDataBase, ParticipantRuntimeData participantRuntimeData, SessionRuntimeData sessionRuntimeData)
    {
        var isRecordableLap = false;
        var sessionType = sessionRuntimeData.CurrentSession.SessionType;
        var driverStatus = lapDataBase.CurrentDriverStatus;

        participantRuntimeData.CarIsOnTrack = driverStatus is DriverStatus.InLap or DriverStatus.OutLap or DriverStatus.FlyingLap or DriverStatus.OnTrack;

        if (sessionType is SessionType.Race
                        or SessionType.Race2
                        or SessionType.Race3)
        {
            // Race
            isRecordableLap = driverStatus is DriverStatus.InLap or DriverStatus.OutLap or DriverStatus.OnTrack;

            // After finishing the race the leader is on InLap and the lap number is greater as the current lap number - can be ignored!
            if (driverStatus == DriverStatus.FlyingLap && sessionRuntimeData.TotalLaps < lapDataBase.CurrentLapNumber)
            {
                isRecordableLap = false;
            }
        }

        if (sessionType is SessionType.Practice1
                        or SessionType.Practice2
                        or SessionType.Practice3
                        or SessionType.ShortPractice)
        {
            // Practice
            isRecordableLap = driverStatus == DriverStatus.FlyingLap;
        }

        if (sessionType is SessionType.Qualifying1
                        or SessionType.Qualifying2
                        or SessionType.Qualifying3
                        or SessionType.ShortQualifying
                        or SessionType.OneShotQualifying
                        or SessionType.SprintShootout1
                        or SessionType.SprintShootout2
                        or SessionType.SprintShootout3
                        or SessionType.ShortSprintShootout
                        or SessionType.OneShotSprintShootout)
        {
            // Qualifying
            isRecordableLap = driverStatus == DriverStatus.FlyingLap;
        }

        if (sessionType == SessionType.TimeTrial)
        {
            // Time trial
            isRecordableLap = driverStatus is DriverStatus.FlyingLap or DriverStatus.OnTrack;
        }

        return isRecordableLap;
    }

    /// <summary>
    /// Gets or create lap entity of current lap
    /// </summary>
    /// <param name="participantRuntimeData">Participant runtime data</param>
    /// <param name="sessionRuntimeData">Session runtime data</param>
    /// <param name="isNewLap">Is new lap?</param>
    /// <returns>Lap entity object</returns>
    private LapEntity GetCurrentLapEntity(ParticipantRuntimeData participantRuntimeData, SessionRuntimeData sessionRuntimeData, out bool isNewLap)
    {
        var lapEntity = participantRuntimeData.GetLap(participantRuntimeData.CurrentLapNumber);

        isNewLap = lapEntity == null;

        lapEntity ??= new LapEntity
                      {
                          LapNumber = participantRuntimeData.CurrentLapNumber,
                          ParticipantId = participantRuntimeData.ParticipantDbId,
                          SessionId = sessionRuntimeData.CurrentSession.DbId,
                          IsStarted = true
                      };

        return lapEntity;
    }

    /// <summary>
    /// Check fastest lap times
    /// </summary>
    /// <param name="lapData">Lap database object</param>
    /// <param name="liveSession">Live session data</param>
    /// <param name="liveDriverData">Live driver data</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CheckFastestLapTimes(IndependentLapData lapData, LiveSessionData liveSession, LiveDriverData? liveDriverData)
    {
        if (liveDriverData != null)
        {
            if (lapData.Sector1Time > 0)
            {
                IsFastestSectorOne(lapData, liveSession, liveDriverData);
            }

            if (lapData.Sector2Time > 0)
            {
                IsFastestSectorTwo(lapData, liveSession, liveDriverData);
            }
        }
    }

    /// <summary>
    /// Is fastest sector 1?
    /// </summary>
    /// <param name="lapData">Lap data</param>
    /// <param name="liveSession">Live session data</param>
    /// <param name="liveDriverData">Live driver data</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void IsFastestSectorOne(IndependentLapData lapData, LiveSessionData liveSession, LiveDriverData liveDriverData)
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void IsFastestSectorTwo(IndependentLapData lapData, LiveSessionData liveSession, LiveDriverData liveDriverData)
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
    /// Gets independent lap data objekt from game dependent lap data packet
    /// </summary>
    /// <param name="lapData">Game dependent lap data</param>
    /// <param name="gameVersion">Game version</param>
    /// <returns>Game independent lap object</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private IndependentLapData GetIndependentLapData(ILapDataBase lapData, int gameVersion)
    {
        IndependentLapData iLapData = new();

        iLapData = gameVersion switch
                   {
                       2019 => GetLapData2019(lapData),
                       2020 => GetLapData2020(lapData),
                       2021 => GetLapData2021(lapData),
                       2022 => GetLapData2022(lapData),
                       2023 or 2024 or 2025 or 2026 => GetLapData2023(lapData),
                       _ => iLapData
                   };

        return iLapData;
    }

    /// <summary>
    /// Get independent lap data from F1 2019 lap data
    /// </summary>
    /// <param name="lapData">Game lap data</param>
    /// <returns>Independent lap data</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private IndependentLapData GetLapData2019(ILapDataBase lapData)
    {
        var ilapData = new IndependentLapData();

        if (lapData is ILapData2019 lapData2019)
        {
            ilapData.CurrentLapTime = (uint)(lapData2019.CurrentLapTime * 1000);
            ilapData.LastLapTime = (uint)(lapData2019.LastLapTime * 1000);
            ilapData.Sector1Time = (uint)(lapData2019.Sector1Time * 1000);
            ilapData.Sector2Time = (uint)(lapData2019.Sector2Time * 1000);
        }

        return ilapData;
    }

    /// <summary>
    /// Get independent lap data from F1 2019 lap data
    /// </summary>
    /// <param name="lapData">Game lap data</param>
    /// <returns>Independent lap data</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private IndependentLapData GetLapData2020(ILapDataBase lapData)
    {
        var ilapData = new IndependentLapData();

        if (lapData is ILapData2020 lapData2020)
        {
            ilapData.CurrentLapTime = (uint)(lapData2020.CurrentLapTime * 1000);
            ilapData.LastLapTime = (uint)(lapData2020.LastLapTime * 1000);
            ilapData.Sector1Time = lapData2020.Sector1Time;
            ilapData.Sector2Time = lapData2020.Sector2Time;
        }

        return ilapData;
    }

    /// <summary>
    /// Get independent lap data from F1 2021 lap data
    /// </summary>
    /// <param name="lapData">Game lap data</param>
    /// <returns>Independent lap data</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private IndependentLapData GetLapData2021(ILapDataBase lapData)
    {
        var ilapData = new IndependentLapData();

        if (lapData is ILapData2021 lapData2021)
        {
            ilapData.CurrentLapTime = lapData2021.CurrentLapTime;
            ilapData.LastLapTime = lapData2021.LastLapTime;
            ilapData.Sector1Time = lapData2021.Sector1Time;
            ilapData.Sector2Time = lapData2021.Sector2Time;
        }

        return ilapData;
    }

    /// <summary>
    /// Get independent lap data from F1 2022 lap data
    /// </summary>
    /// <param name="lapData">Game lap data</param>
    /// <returns>Independent lap data</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private IndependentLapData GetLapData2022(ILapDataBase lapData)
    {
        var ilapData = new IndependentLapData();

        if (lapData is ILapData2022 lapData2022)
        {
            ilapData.CurrentLapTime = lapData2022.CurrentLapTime;
            ilapData.LastLapTime = lapData2022.LastLapTime;
            ilapData.Sector1Time = lapData2022.Sector1Time;
            ilapData.Sector2Time = lapData2022.Sector2Time;
        }

        return ilapData;
    }

    /// <summary>
    /// Get independent lap data from F1 2023 lap data
    /// </summary>
    /// <param name="lapData">Game lap data</param>
    /// <returns>Independent lap data</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private IndependentLapData GetLapData2023(ILapDataBase lapData)
    {
        var ilapData = new IndependentLapData();

        if (lapData is ILapData2023 lapData2023)
        {
            ilapData.CurrentLapTime = lapData2023.CurrentLapTime;
            ilapData.LastLapTime = lapData2023.LastLapTime;
            ilapData.Sector1Time = lapData2023.Sector1Time;
            ilapData.Sector2Time = lapData2023.Sector2Time;
        }

        return ilapData;
    }

    /// <summary>
    /// Check if lap is completed now
    /// </summary>
    /// <param name="lapData">Lap entity</param>
    /// <returns>Is completed?</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool CheckLapIsCompleted(LapEntity lapData)
    {
        var isCompleted = false;

        if (lapData.Sector1Time > 0 && lapData.Sector2Time > 0 && lapData.IsFinished)
        {
            lapData.Sector3Time = lapData.LapTime - lapData.Sector1Time - lapData.Sector2Time;

            if (lapData.Sector3Time > 0 && lapData.LapTime >= (lapData.Sector1Time + lapData.Sector2Time + lapData.Sector3Time))
            {
                lapData.IsCompleted = true;

                isCompleted = true;
            }
        }

        return isCompleted;
    }

    /// <summary>
    /// Publish lap data to telemetry writer
    /// </summary>
    /// <param name="lapData">Information about current times</param>
    /// <param name="lapInfo">Information about current lap</param>
    /// <param name="sessionRuntimeData">Information about the current session</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void PublishLapData(IIndependentLapData lapData, ILapDataBase lapInfo, ISessionRuntimeData sessionRuntimeData)
    {
        if (sessionRuntimeData != null
            && _telemetryWriter?.IsReady == true)
        {
            _telemetryWriter.WriteLapData(lapData, lapInfo, sessionRuntimeData);
        }
    }

    #endregion // Private methods

    #region BaseProcessor

    /// <inheritdoc/>
    public override bool Process(object? dataObject, SessionRuntimeData? sessionRuntimeData)
    {
        var isProcessed = true;

        LastException = string.Empty;

        var processStartTimestamp = Stopwatch.GetTimestamp();

        if (dataObject is LapData lapDataPacket
            && sessionRuntimeData?.IsValid == true
            && sessionRuntimeData.IsFinished == false)
        {
            try
            {
                if (sessionRuntimeData.HasParticipants && sessionRuntimeData.CurrentSession != null)
                {
                    var carsOnTrack = ProcessLaps(sessionRuntimeData, lapDataPacket);

                    UpdateSessionInformation(sessionRuntimeData, carsOnTrack);
                }
            }
            catch (Exception ex)
            {
                LastException = ex.ToString();

                Logger?.LogError(ex, "Error processing LapData packet!");

                isProcessed = false;
            }
        }
        else
        {
            Logger?.LogWarning("Unexpected data object or session not valid (processor: {Processor})!", nameof(LapDataProcessor));
        }

        RecordSlowProcessingActivity(nameof(LapDataProcessor), Stopwatch.GetElapsedTime(processStartTimestamp), isProcessed);

        return isProcessed;
    }

    #endregion // BaseProcessor
}