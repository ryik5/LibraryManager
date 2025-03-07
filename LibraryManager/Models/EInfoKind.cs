namespace LibraryManager.Models;

/// <summary>
/// Specifies the type of information to be displayed.
/// </summary>
/// <author>YR 2025-01-28</author>
public enum EInfoKind
{
    /// <summary>
    /// Total number of books in the current librery.
    /// </summary>
    TotalBooks,
    /// <summary>
    /// Common message.
    /// </summary>
    CommonMessage,
    /// <summary>
    /// Debug message. It shouldn't be shown to the user.
    /// </summary>
    DebugMessage
}