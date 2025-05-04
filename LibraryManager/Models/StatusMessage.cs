using LibraryManager.AbstractObjects;

namespace LibraryManager.Models;

/// <author>YR 2025-03-09</author>
public class StatusMessage
{
    public EInfoKind InfoKind { get; set; }

    public ELogLevel LogLevel { get; set; } = ELogLevel.Info;
    
    public string Message { get; set; }
}
