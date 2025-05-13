using CommunityToolkit.Mvvm.Messaging;
using System.Xml;
using LibraryManager.AbstractObjects;

namespace LibraryManager.Models;

/// <summary>
/// Represents a model for managing a library of books.
/// </summary>
/// <author>YR 2025-01-09</author>
public class LibraryManagerModel : AbstractBindableModel, ILibraryManageable
{
    public LibraryManagerModel(ILibrary? library, IStatusBar statusBar, IPopupService popupService)
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

        _statusBar = statusBar;
        _popupService = popupService;
    }


    #region public methods
    public async Task RunCommand(string commandParameter)
    {
        switch (commandParameter)
        {
            case Constants.LIBRARY_NEW:
                break;
        }
    }


    public async Task<bool> TryLoadLibrary()
    {
        var result = await TryPickFileUpTask("Please select a library file", new string[] { "xml" });
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

        WeakReferenceMessenger.Default.Send(new StatusMessage()
            { InfoKind = EInfoKind.CurrentInfo, Message = $"Created new library with ID:{Library.Id}." });
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
    public Task TryCloseLibrary()
    {
        if (0 < Library.TotalBooks)
            RunInMainThread(() => Library.BookList.Clear());

        var id= Library.Id;
        Library.Name = string.Empty;
        Library.Description = string.Empty;
        Library.Id = 0;
        WeakReferenceMessenger.Default.Send(new StatusMessage()
            { InfoKind = EInfoKind.CurrentInfo, Message = $"Library with ID:{id} was closed." });
        return Task.CompletedTask;
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


    #region Private fields
    private ILibrary? _library;
    private readonly IStatusBar _statusBar;
    private readonly IPopupService _popupService;
    #endregion
}