using System.ComponentModel;
using System.Diagnostics;
using LibraryManager.Models;
using LibraryManager.Views;

namespace LibraryManager.ViewModels;

public class BooksViewModel : AbstractViewModel, IDisposable
{
    public BooksViewModel(ILibrary library)
    {
        Library = library;
        Library.BookList.CollectionChanged += BookList_CollectionChanged;
        _bookManageable = new BookManagerModel(Library);

        IsBooksCollectionViewVisible = true;
        IsEditBookViewVisible = false;
    }


    #region Public properties
    public ILibrary Library
    {
        get => _library;
        set => SetProperty(ref _library, value);
    }

    public IList<Book> SelectedBooks
    {
        get => _selectedBooks;
        set => SetProperty(ref _selectedBooks, value);
    }

    public Book Book
    {
        get => _book;
        set => SetProperty(ref _book, value);
    }

    public string LoadingState
    {
        get => _loadingState;
        set => SetProperty(ref _loadingState, value);
    }

    public bool IsBooksCollectionViewVisible
    {
        get => _isBooksCollectionViewVisible;
        set => SetProperty(ref _isBooksCollectionViewVisible, value);
    }

    public bool IsEditBookViewVisible
    {
        get => _isEditBookViewVisible;
        set => SetProperty(ref _isEditBookViewVisible, value);
    }

    public event EventHandler<TotalBooksEventArgs> TotalBooksChanged;
    #endregion


    #region Public Methods
    protected override async Task PerformAction(string? commandParameter)
    {
        #if DEBUG
        Debug.WriteLine($"NavigateCommand on {nameof(BooksPage)} triggered with commandParameter: {commandParameter}");
        #endif

        if (string.IsNullOrWhiteSpace(commandParameter))
            return;

        // Prevent navigation to the same page - 'Shell.Current.GoToAsync(...'
        if (IsCurrentRoute(nameof(BooksPage)))
        {
            switch (commandParameter)
            {
                case nameof(AboutPage):
                case nameof(LibraryPage):
                case nameof(FindBooksPage):
                case nameof(ToolsPage):
                {
                    try
                    {
                        // Dynamically navigate using the provided commandParameter
                        // begins '//' added in the beginning to switch a Menu as well as Page. without '//' it switch only Page
                        await Shell.Current.GoToAsync($"//{commandParameter}").ConfigureAwait(false);
                    }
                    catch (Exception ex) // Handle any issues with navigation
                    {
                        Debug.WriteLine($"Navigation error: {ex.Message}");
                    }
                }
                    break;
                case Constants.EDIT_BOOK:
                    if (StartEditSelectedBook())
                    {
                        LoadingState = Book.Content is null ? Constants.LOAD_CONTENT : Constants.CONTENT_WAS_LOADED;

                        SwitchViewsVisibilityWithinAction(true);
                    }

                    break;
                case Constants.SORT_BOOKS:
                    // _bookManageable.SafetySortBooks();
                    break;
                default: //jobs perform without creating views
                {
                    // Performing actions at the BooksManager
                    await _bookManageable.RunCommand(commandParameter, SelectedBooks);

                    RunInMainThread(() =>
                        {
                            RaisePropertyChanged(nameof(Library));
                            RaisePropertyChanged(nameof(Library.BookList));
                        }
                    );
                    break;
                }
            }
        }
        else
        {
            #if DEBUG
            Debug.WriteLine(
                $"Navigation error path '{commandParameter}' in class '{nameof(BooksViewModel)}' by method '{nameof(PerformAction)}'");
            #endif
        }
    }

    protected async override Task PerformExtendedAction(string? commandParameter)
    {
        #if DEBUG
        Debug.WriteLine($"ExtendedCommand triggered with commandParameter: {commandParameter}");
        #endif

        if (string.IsNullOrWhiteSpace(commandParameter))
            return;

        switch (commandParameter)
        {
            case Constants.SAVE_CHANGES:
                if (CanEditSelectedBook())
                    SelectedBooks[0].Set(Book);

                Book = null;
                SwitchViewsVisibilityWithinAction(false);
                break;

            case Constants.EDITING_BOOK_WAS_CANCELLED:
                Book = null;
                SwitchViewsVisibilityWithinAction(false);
                break;

            default:
            {
                break;
            }
        }
    }

    // Dispose method for external calls
    public void Dispose()
    {
        if (_disposed) return; // Safeguard against multiple Dispose calls.
        _disposed = true;
        Library.BookList.CollectionChanged -= BookList_CollectionChanged;

        // Perform cleanup: Unsubscribe from any events
        SelectedBooks = null;
        if (_library is INotifyPropertyChanged notifyLibrary)
        {
            notifyLibrary.PropertyChanged -= OnLibraryChanged;
        }

        Debug.WriteLine("BooksViewModel disposed successfully.");
    }

    ~BooksViewModel()
    {
        Dispose(); // Safeguard cleanup in destructor (if proper disposal is skipped)
    }
    #endregion


    #region Private methods
    private void OnLibraryChanged(object sender, PropertyChangedEventArgs e)
    {
        Debug.WriteLine($"Library property changed: {e.PropertyName}");
    }

    /// <summary>
    /// Handles the CollectionChanged event of the BookList.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The NotifyCollectionChangedEventArgs instance containing the event data.</param>
    private void BookList_CollectionChanged(object? sender,
        System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        TotalBooksChanged?.Invoke(this, new TotalBooksEventArgs { TotalBooks = Library.BookList.Count });
        SelectedBooks.Clear();
        Book = null;
        Library.TotalBooks = Library.BookList.Count;
    }

    private bool StartEditSelectedBook()
    {
        if (!CanEditSelectedBook())
            return false;

        Book = SelectedBooks[0].Clone() as Book;

        return true;
    }

    private bool CanEditSelectedBook()
    {
        if (SelectedBooks.Count == 0)
            return false;

        return true;
    }

    private void SwitchViewsVisibilityWithinAction(bool isSwitchOff)
    {
        IsBooksCollectionViewVisible = !isSwitchOff;
        IsEditBookViewVisible = isSwitchOff;
    }
    #endregion


    #region Private fields
    private readonly IBookManageable _bookManageable;
    private ILibrary _library;
    private IList<Book> _selectedBooks = new List<Book>();
    private Book _book;
    private bool _isBooksCollectionViewVisible;
    private bool _isEditBookViewVisible;
    private bool _disposed; // Safeguard for multiple calls to Dispose.
    private string _loadingState;
    #endregion
}