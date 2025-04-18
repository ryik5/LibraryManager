namespace LibraryManager.Models;

/// <summary>
/// Represents the result of an input operation.
/// </summary>
/// <author>YR 2025-03-09</author>
public class ResultInput
{
    public ResultInput(bool isOk, string? inputString = null)
    {
        IsOk = isOk;
        InputString = inputString;
    }

    public bool IsOk { get; private set; }
    public string? InputString { get; private set; }
}