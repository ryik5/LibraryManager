namespace LibraryManager.Models;

/// <summary>
/// Represents the result of a book-related operation.
/// </summary>
/// <author>YR 2025-03-09</author>
public class ResultBook
{
    public Book? Book { get; set; }
    public bool IsSuccess { get; set; }
}