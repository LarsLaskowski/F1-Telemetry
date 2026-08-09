using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Session data of F1 2025. The session layout is unchanged compared to F1 2024, so every property is inherited
/// from <see cref="SessionData2024"/> instead of being declared a second time
/// </summary>
public class SessionData2025 : SessionData2024, ISessionData2025;