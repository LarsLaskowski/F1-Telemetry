using F1Server.Service.Runtime;

namespace F1Server.Tests.Runtime;

/// <summary>
/// Job failing with an exception to test the error handling of the database writer consumer task
/// </summary>
internal sealed class ThrowingJob : DatabaseWriterJob
{
    #region DatabaseWriterJob

    /// <inheritdoc/>
    public override Task ExecuteAsync()
    {
        throw new InvalidOperationException("Test job failure!");
    }

    #endregion // DatabaseWriterJob
}