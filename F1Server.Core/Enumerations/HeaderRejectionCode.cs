namespace F1Server.Core.Enumerations;

/// <summary>
/// Bounded classification of why a packet header was rejected, safe to use as a metric dimension
/// </summary>
public enum HeaderRejectionCode
{
    /// <summary>
    /// The header was not rejected
    /// </summary>
    None = 0,

    /// <summary>
    /// The packet was shorter than the minimum header size
    /// </summary>
    PacketTooShort,

    /// <summary>
    /// The packet reported game version 2023 or later but was shorter than the 2023+ header size
    /// </summary>
    Undersized2023Header,

    /// <summary>
    /// An exception was thrown while parsing the packet header
    /// </summary>
    ParseException
}