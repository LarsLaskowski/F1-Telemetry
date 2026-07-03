namespace F1PacketTester;

/// <summary>
/// Renders a single line console progress bar that is updated in place
/// </summary>
internal sealed class ConsoleProgressBar
{
    #region Constants

    /// <summary>
    /// Number of segments used to render the progress bar
    /// </summary>
    private const int BarWidth = 30;

    /// <summary>
    /// Character used to render a filled segment
    /// </summary>
    private const char FilledSegment = '█';

    /// <summary>
    /// Character used to render an empty segment
    /// </summary>
    private const char EmptySegment = '░';

    #endregion // Constants

    #region Fields

    /// <summary>
    /// Total number of work items to process
    /// </summary>
    private readonly int _total;

    /// <summary>
    /// Label shown in front of the progress bar
    /// </summary>
    private readonly string _label;

    /// <summary>
    /// Number of already processed work items
    /// </summary>
    private int _current;

    /// <summary>
    /// Length of the last rendered line, used to overwrite leftover characters
    /// </summary>
    private int _lastLength;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleProgressBar"/> class
    /// </summary>
    /// <param name="total">Total number of work items to process</param>
    /// <param name="label">Label shown in front of the progress bar</param>
    public ConsoleProgressBar(int total, string label)
    {
        _total = total;
        _label = label;
        _current = 0;
        _lastLength = 0;
    }

    #endregion // Constructors

    #region Methods

    /// <summary>
    /// Increments the progress by one work item and redraws the bar
    /// </summary>
    public void Increment()
    {
        _current++;

        Draw();
    }

    /// <summary>
    /// Writes a message on its own line without destroying the progress bar
    /// </summary>
    /// <param name="message">Message to write</param>
    public void WriteLine(string message)
    {
        Console.Write('\r' + message.PadRight(_lastLength));
        Console.WriteLine();

        _lastLength = 0;

        Draw();
    }

    /// <summary>
    /// Completes the progress bar and moves the cursor to the next line
    /// </summary>
    public void Complete()
    {
        Draw();

        Console.WriteLine();
    }

    /// <summary>
    /// Draws the current progress bar state in place
    /// </summary>
    private void Draw()
    {
        var fraction = _total > 0 ? (double)_current / _total : 1d;

        if (fraction > 1d)
        {
            fraction = 1d;
        }

        var filled = (int)(fraction * BarWidth);

        var bar = string.Concat(new string(FilledSegment, filled), new string(EmptySegment, BarWidth - filled));

        var visible = $"{_label} [{bar}] {_current}/{_total} ({fraction:P0})";

        Console.Write('\r' + visible.PadRight(_lastLength));

        _lastLength = visible.Length;
    }

    #endregion // Methods
}