namespace F1PacketTester;

/// <summary>
/// Renders a single line spinner with a status text for operations without a known total
/// </summary>
internal sealed class ConsoleSpinner
{
    #region Fields

    /// <summary>
    /// Animation frames cycled through on every update
    /// </summary>
    private static readonly char[] _frames = ['|', '/', '-', '\\'];

    /// <summary>
    /// Label shown in front of the spinner
    /// </summary>
    private readonly string _label;

    /// <summary>
    /// Index of the currently shown animation frame
    /// </summary>
    private int _frameIndex;

    /// <summary>
    /// Length of the last rendered line, used to overwrite leftover characters
    /// </summary>
    private int _lastLength;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleSpinner"/> class
    /// </summary>
    /// <param name="label">Label shown in front of the spinner</param>
    public ConsoleSpinner(string label)
    {
        _label = label;
        _frameIndex = 0;
        _lastLength = 0;
    }

    #endregion // Constructors

    #region Methods

    /// <summary>
    /// Advances the spinner animation and updates the status text in place
    /// </summary>
    /// <param name="status">Status text shown next to the spinner</param>
    public void Update(string status)
    {
        var frame = _frames[_frameIndex % _frames.Length];

        _frameIndex++;

        var visible = $"{_label} {frame} {status}";

        Console.Write('\r' + visible.PadRight(_lastLength));

        _lastLength = visible.Length;
    }

    /// <summary>
    /// Replaces the spinner with a final status text and moves to the next line
    /// </summary>
    /// <param name="status">Status text shown as result</param>
    public void Complete(string status)
    {
        var visible = $"{_label} {status}";

        Console.Write('\r' + visible.PadRight(_lastLength));
        Console.WriteLine();

        _lastLength = visible.Length;
    }

    #endregion // Methods
}