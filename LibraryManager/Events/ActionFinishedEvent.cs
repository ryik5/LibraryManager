namespace LibraryManager.Models;

/// <summary>
/// Represents event arguments for an action that has finished.
/// </summary>
/// <author>YR 2025-01-26</author>
public class ActionFinishedEventArgs : EventArgs
{
    /// <summary>
    /// Gets or sets a value indicating whether the action has finished.
    /// </summary>
    public bool IsFinished { get; set; }

    /// <summary>
    /// Gets or sets a message associated with the finished action.
    /// </summary>
    public string Message { get; set; }
}

/// <summary>
/// Represents an event that is raised when an action has finished.
/// </summary>
/// <author>YR 2025-01-26</author>
public class ActionFinishedEvent 
{
}