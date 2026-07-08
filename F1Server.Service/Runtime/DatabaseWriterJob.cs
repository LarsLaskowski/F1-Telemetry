namespace F1Server.Service.Runtime;

/// <summary>
/// Base class of all jobs executed by the <see cref="DatabaseWriter"/> background consumer task
/// </summary>
internal abstract class DatabaseWriterJob
{
    #region Methods

    /// <summary>
    /// Executes the database work of this job
    /// </summary>
    /// <returns>A task representing the asynchronous operation</returns>
    public abstract Task ExecuteAsync();

    #endregion // Methods
}