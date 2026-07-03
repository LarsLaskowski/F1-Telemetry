using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Implementation of participant data interface 2026 (driver, network and team ids are uint16)
/// </summary>
public class ParticipantData2026 : ParticipantData2025, IParticipantData2026;