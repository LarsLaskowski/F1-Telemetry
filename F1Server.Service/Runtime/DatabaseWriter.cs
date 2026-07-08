using System.Threading.Channels;

using F1Server.Data;
using F1Server.Db.Entity;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace F1Server.Service.Runtime;

/// <summary>
/// Background database writer executing queued write jobs on a single long-lived consumer task,
/// so lap and telemetry persistence never blocks the packet-processing thread
/// </summary>
internal static class DatabaseWriter
{
    #region Fields

    /// <summary>
    /// Lock object synchronizing consumer start, flush and shutdown
    /// </summary>
    private static readonly Lock _lockObj = new();

    /// <summary>
    /// Channel holding the queued database writer jobs
    /// </summary>
    private static Channel<DatabaseWriterJob>? _jobChannel;

    /// <summary>
    /// Single consumer task processing the queued jobs in FIFO order
    /// </summary>
    private static Task? _consumerTask;

    #endregion // Fields

    #region Properties

    /// <summary>
    /// Gets a value indicating whether the consumer task is currently running
    /// </summary>
    public static bool IsRunning
    {
        get
        {
            lock (_lockObj)
            {
                return _consumerTask != null;
            }
        }
    }

    /// <summary>
    /// Logger resolved lazily from the application service provider
    /// </summary>
    private static ILogger? Logger => RepositoryFactory.ServiceProvider?.GetService<F1ServerApplicationData>()?.Logger;

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Enqueues a job for the background consumer task; the consumer is started at first use
    /// </summary>
    /// <param name="job">Job to execute in the background</param>
    public static void Enqueue(DatabaseWriterJob job)
    {
        if (job is not null)
        {
            lock (_lockObj)
            {
                if (_jobChannel == null)
                {
                    _jobChannel = Channel.CreateUnbounded<DatabaseWriterJob>(new UnboundedChannelOptions
                                                                             {
                                                                                 SingleReader = true
                                                                             });

                    var jobReader = _jobChannel.Reader;

                    using (ExecutionContext.SuppressFlow())
                    {
                        _consumerTask = Task.Run(() => ConsumeJobsAsync(jobReader));
                    }
                }

                _jobChannel.Writer.TryWrite(job);
            }
        }
    }

    /// <summary>
    /// Returns a task completing when all jobs enqueued before this call have been executed
    /// </summary>
    /// <returns>Task representing the flush barrier</returns>
    public static Task FlushAsync()
    {
        var barrierJob = new FlushBarrierJob();

        lock (_lockObj)
        {
            if (_jobChannel == null || _jobChannel.Writer.TryWrite(barrierJob) == false)
            {
                return Task.CompletedTask;
            }
        }

        return barrierJob.Completion;
    }

    /// <summary>
    /// Blocks until all jobs enqueued before this call have been executed
    /// </summary>
    public static void Flush()
    {
        FlushAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Completes the channel and waits until all pending jobs are executed, so no queued data is lost on shutdown
    /// </summary>
    public static void Shutdown()
    {
        Channel<DatabaseWriterJob>? jobChannel;
        Task? consumerTask;

        lock (_lockObj)
        {
            jobChannel = _jobChannel;
            consumerTask = _consumerTask;

            _jobChannel = null;
            _consumerTask = null;
        }

        jobChannel?.Writer.TryComplete();

        consumerTask?.GetAwaiter().GetResult();
    }

    #endregion // Methods

    #region Task methods

    /// <summary>
    /// Consumes queued jobs until the channel is completed
    /// </summary>
    /// <param name="jobReader">Reader of the job channel</param>
    /// <returns>A task representing the asynchronous operation</returns>
    private static async Task ConsumeJobsAsync(ChannelReader<DatabaseWriterJob> jobReader)
    {
        await foreach (var job in jobReader.ReadAllAsync().ConfigureAwait(false))
        {
            try
            {
                await job.ExecuteAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error executing database writer job {JobType}!", job.GetType().Name);
            }
        }
    }

    #endregion // Task methods
}