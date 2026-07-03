using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Lap history data from session (F1 2026)
/// </summary>
public class SessionHistoryLapData2026 : SessionHistoryLapData2025, ILapHistoryData2026;