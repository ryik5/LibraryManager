namespace LibraryManager.AbstractObjects;

/// <summary>
/// Specifies the type of information to be displayed.
/// </summary>
/// <author>YR 2025-01-28</author>
public enum EInfoKind
{
    /// <summary>
    /// Common message.
    /// </summary>
    CommonInfo,

    /// <summary>
    /// Debug message. It shouldn't be shown to the user.
    /// </summary>
    CurrentInfo,

    /// <summary>
    /// Total number of books in the current librery.
    /// </summary>
    TotalBooks
}