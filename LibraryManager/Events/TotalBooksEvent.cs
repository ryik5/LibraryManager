namespace LibraryManager.Models;

/// <summary>
/// Represents event arguments for the total number of books.
/// </summary>
/// <author>YR 2025-01-28</author>
public class TotalBooksEventArgs : EventArgs
{
    /// <summary>
    /// Gets or sets the total number of books.
    /// </summary>
    public int TotalBooks
    {
        get; set;
    }
}

/// <summary>
/// Represents an event that is raised when the total number of books changes.
/// </summary>
/// <author>YR 2025-01-28</author>
public class TotalBooksEvent 
{
}