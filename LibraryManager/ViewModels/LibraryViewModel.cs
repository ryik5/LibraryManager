using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using Foundation;
using LibraryManager.Models;
using LibraryManager.Utils;
using LibraryManager.Views;

namespace LibraryManager.ViewModels;

public class LibraryViewModel : AbstractViewModel, IDisposable
{
    public LibraryViewModel(ILibrary library)
    {
        Library = library; // Constructor injection ensures proper dependency handling
        Library.TotalBooksChanged += BookList_CollectionChanged;
        Library.LibraryIdChanged += Handle_LibraryIdChanged;
        _libraryManager = new LibraryManagerModel(Library);

        // Initialize the generic navigation command
        NavigateCommand = new AsyncRelayCommand<string>(PerformAction);

        /*MessagingCenter.Subscribe<BooksViewModel>(this, "Navigate", async (sender) =>
        {
            Console.WriteLine("Received navigation request from BooksViewModel.");
           // await PerformAction("CreateLibrary").ConfigureAwait(false);
        });*/
    }


    private void BookList_CollectionChanged(object? sender, TotalBooksEventArgs e)
    {
        RaisePropertyChanged(nameof(Library));
        RaisePropertyChanged(nameof(Library.TotalBooks));
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

    #region Public Methods

    protected override async Task PerformAction(string? commandParameter)
    {
        #if DEBUG
        Debug.WriteLine($"NavigateCommand on {nameof(LibraryPage)} triggered with commandParameter: {commandParameter}");
        #endif

        if (string.IsNullOrWhiteSpace(commandParameter))
            return;

        if (CurrentRoute == $"//{nameof(LibraryPage)}")
        {
            switch (commandParameter)
            {
                case nameof(AboutPage):
                case nameof(BooksPage):
                {
                    try
                    {
                        // Dynamically navigate using the provided commandParameter
                        // begins '//' added in the beginning to switch a Menu as well as Page. without '//' it switch only Page
                        await Shell.Current.GoToAsync($"//{commandParameter}").ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Navigation error: {ex.Message}");
                    }

                    break;
                }
                case Constants.CREATE_NEW_LIBRARY:
                {
                    if (await HasLibraryHashCodeChanged())
                    {
                        var success =
                            await _libraryManager.TrySaveLibrary(new XmlLibraryKeeper(), GetPathToCurrentLibrary());
                        if (!success)
                            return;
                    }

                    //var u = Environment.SpecialFolder.DesktopDirectory;
                    // MyDocuments //  $HOME/Documents
                    // CommonApplicationData // /usr/share
                    // DesktopDirectory // $HOME/Desktop
                    // Personal // $HOME/Documents
                    // UserProfile // $HOME

                    await _libraryManager.CreateNewLibrary();

                    // await ShowDisplayAlertAsync(Constants.LIBRARY, Constants.CREATE_NEW_LIBRARY);

                    await UpdateLibraryHashCode();

                    break;
                }
                case Constants.LIBRARY_LOAD:
                    if (await HasLibraryHashCodeChanged())
                    {
                        var success =
                            await _libraryManager.TrySaveLibrary(new XmlLibraryKeeper(), GetPathToCurrentLibrary());
                        if (!success)
                            return;
                    }

                    await _libraryManager.TryLoadLibrary();

                    await UpdateLibraryHashCode();
                    break;
                case Constants.LIBRARY_SAVE:
                {
                    await _libraryManager.TrySaveLibrary(new XmlLibraryKeeper(), GetPathToCurrentLibrary());

                    await UpdateLibraryHashCode();
                    break;
                }
                case Constants.LIBRARY_CLOSE:
                {
                    if (await HasLibraryHashCodeChanged())
                    {
                        var success =
                            await _libraryManager.TrySaveLibrary(new XmlLibraryKeeper(), GetPathToCurrentLibrary());
                        if (!success)
                            return;
                    }

                    _libraryManager.TryCloseLibrary();
                    await UpdateLibraryHashCode();
                    break;
                }
                default:
                {
                    // Performing actions at the LibraryManager
                    await _libraryManager.RunCommand(commandParameter);
                    break;
                }
            }

            await Task.Run(() => RunInMainThread(() =>
            {
                CanOperateWithBooks = Library.Id != 0;
                Library.TotalBooks = Library.BookList.Count;
                RaisePropertyChanged(nameof(Library));
                RaisePropertyChanged(nameof(Library.BookList));
            }));
        }
        else
        {
#if DEBUG
            Debug.WriteLine(
                $"Navigation error path '{commandParameter}' in class '{nameof(BooksViewModel)}' by method '{nameof(PerformAction)}'");
#endif
        }
    }


    // Dispose method for external calls
    public void Dispose()
    {
        if (_disposed) return; // Safeguard against multiple Dispose calls.
        _disposed = true;

        // MessagingCenter cleanup
#if DEBUG
        Console.WriteLine("Cleaning up MessagingCenter resources in LibraryViewModel.");
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
        CanOperateWithBooks = (sender as ILibrary)?.Id != 0;
    }


    /// <summary>
    /// Updates the library state by raising a property changed event for the Library property
    /// and setting the IsEnabled property based on whether the Library Id differs from the default value of 0.
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
            return await ShowCustomDialogPage(Constants.LIBRARY_SAVE, StringsHandler.LibraryChangedMessage());
        }

        return false;
    }

    private string GetPathToCurrentLibrary()
    {
        var folder =
            new NSFileManager().GetUrls(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User)[0]
                .Path;
        return Path.Combine(folder, $"{Library.Id}.xml");
    }

    #endregion


    #region Private Members

    private ILibrary _library;
    private bool _canOperateWithBooks = true;
    private readonly ILibraryManageable _libraryManager;
    private bool _disposed; // Safeguard for multiple calls to Dispose.
    private int _libraryHashCode;

    #endregion
}