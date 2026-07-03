namespace F1Server.WebApi.Core;

/// <summary>
/// Timer manager
/// </summary>
public class TimerManager : IDisposable
{
    #region Fields

    private Timer? _timer;
    private AutoResetEvent? _autoResetEvent;
    private Action? _action;

    #endregion // Fields

    #region Properties

    /// <summary>
    /// Timestamp staring the timer
    /// </summary>
    public DateTime TimerStarted { get; private set; }

    /// <summary>
    /// Is timer running?
    /// </summary>
    public bool IsTimerStarted { get; private set; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Prepare the timer
    /// </summary>
    /// <param name="action">Action</param>
    public void PrepareTimer(Action action)
    {
        _action = action;
        _autoResetEvent = new AutoResetEvent(false);
        _timer = new Timer(Execute, _autoResetEvent, 1000, 1000);

        TimerStarted = DateTime.Now;

        IsTimerStarted = true;
    }

    /// <summary>
    /// Timer action execution
    /// </summary>
    /// <param name="stateInfo">State info</param>
    public void Execute(object? stateInfo)
    {
        _action?.Invoke();
    }

    #endregion // Methods

    #region IDisposable

    /// <summary>
    /// Dispose method
    /// </summary>
    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Internal dispose method
    /// </summary>
    /// <param name="disposing">disposing</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _autoResetEvent?.Dispose();
            _timer?.Dispose();
        }
    }

    #endregion // IDisposable
}