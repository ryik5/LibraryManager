namespace LibraryManager.Models;

/// <summary>
/// Saver of the library as XML file on the local disk.
/// <author>YR 2025-01-09</author>
public class XmlLibraryKeeper : ILibraryKeeper
{
    /// <summary>
    /// Saves the library to an XML file at the specified path.
    /// </summary>
    /// <param name="library">The instance of the library to save.</param>
    /// <param name="pathToFile">The path to the file where the library will be saved.</param>
    /// <returns>True if the library was saved successfully; otherwise, false.</returns>
    public bool TrySaveLibrary(ILibrary? library, string pathToFile)
    {
        try
        {
            XmlObjectSerializer.Save(library as Library, pathToFile);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
