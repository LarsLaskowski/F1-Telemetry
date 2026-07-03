using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Data packet with lap information (F1 2026)
/// </summary>
public class LapData2026 : LapData2025, ILapData2026;