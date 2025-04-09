namespace LibraryManager.AbstractObjects;

/// <summary>
/// Represents an interface for a status bar, providing properties for common, current, and status information.
/// </summary>
/// <author>YR 2025-03-28</author>
public interface IStatusBar
{
    /// <summary>
    /// Gets the common information displayed in the start of the status bar.
    /// </summary>
    string CommonInfo { get; }

    /// <summary>
    /// Gets the current information displayed in the middle of the status bar.
    /// </summary>
    string CurrentInfo { get; }

    /// <summary>
    /// Gets the status information displayed in the end of the status bar.
    /// </summary>
    string StatusInfo { get; }

    /// <summary>
    /// Sets the status message based on the provided information kind and message.
    /// </summary>
    /// <param name="infoKind">The type of information to be displayed.</param>
    /// <param name="message">The message to be displayed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetStatusMessage(EInfoKind infoKind, string message);

    /// <summary>
    /// Sets the status message based on the provided information kind and total books.
    /// </summary>
    /// <param name="infoKind">The type of information to be displayed.</param>
    /// <param name="totalBooks">The total number of books.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetStatusMessage(EInfoKind infoKind, int totalBooks);
}