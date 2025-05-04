using LibraryManager.AbstractObjects;

namespace LibraryManager.Models;

/// <author>YR 2025-03-09</author>
public class IndexedString
{
    public string TimeStamp { get; set; }
    
    public ELogLevel LogLevel { get; set; }
    
    public string Message { get; set; }
}