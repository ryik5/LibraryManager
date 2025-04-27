using LibraryManager.AbstractObjects;
using LibraryManager.Models;
using LibraryManager.Utils;
using LibraryManager.Views;

namespace LibraryManager.ViewModels;

/// <author>YR 2025-02-09</author>
public sealed class LibraryViewModel : AbstractBookViewModel, IRefreshable
{
    public LibraryViewModel(ILibrary library, SettingsViewModel settings, IStatusBar statusBar) 
        : base(library, statusBar)
    {
        _settings = settings;
        _libraryManager = new LibraryManagerModel(Library, statusBar);
    }


    #region Public Methods
    protected override async Task PerformAction(string? commandParameter)
    {
        await ShowNavigationCommandInDebug(commandParameter, nameof(LibraryPage));

        if (string.IsNullOrWhiteSpace(commandParameter))
            return;

        if (IsCurrentRoute(nameof(LibraryPage)))
        {
            switch (commandParameter)
            {
                case nameof(AboutPage):
                case nameof(BooksPage):
                case nameof(FindBooksPage):
                case nameof(ToolsPage):
                {
                    await TryGoToPage(commandParameter);
                    break;
                }
                case Constants.LIBRARY_NEW:
                {
                    if (await HasLibraryHashCodeChanged())
                    {
                        var success =
                            await _libraryManager.TrySaveLibrary(new XmlLibraryKeeper(), GetPathToCurrentLibraryFile());
                        if (!success)
                            return;
                    }

                    await _libraryManager.CreateNewLibrary();
                    await UpdateLibraryHashCode();
                    break;
                }
                case Constants.LIBRARY_LOAD:
                {
                    if (await HasLibraryHashCodeChanged())
                    {
                        var success =
                            await _libraryManager.TrySaveLibrary(new XmlLibraryKeeper(), GetPathToCurrentLibraryFile());
                        if (!success)
                            return;
                    }

                    await _libraryManager.RunCommand(commandParameter);
                    await UpdateLibraryHashCode();
                    await UpdateControlsOnChangeID();
                    break;
                }
                case Constants.LIBRARY_SAVE:
                {
                    var res = await _libraryManager.TrySaveLibrary(new XmlLibraryKeeper(),
                        GetPathToCurrentLibraryFile());
                    if (res)
                        await UpdateLibraryHashCode();
                    break;
                }
                case Constants.LIBRARY_SAVE_WITH_NEW_NAME:
                {
                    // display window with input a new library name
                    var castomDialog = await ShowCustomDialogPage(Constants.LIBRARY_SAVE_WITH_NEW_NAME,
                        Constants.LIBRARY_NAME, true);

                    var libName = castomDialog.IsOk && !string.IsNullOrEmpty(castomDialog.InputString)
                        ? castomDialog.InputString
                        : Library.Id.ToString();

                    if (await _libraryManager.TrySaveLibrary(new XmlLibraryKeeper(), GetPathToFile(libName)))
                        await UpdateLibraryHashCode();
                    break;
                }
                case Constants.LIBRARY_CLOSE:
                {
                    if (await HasLibraryHashCodeChanged())
                    {
                        var success =
                            await _libraryManager.TrySaveLibrary(new XmlLibraryKeeper(), GetPathToCurrentLibraryFile());
                        if (!success)
                            return;
                    }

                    await _libraryManager.RunCommand(commandParameter);
                    await UpdateLibraryHashCode();

                    HandleCloseLibrary();
                    break;
                }
                default:
                {
                    // Performs other actions at the LibraryManager
                    await _libraryManager.RunCommand(commandParameter);
                    break;
                }
            }
        }
        else
        {
            await ShowNavigationErrorInDebug(commandParameter, nameof(FindBooksViewModel));
        }
    }

    private async Task HandleCloseLibrary()
    {
        await Task.Delay(200);
        CanOperateWithBooks = false;
        CanCloseLibrary = false;
    }

    protected override async Task RefreshControlsOnAppearing()
    {
        await UpdateControlsOnChangeID();
        RaisePropertyChanged(nameof(Library));
        RaisePropertyChanged(nameof(Library.TotalBooks));
    }
    #endregion

    #region Public Properties
    /// <summary>
    /// Gets or sets a value indicating whether the book can be edited.
    /// </summary>
    public bool CanCloseLibrary
    {
        get => _canCloseLibrary;
        set => SetProperty(ref _canCloseLibrary, value);
    }
    #endregion

    #region Private methods
    private async Task UpdateControlsOnChangeID()
    {
        await Task.Delay(20);
        await RunInMainThreadAsync(() =>
        {
            // Library.TotalBooks = Library.BookList.Count;
            CanOperateWithBooks = IsValidLibrary && IsNotEmptyLibrary;
            CanCloseLibrary = IsValidLibrary;
        });
    }

    /// <summary>
    /// Updates the library state by raising a property changed event for the Library property
    /// and setting the IsEnabled property based on whether the Library ID differs from the default value of 0.
    /// </summary>
    private Task UpdateLibraryHashCode()
    {
        _libraryHashCode = Library.GetHashCode();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Checks if the library hash code has changed and prompts the user to save changes if necessary.
    /// </summary>
    /// <returns>True if the user confirms saving changes, false otherwise.</returns>
    private async Task<bool> HasLibraryHashCodeChanged()
    {
        if (!IsValidLibrary || !IsNotEmptyLibrary)
            return false;

        var currentHash = Library.GetHashCode();
        var libraryChanged = currentHash != 0 && currentHash != _libraryHashCode;

        if (libraryChanged)
        {
            var res = await ShowCustomDialogPage(Constants.LIBRARY_SAVE, StringsHandler.LibraryChangedMessage());
            return res.IsOk;
        }

        return false;
    }


    private string GetPathToCurrentLibraryFile() => GetPathToFile(Library.Id.ToString());
    #endregion
    
    #region Private Members
    private readonly SettingsViewModel _settings;
    private readonly ILibraryManageable _libraryManager;
    private int _libraryHashCode;
    private bool _canCloseLibrary;
    #endregion
}