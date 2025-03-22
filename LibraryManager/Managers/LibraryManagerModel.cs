using System.Xml;
using LibraryManager.AbstractObjects;

namespace LibraryManager.Models;

/// <summary>
/// Represents a model for managing a library of books.
/// </summary>
/// <author>YR 2025-01-09</author>
public class LibraryManagerModel : AbstractBindableModel, ILibraryManageable
{
    public LibraryManagerModel(ILibrary? library)
    {
        if (library is null)
            throw new ArgumentNullException(nameof(library));

        if (Library is ILibrary)
        {
            Library.Set(library);
        }
        else
        {
            _library = library;
        }
    }


    #region public methods
    public async Task RunCommand(string commandParameter)
    {
        switch (commandParameter)
        {
            case Constants.LIBRARY_NEW:
                break;

            case Constants.LIBRARY_EDIT:
                break;

            case Constants.LIBRARY_LOAD:
                break;

            case Constants.LIBRARY_SAVE:
                break;

            case Constants.LIBRARY_CLOSE:
                break;

            case Constants.LIBRARY_SAVE_WITH_NEW_NAME:
                break;

            default:
                break;
        }
    }

    public async Task<bool> TryLoadLibrary()
    {
        return await TryLoadLibrary(await TryPickFileUpTask("Please select a library file", new string[] { "xml" }));
    }


    private async Task<bool> TryLoadLibrary(FileResult result)
    {
        try
        {
            await using var fileStream = await result.OpenReadAsync();
            using var xmlReader = XmlReader.Create(fileStream, new XmlReaderSettings
            {
                IgnoreWhitespace = false,
                IgnoreComments = false
            });

            var lib = await Task.Run(() =>
                (Library)(new System.Xml.Serialization.XmlSerializer(typeof(Library))).Deserialize(xmlReader));

            await SetLibrary(lib);
        }
        catch (Exception e)
        {
            return false;
        }

        return true;
    }

    private Task SetLibrary(ILibrary lib)
    {
        RunInMainThread(() => Library.Set(lib));
        return Task.CompletedTask;
    }


    /// <summary>
    /// Creates a new library with the specified ID.
    /// </summary>
    public Task CreateNewLibrary()
    {
        TryCloseLibrary();

        Library.Id = new Random().Next();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Loads a library from the specified path.
    /// </summary>
    /// <param name="libraryLoader">The loader responsible for loading the library.</param>
    /// <param name="pathToLibrary">The path to the storage containing the library data.</param>
    /// <returns>True if the library was successfully loaded; otherwise, false.</returns>
    public bool TryLoadLibrary(ILibraryLoader libraryLoader, string pathToLibrary)
    {
        libraryLoader.LoadingFinished += LibraryLoader_LoadingLibraryFinished;

        TryCloseLibrary();

        var result = false;
        RunInMainThread(() =>
        {
            /*result = libraryLoader.TryLoadLibrary(pathToLibrary, out var library);
            if (result)
            {
                Library.Set(library);
            }*/
        });

        libraryLoader.LoadingFinished -= LibraryLoader_LoadingLibraryFinished;

        return result;
    }

    /// <summary>
    /// Saves the specified library to the specified folder.
    /// </summary>
    /// <param name="keeper">The keeper responsible for saving the library.</param>
    /// <param name="pathToStorage">The path to the storage where the library will be saved.</param>
    /// <returns>True if the library was successfully saved; otherwise, false.</returns>
    public Task<bool> TrySaveLibrary(ILibraryKeeper keeper, string pathToStorage) =>
        keeper.TrySaveLibrary(Library, pathToStorage);

    /// <summary>
    /// Closes the current library.
    /// </summary>
    public void TryCloseLibrary()
    {
        if (0 < Library.TotalBooks)
            RunInMainThread(() => Library.BookList.Clear());

        Library.Name = string.Empty;
        Library.Description = string.Empty;
        Library.Id = 0;
    }
    #endregion


    #region Properties
    /// <summary>
    /// Gets or sets a library.
    /// </summary>
    public ILibrary? Library
    {
        get => _library;
        set => SetProperty(ref _library, value);
    }

    public event EventHandler<ActionFinishedEventArgs> LoadingFinished;
    #endregion


    #region private methods
    /// <summary>
    /// Handles the LoadingFinished event of the LibraryLoader.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The ActionFinishedEventArgs instance containing the event data.</param>
    private void LibraryLoader_LoadingLibraryFinished(object? sender, ActionFinishedEventArgs e)
    {
        LoadingFinished?.Invoke(this, new ActionFinishedEventArgs { Message = e.Message, IsFinished = e.IsFinished });
    }
    #endregion


    #region Private fields
    private ILibrary? _library;
    #endregion
}