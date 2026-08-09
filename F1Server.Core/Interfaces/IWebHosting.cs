namespace F1Server.Core.Interfaces;

/// <summary>
/// Abstraction of the web hosting used to serve the web api and the frontend. It allows lower layers such as
/// F1Server.Service to start and stop the hosting without referencing the hosting implementation itself
/// </summary>
public interface IWebHosting : IDisposable
{
    #region Properties

    /// <summary>
    /// Web hosting is running?
    /// </summary>
    bool IsRunning { get; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Start web hosting
    /// </summary>
    /// <param name="serviceProvider">A <see cref="IServiceProvider"/> containing the configured services</param>
    void StartWebHosting(IServiceProvider serviceProvider);

    /// <summary>
    /// Stop web hosting
    /// </summary>
    /// <returns>Status</returns>
    bool StopWebHosting();

    #endregion // Methods
}