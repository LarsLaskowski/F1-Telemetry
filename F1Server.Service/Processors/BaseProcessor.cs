using F1Server.Core.Packets.Data;
using F1Server.Data;
using F1Server.Service.Runtime;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace F1Server.Service.Processors;

/// <summary>
/// Base class of specialized packet processors
/// </summary>
public abstract class BaseProcessor
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="serviceProvider">Service provider</param>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="gameInfo">Database id of current game</param>
    protected BaseProcessor(IServiceProvider serviceProvider, PacketHeader packetHeader, LiveGameData gameInfo)
    {
        ServiceProvider = serviceProvider;
        PacketHeader = packetHeader;
        GameInfo = gameInfo;

        var applicationData = serviceProvider.GetRequiredService<F1ServerApplicationData>();

        Logger = applicationData.Logger;
    }

    #endregion // Constructors

    #region Properties

    /// <summary>
    /// Logger for this processor
    /// </summary>
    public ILogger Logger { get; }

    /// <summary>
    /// Service provider
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Packet header
    /// </summary>
    public PacketHeader PacketHeader { get; }

    /// <summary>
    /// Information about current game
    /// </summary>
    public LiveGameData GameInfo { get; }

    /// <summary>
    /// Current frame identifier
    /// </summary>
    public uint CurrentFrameIdentifier { get; set; }

    /// <summary>
    /// Last frame identifier
    /// </summary>
    public uint LastFrameIdentifier { get; set; }

    /// <summary>
    /// Session timestamp
    /// </summary>
    public uint SessionTimestampNum { get; set; }

    /// <summary>
    /// Last exception
    /// </summary>
    public string LastException { get; set; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Abstract method
    /// </summary>
    /// <param name="dataObject">Data object</param>
    /// <param name="sessionRuntimeData">Runtime information of current session</param>
    /// <returns>Status</returns>
    public abstract bool Process(object? dataObject, SessionRuntimeData? sessionRuntimeData);

    #endregion // Methods
}