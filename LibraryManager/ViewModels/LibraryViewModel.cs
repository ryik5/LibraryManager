using System.Diagnostics;
using LibraryManager.AbstractObjects;
using LibraryManager.Models;
using LibraryManager.Utils;
using LibraryManager.Views;

namespace LibraryManager.ViewModels;

public class LibraryViewModel : AbstractViewModel, IDisposable, IRefreshable
{
    public LibraryViewModel(ILibrary library)
    {
        Library = library; // Constructor injection ensures proper dependency handling
        Library.TotalBooksChanged += BookList_CollectionChanged;
        Library.LibraryIdChanged += Handle_LibraryIdChanged;
        _libraryManager = new LibraryManagerModel(Library);

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

    public bool CanOperateWithBooks
    {
        get => _canOperateWithBooks;
        set => SetProperty(ref _canOperateWithBooks, value);
    }

    public Binding LibraryControlsView { get; }
    public event EventHandler<TotalBooksEventArgs> TotalBooksChanged;
    #endregion


    #region Public Methods
    /*    Match macOS behavior on Mac Catalyst
       If you need to match macOS app behavior and use the same system paths on Mac Catalyst, the recommended way of obtaining such paths is shown below:
           Environment.SpecialFolder.ApplicationData
           Instead of Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.None), use new NSFileManager().GetUrls(NSSearchPathDirectory.ApplicationSupportDirectory, NSSearchPathDomain.User)[0].Path.
           Environment.SpecialFolder.Desktop
           Instead of Environment.GetFolderPath(Environment.SpecialFolder.Desktop, Environment.SpecialFolderOption.None), use new NSFileManager().GetUrls(NSSearchPathDirectory.DesktopDirectory, NSSearchPathDomain.User)[0].Path.
           Environment.SpecialFolder.DesktopDirectory
           Instead of Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory, Environment.SpecialFolderOption.None), use new NSFileManager().GetUrls(NSSearchPathDirectory.DesktopDirectory, NSSearchPathDomain.User)[0].Path.
           Environment.SpecialFolder.Fonts
           Instead of Environment.GetFolderPath(Environment.SpecialFolder.Fonts, Environment.SpecialFolderOption.None), use Path.Combine(new NSFileManager().GetUrls(NSSearchPathDirectory.LibraryDirectory, NSSearchPathDomain.User)[0].Path, "Fonts").
           Environment.SpecialFolder.LocalApplicationData
           Instead of Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.None), use new NSFileManager().GetUrls(NSSearchPathDirectory.ApplicationSupportDirectory, NSSearchPathDomain.User)[0].Path.
           Environment.SpecialFolder.MyMusic
           Instead of Environment.GetFolderPath(Environment.SpecialFolder.MyMusic, Environment.SpecialFolderOption.None), use new NSFileManager().GetUrls(NSSearchPathDirectory.MusicDirectory, NSSearchPathDomain.User)[0].Path.
           Environment.SpecialFolder.MyPictures
           Instead of Environment.GetFolderPath(Environment.SpecialFolder.MyPictures, Environment.SpecialFolderOption.None), use new NSFileManager().GetUrls(NSSearchPathDirectory.PicturesDirectory, NSSearchPathDomain.User)[0].Path.
           Environment.SpecialFolder.MyVideos
           Instead of Environment.GetFolderPath(Environment.SpecialFolder.MyVideos, Environment.SpecialFolderOption.None), use new NSFileManager().GetUrls(NSSearchPathDirectory.MoviesDirectory, NSSearchPathDomain.User)[0].Path.
           Environment.SpecialFolder.ProgramFiles
           Instead of Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles, Environment.SpecialFolderOption.None), use "/Applications".
           Environment.SpecialFolder.System
           Instead of Environment.GetFolderPath(Environment.SpecialFolder.System, Environment.SpecialFolderOption.None), use "/System".
           Environment.SpecialFolder.Templates
           Instead of Environment.GetFolderPath(Environment.SpecialFolder.Templates, Environment.SpecialFolderOption.None), use Path.Combine(NSFileManager.HomeDirectory, "Templates").
     */

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
                    if (await _libraryManager.TrySaveLibrary(new XmlLibraryKeeper(), GetPathToCurrentLibraryFile()))
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
                CanOperateWithBooks = ValidLibrary();
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

    public void RefreshControls()
    {
        RaisePropertyChanged(nameof(Library));
        RaisePropertyChanged(nameof(Library.BookList));
        CanOperateWithBooks = ValidLibrary();
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
    /// Handles the LibraryIdChanged event by updating the CanOperateWithBooks property.
    /// </summary>
    private void Handle_LibraryIdChanged(object? sender, EventArgs e)
    {
        CanOperateWithBooks = ValidLibrary();
    }

    private void BookList_CollectionChanged(object? sender, TotalBooksEventArgs e)
    {
        RaisePropertyChanged(nameof(Library));
        RaisePropertyChanged(nameof(Library.TotalBooks));
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

    private bool ValidLibrary() => Library?.Id != 0;
    #endregion


    #region Private Members
    private ILibrary _library;
    private bool _canOperateWithBooks = true;
    private readonly ILibraryManageable _libraryManager;
    private bool _disposed; // Safeguard for multiple calls to Dispose.
    private int _libraryHashCode;
    #endregion
}