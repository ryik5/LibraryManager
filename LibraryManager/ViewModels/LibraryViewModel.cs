using System.Diagnostics;
using LibraryManager.AbstractObjects;
using LibraryManager.Models;
using LibraryManager.Utils;
using LibraryManager.Views;

namespace LibraryManager.ViewModels;

/// <author>YR 2025-01-09</author>
public class LibraryViewModel : AbstractViewModel, IDisposable, IRefreshable
{
    public LibraryViewModel(ILibrary library, SettingsViewModel settings, IStatusBar statusBar)
    {
        _settings = settings;
        StatusBar = statusBar;
        Library = library; // Constructor injection ensures proper dependency handling

        Library.TotalBooksChanged += Handle_TotalBooksChanged;
        Library.LibraryIdChanged += Handle_LibraryIdChanged;
        _libraryManager = new LibraryManagerModel(Library,statusBar);
        CanOperateWithLibrary = ValidLibrary();
        /*MessagingCenter.Subscribe<BooksViewModel>(this, "Navigate", async (sender) =>
        {
            Console.WriteLine("Received navigation request from BooksViewModel.");
           // await PerformAction("CreateLibrary").ConfigureAwait(false);
        });*/
    }


    #region Public properties
    public ILibrary Library
    {
        get => _library;
        set => SetProperty(ref _library, value);
    }

    public bool CanOperateWithLibrary
    {
        get => _canOperateWithLibrary;
        set => SetProperty(ref _canOperateWithLibrary, value);
    }

    public IStatusBar StatusBar
    {
        get => _statusBar;
        set => SetProperty(ref _statusBar, value);
    }
    #endregion


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

                    await _libraryManager.TryLoadLibrary();
                    await UpdateLibraryHashCode();
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

                    _libraryManager.TryCloseLibrary();
                    await UpdateLibraryHashCode();
                    break;
                }
                default:
                {
                    // Performs other actions at the LibraryManager
                    await _libraryManager.RunCommand(commandParameter);
                    break;
                }
            }

            await Task.Run(() => RunInMainThread(() =>
            {
                CanOperateWithLibrary = ValidLibrary();
                Library.TotalBooks = Library.BookList.Count;
                RaisePropertyChanged(nameof(Library));
                RaisePropertyChanged(nameof(Library.BookList));
            }));
        }
        else
        {
            await ShowNavigationErrorInDebug(commandParameter, nameof(FindBooksViewModel));
        }
    }

    public async Task RefreshControlsOnAppearing()
    {
        CanOperateWithLibrary = ValidLibrary();
        RaisePropertyChanged(nameof(Library));
        RaisePropertyChanged(nameof(Library.TotalBooks));
        RaisePropertyChanged(nameof(CanOperateWithLibrary));
    }


    // Dispose method for external calls
    public void Dispose()
    {
        if (_disposed) return; // Safeguard against multiple Dispose calls.
        _disposed = true;

        // MessagingCenter cleanup
        #if DEBUG
        Debug.WriteLine("Cleaning up MessagingCenter resources in LibraryViewModel.");
        #endif

        MessagingCenter.Unsubscribe<BooksViewModel>(this, "Navigate");
    }

    ~LibraryViewModel()
    {
        Dispose(); // Safeguard cleanup in destructor (if proper disposal is skipped)
    }
    #endregion


    #region Private methods
    /// <summary>
    /// Handles the LibraryIdChanged event by updating the CanOperateWithLibrary property.
    /// </summary>
    private void Handle_LibraryIdChanged(object? sender, EventArgs e)
    {
        CanOperateWithLibrary = ValidLibrary();
    }

    private async void Handle_TotalBooksChanged(object? sender, TotalBooksEventArgs e)
    {
        RefreshControlsOnAppearing();
        await StatusBar.SetStatusMessage(EInfoKind.TotalBooks, Library.TotalBooks);
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
        if (Library.TotalBooks == 0 || !ValidLibrary())
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

    private bool ValidLibrary() => Library.Id != 0;
    #endregion


    #region Private Members
    private ILibrary _library;
    private readonly SettingsViewModel _settings;
    private bool _canOperateWithLibrary = true;
    private readonly ILibraryManageable _libraryManager;
    private bool _disposed; // Safeguard for multiple calls to Dispose.
    private int _libraryHashCode;
    private IStatusBar _statusBar;
    #endregion
}