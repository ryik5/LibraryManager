namespace LibraryManager.Models;

/// <summary>
/// Interface for saving a library.
/// </summary>
/// <author>YR 2025-01-09</author>
public interface ILibraryKeeper
{
    /// <summary>
    /// Saves the library to the specified place.
    /// </summary>
    /// <param name="library">The library to save.</param>
    /// <param name="selectedPlace">The place where the library will be saved.</param>
    /// <returns>true if the library was successfully saved; otherwise, false.</returns>
    bool TrySaveLibrary(ILibrary? library, string selectedPlace);
}
