using System.Diagnostics;
using LibraryManager.Models;
using LibraryManager.Utils;
using LibraryManager.Views;
using System.Collections.Specialized;
using System.Windows.Input;

namespace LibraryManager.ViewModels;

public class BooksViewModel : AbstractViewModel, IDisposable
{
    public BooksViewModel(ILibrary library)
    {
        _settings = new SettingsViewModel();

        Library = library;
        Library.BookList.CollectionChanged += BookList_CollectionChanged;
        _bookManageable = new BookManagerModel(Library);
        SelectedBooks = new List<Book>();
        SelectionChangedCommand = new Command<IList<object>>(HandleOnCollectionViewSelectionChanged);
        IsBooksCollectionViewVisible = true;
        IsEditBookViewVisible = false;
        Handle_SelectedBooks_CollectionChanged().ConfigureAwait(false);
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

    public ICommand SelectionChangedCommand { get; }

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

    public bool CanEditBook
    {
        get => _canEditBook;
        set => SetProperty(ref _canEditBook, value);
    }

    public event EventHandler<TotalBooksEventArgs>? TotalBooksChanged;

    #region CommandParameters
    public string OK
    {
        get => _ok;
        set => SetProperty(ref _ok, value);
    }

    public string Cancel => Constants.CANCEL;
    #endregion
    #endregion


    #region Public Methods
    protected override async Task PerformAction(string? commandParameter)
    {
        await ShowNavigationCommandInDebug(commandParameter, nameof(BooksPage));

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
                    await TryGoToPage(commandParameter);
                    break;
                }
                case Constants.CANCEL:
                {
                    Book = null;
                    IsBooksCollectionViewVisible = true;
                    IsEditBookViewVisible = false;
                    break;
                }
                case Constants.SAVE:
                {
                    if (Book.IsValid())
                    {
                        RunInMainThread(() => _bookManageable.AddBook(Book));
                    }

                    IsBooksCollectionViewVisible = true;
                    IsEditBookViewVisible = false;
                    Book = null;
                    break;
                }
                case Constants.ADD_BOOK:
                {
                    OK = Constants.SAVE;
                    Book = BookModelMaker.GenerateBook();
                    IsBooksCollectionViewVisible = false;
                    IsEditBookViewVisible = true;
                    break;
                }
                case Constants.SAVE_CHANGES:
                {
                    if (Book.IsValid())
                    {
                        RunInMainThread(() => { SelectFirstFoundBook()?.Set(Book); });
                    }

                    Book = null;
                    IsBooksCollectionViewVisible = true;
                    IsEditBookViewVisible = false;
                    break;
                }
                case Constants.EDIT_BOOK:
                {
                    if (!ValidSelectedBooks())
                        return;
                    RunInMainThread(() => Book = (Book)SelectFirstFoundBook().Clone());

                    OK = Constants.SAVE_CHANGES;

                    IsBooksCollectionViewVisible = false;
                    IsEditBookViewVisible = true;

                    break;
                }
                case Constants.SORT_BOOKS:
                {
                    if (await MakeSortingList() is { Count: > 0 } props)
                        _bookManageable.SafetySortBooks(props);
                    break;
                }
                case Constants.EXPORT_BOOK:
                {
                    if (!ValidSelectedBooks())
                        return;

                    // display window with input a new book name
                    var customDialog = await ShowCustomDialogPage(Constants.EXPORT_BOOK,
                        Constants.INPUT_NAME, true);

                    RunInMainThread(() => Book = SelectFirstFoundBook());

                    var bookName = customDialog.IsOk && !string.IsNullOrEmpty(customDialog.InputString)
                        ? customDialog.InputString
                        : $"{Book.Author}. {Book.Title}";

                   await _bookManageable.TrySaveBook(new XmlBookKeeper(), Book, GetPathToFile(bookName));

                    break;
                }
                default: //jobs perform without creating views
                {
                    // Performing actions at the BooksManager
                    await _bookManageable.RunCommand(commandParameter, SelectedBooks);
                    break;
                }
            }
        }
        else
        {
            await ShowNavigationErrorInDebug(commandParameter, nameof(BooksViewModel));
        }

        RunInMainThread(() =>
            {
                RaisePropertyChanged(nameof(Library));
                RaisePropertyChanged(nameof(Library.BookList));
            }
        );
    }

    // Dispose method for external calls
    public void Dispose()
    {
        if (_disposed)
            return; // Safeguard against multiple Dispose calls.

        _disposed = true;

        // Perform cleanup: Unsubscribe from any events
        Library.BookList.CollectionChanged -= BookList_CollectionChanged;
        // SelectedBooks.CollectionChanged -= Handle_SelectedBooks_CollectionChanged;
        SelectedBooks.Clear();

        #if DEBUG
        Debug.WriteLine("BooksViewModel disposed successfully.");
        #endif
    }

    ~BooksViewModel()
    {
        Dispose(); // Safeguard cleanup in destructor (if proper disposal is skipped)
    }
    #endregion


    #region Private methods
    /// <summary>
    /// Handles the CollectionChanged event of the BookList.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The NotifyCollectionChangedEventArgs instance containing the event data.</param>
    private void BookList_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        TotalBooksChanged?.Invoke(this, new TotalBooksEventArgs { TotalBooks = Library.BookList.Count });
        SelectedBooks.Clear();
        Book = null;
        Library.TotalBooks = Library.BookList.Count;
    }


    private bool CanEditSelectedBook()
    {
        if (SelectedBooks.Count == 0)
            return false;

        return true;
    }

    /// <summary>
    /// Makes sorting property name list.
    /// </summary>
    private Task<List<PropertyCustomInfo>> MakeSortingList()
    {
        var props = new List<PropertyCustomInfo>();

        MakeBookCutomPropertyList(props, _settings.FirstSortBookProperty, _settings.FirstSortProperty_ByDescend);
        MakeBookCutomPropertyList(props, _settings.SecondSortBookProperty, _settings.SecondSortProperty_ByDescend);
        MakeBookCutomPropertyList(props, _settings.ThirdSortBookProperty, _settings.ThirdSortProperty_ByDescend);

        void MakeBookCutomPropertyList(List<PropertyCustomInfo> props, string name, bool byDescend)
        {
            var prop = _bookManageable.Library.FindBookPropertyInfo(name);
            var customProp = new PropertyCustomInfo { PropertyInfo = prop, DescendingOrder = byDescend };
            if (prop.Name != nameof(Book.None))
                props.Add(customProp);
        }

        return Task.FromResult(props);
    }

    private void HandleOnCollectionViewSelectionChanged(IList<object> obj)
    {
        SelectedBooks = obj?.Select(b => b as Book)?.ToList();
    }


    private async void Handle_SelectedBooks_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        await Handle_SelectedBooks_CollectionChanged();
    }

    private Task Handle_SelectedBooks_CollectionChanged()
    {
        CanEditBook = 0 < SelectedBooks.Count;
        Book = null;
        return Task.CompletedTask;
    }

    private bool ValidSelectedBooks() => MoreZero(SelectedBooks?.Count ?? 0);

    private bool ValidLibary() => MoreZero(Library.TotalBooks);

    private bool MoreZero(int number)
    {
        if (number == 0)
            return false;

        return true;
    }

    private Book? SelectFirstFoundBook() => ValidSelectedBooks() ? SelectedBooks[0] : null;
    #endregion


    #region Private fields
    private readonly IBookManageable _bookManageable;
    private ILibrary _library;
    private IList<Book> _selectedBooks;
    private Book _book;
    private SettingsViewModel _settings;
    private bool _isBooksCollectionViewVisible;
    private bool _isEditBookViewVisible;
    private bool _disposed; // Safeguard for multiple calls to Dispose.
    private string _loadingState;
    private bool _canEditBook;
    private string _ok;
    private object _onSelectionObject;
    #endregion
}