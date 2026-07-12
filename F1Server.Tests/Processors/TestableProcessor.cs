using F1Server.Core.Packets.Data;
using F1Server.Data;
using F1Server.Service.Processors;
using F1Server.Service.Runtime;

namespace F1Server.Tests.Processors;

/// <summary>
/// Minimal <see cref="BaseProcessor"/> subclass exposing the protected <see cref="BaseProcessor.RecordSlowProcessingActivity"/>
/// helper for direct testing
/// </summary>
internal sealed class TestableProcessor : BaseProcessor
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="serviceProvider">Service provider</param>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="gameInfo">Runtime game information</param>
    public TestableProcessor(IServiceProvider serviceProvider, PacketHeader packetHeader, LiveGameData gameInfo)
        : base(serviceProvider, packetHeader, gameInfo)
    {
    }

    #endregion // Constructors

    #region Methods

    /// <summary>
    /// Exposes the protected <see cref="BaseProcessor.RecordSlowProcessingActivity"/> helper for testing
    /// </summary>
    /// <param name="processorName">Name of the processor, used as the span name</param>
    /// <param name="elapsed">Elapsed processing time</param>
    /// <param name="isProcessed">Whether the packet was processed successfully</param>
    public void InvokeRecordSlowProcessingActivity(string processorName, TimeSpan elapsed, bool isProcessed)
    {
        RecordSlowProcessingActivity(processorName, elapsed, isProcessed);
    }

    #endregion // Methods

    #region BaseProcessor

    /// <inheritdoc/>
    public override bool Process(object? dataObject, SessionRuntimeData? sessionRuntimeData)
    {
        return true;
    }

    #endregion // BaseProcessor
}