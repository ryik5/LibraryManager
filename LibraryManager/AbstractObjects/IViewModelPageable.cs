namespace LibraryManager.Models;

/// <summary>
/// Represents a view model that supports pagination and selection.
/// </summary>
/// <author>YR 2025-02-04</author>
public interface IViewModelPageable
{
    /// <summary>
    /// Gets the name of this IViewModelPageable model.
    /// </summary>
    string Name
    {
        get;
    }

    /// <summary>
    /// Value indicating whether the current IViewModelPageable model was selected 
    /// among other IViewModelPageable classes
    /// </summary>
    bool IsChecked
    {
        get; set;
    }
}