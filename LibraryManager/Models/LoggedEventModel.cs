namespace LibraryManager.Models;

/// <summary>
/// Represents a logged event with a specified log level, message, and timestamp.
/// </summary>
/// <author>YR 2025-02-05</author>
public sealed class LoggedEventModel
{
    /// <summary>
    /// Gets the log level of the event.
    /// </summary>
    public ELogLevel LogLevel
    {
        get;
    }

    /// <summary>
    /// Gets the message text.
    /// </summary>
    public string Message
    {
        get;
    }

    /// <summary>
    /// Gets the timestamp when the event was logged.
    /// </summary>
    public DateTime Timestamp
    {
        get;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggedEventModel"/> class with the specified log level and message.
    /// </summary>
    /// <param name="level">The log level of the event.</param>
    /// <param name="message">The message text.</param>
    public LoggedEventModel(ELogLevel level, string message)
    {
        LogLevel = level;
        Message = message;
        Timestamp = DateTime.Now;
    }

    public override string ToString()
    {
        return $"{Timestamp:yyyy-MM-dd HH:mm:ss} [{LogLevel}] {Message}";
    }
}
