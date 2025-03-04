namespace LibraryManager.Models;

/// <summary>
/// Loader of the Book from XML file stored on a local disk.
/// </summary>
/// <author>YR 2025-01-09</author>
public class XmlBookLoader : IBookLoader
{
    public bool TryLoadBook(string pathToBook, out Book? book)
    {
        var result = false;
        book = null;
        var msg = string.Empty;
        LoadingFinished?.Invoke(this, new ActionFinishedEventArgs { Message = "Loading started", IsFinished = false });

        try
        {
            book = XmlObjectSerializer.Load<Book>(pathToBook);
            result = true;
            msg = "Book loaded";
        }
        catch
        {
            result = false;
            msg = "Book was not loaded";
        }
        LoadingFinished?.Invoke(this, new ActionFinishedEventArgs { Message = msg, IsFinished = result });

        return result;
    }

    public event EventHandler<ActionFinishedEventArgs>? LoadingFinished;
}
