namespace F1Server.WebApi.Core;

/// <summary>
/// Timer manager
/// </summary>
public class TimerManager : IDisposable
{
    #region Constants

    /// <summary>
    /// Delay in milliseconds before the timer callback is invoked for the first time
    /// </summary>
    private const int TimerDueTime = 1000;

    /// <summary>
    /// Interval in milliseconds between two timer callbacks
    /// </summary>
    private const int TimerPeriod = 1000;

    #endregion // Constants

    #region Fields

    /// <summary>
    /// Guards the timer state against concurrent access
    /// </summary>
    private readonly Lock _lock = new();

    /// <summary>
    /// Timer invoking the configured action periodically
    /// </summary>
    private Timer? _timer;

    /// <summary>
    /// Reset event handed over to the timer callback as state object
    /// </summary>
    private AutoResetEvent? _autoResetEvent;

    /// <summary>
    /// Action executed on every timer tick
    /// </summary>
    private Action? _action;

    /// <summary>
    /// Has the timer manager already been disposed?
    /// </summary>
    private bool _disposed;

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
    /// Prepare the timer, the timer is started only once, further calls are ignored while it is running
    /// </summary>
    /// <param name="action">Action</param>
    public void PrepareTimer(Action action)
    {
        lock (_lock)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            if (IsTimerStarted)
            {
                return;
            }

            _action = action;
            _autoResetEvent = new AutoResetEvent(false);
            _timer = new Timer(Execute, _autoResetEvent, TimerDueTime, TimerPeriod);

            TimerStarted = DateTime.Now;

            IsTimerStarted = true;
        }
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
            lock (_lock)
            {
                if (_disposed)
                {
                    return;
                }

                _timer?.Dispose();
                _timer = null;

                _autoResetEvent?.Dispose();
                _autoResetEvent = null;

                _action = null;

                IsTimerStarted = false;

                _disposed = true;
            }
        }
    }

    #endregion // IDisposable
}