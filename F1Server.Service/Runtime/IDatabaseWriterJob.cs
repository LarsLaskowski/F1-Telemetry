namespace F1Server.Service.Runtime;

/// <summary>
/// Defines a contract for jobs executed by the <see cref="DatabaseWriter"/> background consumer task
/// </summary>
internal interface IDatabaseWriterJob
{
    #region Methods

    /// <summary>
    /// Executes the database work of this job
    /// </summary>
    /// <returns>A task representing the asynchronous operation</returns>
    Task ExecuteAsync();

    #endregion // Methods
}