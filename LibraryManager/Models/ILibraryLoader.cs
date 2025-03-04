namespace LibraryManager.Models;

/// <summary>
/// Defines an interface for loading a library.
/// </summary>
/// <author>YR 2025-01-09</author>
public interface ILibraryLoader : ILoadable
{
    /// <summary>
    /// Loads a library from the specified path.
    /// </summary>
    /// <param name="pathToLibrary">The path to the library.</param>
    /// <param name="library">The loaded library instance.</param>
    /// <returns>True if the library was successfully loaded; otherwise, false.</returns>
    bool TryLoadLibrary(string pathToLibrary, out ILibrary? library);

    event EventHandler<ActionFinishedEventArgs> LoadingFinished;

}
