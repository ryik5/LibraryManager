namespace LibraryManager.Models;

/// <summary>
/// Loader of the library from XML file stored on a local disk.
/// </summary>
/// <author>YR 2025-01-09</author>
public class XmlLibraryLoader : ILibraryLoader
{
    /// <summary>
    /// Loads the library from the specified XML file.
    /// </summary>
    /// <param name="pathToLibrary">The path to the XML file containing the library data.</param>
    /// <param name="library">The loaded library instance.</param>
    /// <returns>True if the library was successfully loaded; otherwise, false.</returns>
    public bool TryLoadLibrary(string pathToLibrary, out ILibrary? library)
    {
        var result = false;
        library = null;
        var msg = string.Empty;
        LoadingFinished?.Invoke(this, new ActionFinishedEventArgs { Message = "Loading started", IsFinished = false });
        try
        {
            library = XmlSerializer.Load<ILibrary>(pathToLibrary);
            result = true;
            msg = "Library loaded";
        }
        catch
        {
            result = false;
            msg = "Library was not loaded";
        }

        LoadingFinished?.Invoke(this, new ActionFinishedEventArgs { Message = msg, IsFinished = result });
        return result;
    }

    public event EventHandler<ActionFinishedEventArgs>? LoadingFinished;
}
