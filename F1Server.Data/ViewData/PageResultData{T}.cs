namespace F1Server.Data.ViewData;

/// <summary>
/// Represents a paginated result containing a collection of items and the total count of items
/// </summary>
/// <typeparam name="T">The type of items in the collection. Must be a reference type</typeparam>
public class PageResultData<T>
    where T : class
{
    #region Properties

    /// <summary>
    /// Gets or sets the collection of items
    /// </summary>
    public List<T> Items { get; set; } = new List<T>();

    /// <summary>
    /// Gets or sets the total count of items
    /// </summary>
    public int TotalCount { get; set; }

    #endregion // Properties
}