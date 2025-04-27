using LibraryManager.Models;
using LibraryManager.ViewModels;

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
    string TotalBooksInfo { get; }

    /// <summary>
    /// Gets a list of debug information strings.
    /// </summary>
    List<IndexedString> DebugInfo { get; }
}