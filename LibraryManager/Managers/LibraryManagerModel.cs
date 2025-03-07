using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace LibraryManager.Models;

/// <summary>
/// Represents a model for managing a library of books.
/// </summary>
/// <author>YR 2025-01-09</author>
public class LibraryManagerModel : INotifyPropertyChanged, ILibraryManageable
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
    
    public Task RunCommand(string commandParameter)
    {
        switch (commandParameter)
        {
            case Constants.CREATE_NEW_LIBRARY:
                break;
            
            default:
                break;
        }
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Creates a new library with the specified ID.
    /// </summary>
    /// <param name="idLibrary">The ID of the new library.</param>
    public void CreateNewLibrary(int idLibrary)
    {
        TryCloseLibrary();

        Library.Id = idLibrary;
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
        InvokeOnUiThread(() =>
        {
            result = libraryLoader.TryLoadLibrary(pathToLibrary, out var library);
            if (result)
            {
                Library.Set(library);
            }
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
    public bool TrySaveLibrary(ILibraryKeeper keeper, string pathToStorage) => keeper.TrySaveLibrary(Library, pathToStorage);

    /// <summary>
    /// Closes the current library.
    /// </summary>
    public void TryCloseLibrary()
    {
        if (0 < Library.BookList.Count)
            InvokeOnUiThread(() => Library.BookList.Clear());

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

    /// <summary>
    /// Invokes the specified action on the UI thread.
    /// </summary>
    /// <param name="action">The action to invoke.</param>
    private void InvokeOnUiThread(Action action) => MainThread.BeginInvokeOnMainThread(() => action());


    private ILibrary? _library;
    #endregion
    
    
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        RaisePropertyChanged(propertyName);
        return true;
    }
}
