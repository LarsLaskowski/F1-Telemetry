namespace F1Server.Service.Runtime;

/// <summary>
/// Barrier job signaling a task when all previously enqueued database writer jobs have been executed
/// </summary>
internal class FlushBarrierJob : IDatabaseWriterJob
{
    #region Fields

    /// <summary>
    /// Completion source signaled when the barrier is reached by the consumer task
    /// </summary>
    private readonly TaskCompletionSource _completionSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

    #endregion // Fields

    #region Properties

    /// <summary>
    /// Gets the task completing when the barrier is reached
    /// </summary>
    public Task Completion => _completionSource.Task;

    #endregion // Properties

    #region IDatabaseWriterJob

    /// <inheritdoc/>
    public Task ExecuteAsync()
    {
        _completionSource.TrySetResult();

        return Task.CompletedTask;
    }

    #endregion // IDatabaseWriterJob
}