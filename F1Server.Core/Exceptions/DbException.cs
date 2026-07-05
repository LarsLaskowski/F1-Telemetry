namespace F1Server.Core.Exceptions;

/// <summary>
/// Database exception class
/// </summary>
public class DbException : Exception
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="message">Exception message</param>
    public DbException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="innerException">Inner exception object</param>
    public DbException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    #endregion // Constructors
}