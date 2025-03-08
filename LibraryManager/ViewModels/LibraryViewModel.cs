using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using LibraryManager.Models;
using LibraryManager.Views;

namespace LibraryManager.ViewModels;

public class LibraryViewModel : AbstractViewModel, IDisposable
{
    public LibraryViewModel(ILibrary library)
    {
        Library = library; // Constructor injection ensures proper dependency handling
        Library.BookList.CollectionChanged += BookList_CollectionChanged;

        _libraryManageable = new LibraryManagerModel(Library);

        // Initialize the generic navigation command
        NavigateCommand = new AsyncRelayCommand<string>(NavigateToPage);

        /*MessagingCenter.Subscribe<BooksViewModel>(this, "Navigate", async (sender) =>
        {
            Console.WriteLine("Received navigation request from BooksViewModel.");
           // await NavigateToPage("CreateLibrary").ConfigureAwait(false);
        });*/
    }


    #region Public properties

    public ILibrary? Library
    {
        get => _library;
        set => SetProperty(ref _library, value);
    }

    public event EventHandler<TotalBooksEventArgs> TotalBooksChanged;

    #endregion


    #region Public Methods

    protected override async Task NavigateToPage(string? commandName)
    {
        Debug.WriteLine($"NavigateCommand triggered with commandName: {commandName}");

        if (string.IsNullOrWhiteSpace(commandName))
            return;

        var currentRoute = Shell.Current.CurrentState.Location.OriginalString;
        if (currentRoute == $"//{nameof(LibraryPage)}")
        {
            switch (commandName)
            {
                case nameof(BooksPage):
                {
                    try
                    {
                        // Dynamically navigate using the provided commandName
                        // begins '//' added in the beginning to switch a Menu as well as Page. without '//' it switch only Page
                        await Shell.Current.GoToAsync($"//{commandName}").ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Navigation error: {ex.Message}");
                    }
                }
                    break;
                default:
                {
#if DEBUG
                    Debug.WriteLine($"Commands {commandName} on {nameof(LibraryPage)} page.");
#endif
                    // TODO : performing actions at the LibraryManager
                }
                    break;
            }
        }
        else
        {
#if DEBUG
            Debug.WriteLine(
                $"Navigation error path '{commandName}' in class '{nameof(BooksViewModel)}' by method '{nameof(NavigateToPage)}'");
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
        if (Library != null)
            Library.BookList.CollectionChanged -= BookList_CollectionChanged;

        MessagingCenter.Unsubscribe<BooksViewModel>(this, "Navigate");
    }

    ~LibraryViewModel()
    {
        Dispose(); // Safeguard cleanup in destructor (if proper disposal is skipped)
    }

    #endregion


    #region Private methods

    private async Task LibraryCreateNewAsync()
    {
        await ShowDisplayAlertAsync("Message", "Pressed - CreateNewLibrary");
    }

    /// <summary>
    /// Handles the CollectionChanged event of the BookList.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The NotifyCollectionChangedEventArgs instance containing the event data.</param>
    private void BookList_CollectionChanged(object? sender,
        System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        TotalBooksChanged?.Invoke(this, new TotalBooksEventArgs { TotalBooks = Library?.BookList?.Count ?? 0 });
    }

    #endregion


    #region Private fields

    private ILibrary? _library;
    private ILibraryManageable _libraryManageable;
    private bool _disposed; // Safeguard for multiple calls to Dispose.

    #endregion
}