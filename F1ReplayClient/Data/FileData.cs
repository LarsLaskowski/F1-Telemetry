using System.IO;

namespace F1ReplayClient.Data;

/// <summary>
/// Data for one file
/// </summary>
internal class FileData
{
    #region Properties

    /// <summary>
    /// File name
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// File information
    /// </summary>
    public FileInfo FileInfo { get; set; }

    /// <summary>
    /// Overall frame identifier
    /// </summary>
    public uint OverallFrameIdentifier { get; set; }

    /// <summary>
    /// Session time
    /// </summary>
    public uint SessionTime { get; set; }

    #endregion // Properties
}