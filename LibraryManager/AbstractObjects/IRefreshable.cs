namespace LibraryManager.AbstractObjects;

/// <summary>
/// Represents an interface for objects that can refresh their controls when appearing.
/// </summary>
/// <author>YR 2025-03-09</author>
public interface IRefreshable
{
    /// <summary>
    /// Refreshes the controls when the object appears.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RefreshControlsOnAppearing();
}