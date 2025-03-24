using LibraryManager.AbstractObjects;
using LibraryManager.Extensions;
using LibraryManager.Models;
using LibraryManager.Views;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;

namespace LibraryManager.ViewModels;

public class FindBooksViewModel : AbstractViewModel, IRefreshable
{
    public FindBooksViewModel(ILibrary library, SettingsViewModel settings, StatusBarViewModel statusBar)
    {
        SearchFields = Enum.GetValues(typeof(EBibliographicKindInformation)).Cast<EBibliographicKindInformation>()
            .ToList();
        _settings = settings;
        StatusBar = statusBar;
        Library = library;

        Library.TotalBooksChanged += Handle_TotalBooksChanged;
        FoundBookList.CollectionChanged += Handle_FoundBookListChanged;
        SelectionChangedCommand = new Command<IList<object>>(Handle_OnCollectionViewSelectionChanged);
        IsBooksCollectionViewVisible = true;
        IsEditBookViewVisible = false;
        ContentState = Constants.LOAD_CONTENT;
        ClearingState = Constants.CLEAR_CONTENT;
        _bookManageable = new BookManagerModel(Library);

        CanOperateWithBooks = ValidLibrary();
        SelectedBooks?.Clear();
        CanEditBook = false;
    }


    #region Public Properties
    public ICommand SelectionChangedCommand { get; }

    public ILibrary Library { get; set; }

    public ObservableCollection<Book> FoundBookList
    {
        get => _foundBookList;
        set => SetProperty(ref _foundBookList, value);
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

    /// <summary>
    /// The fields of the book to perform search.
    /// </summary>
    public List<EBibliographicKindInformation> SearchFields { get; }

    /// <summary>
    /// The search text.
    /// </summary>
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value) && SearchOnFly)
                FindBooks();
        }
    }

    /// <summary>
    /// Value indicating whether to perform search on the fly.
    /// </summary>
    public bool SearchOnFly
    {
        get => _searchOnFly;
        set
        {
            if (SetProperty(ref _searchOnFly, value) && value && !string.IsNullOrEmpty(SearchText))
                FindBooks();
        }
    }

    /// <summary>
    /// Gets or sets the field of the book to search.
    /// </summary>
    public EBibliographicKindInformation SelectedSearchField
    {
        get => _searchField;
        set
        {
            if (SetProperty(ref _searchField, value) && SearchOnFly && !string.IsNullOrEmpty(SearchText))
                FindBooks();
        }
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

    public bool CanOperateWithBooks
    {
        get => _canOperateWithBooks;
        set => SetProperty(ref _canOperateWithBooks, value);
    }

    public IStatusBar StatusBar
    {
        get => _statusBar;
        set => SetProperty(ref _statusBar, value);
    }

    #region CommandParameters
    public string OK => Constants.SAVE_CHANGES;

    public string Cancel => Constants.CANCEL;

    // TODO : 
    public string ContentState
    {
        get => _contentState;
        set => SetProperty(ref _contentState, value);
    }

    public string ClearingState
    {
        get => _clearingState;
        set => SetProperty(ref _clearingState, value);
    }

    public bool CanClearContent
    {
        get => _canClearContent;
        set => SetProperty(ref _canClearContent, value);
    }
    #endregion
    #endregion


    #region Public Methods
    protected override async Task PerformAction(string? commandParameter)
    {
        await ShowNavigationCommandInDebug(commandParameter, nameof(FindBooksPage));

        if (string.IsNullOrWhiteSpace(commandParameter))
            return;

        // Prevent navigation to the same page - 'Shell.Current.GoToAsync(...'
        if (IsCurrentRoute(nameof(FindBooksPage)))
        {
            switch (commandParameter)
            {
                case nameof(LibraryPage):
                case nameof(BooksPage):
                case nameof(AboutPage):
                case nameof(ToolsPage):
                    await TryGoToPage(commandParameter);
                    break;

                case Constants.FIND_BOOKS:
                {
                    if (!ValidLibrary())
                    {
                        RefreshControlsOnAppearing();
                        return;
                    }

                    await FindBooksTask();
                    break;
                }

                case Constants.CANCEL:
                {
                    IsBooksCollectionViewVisible = true;
                    IsEditBookViewVisible = false;
                    break;
                }

                case Constants.SAVE_CHANGES:
                {
                    if (Book.IsValid())
                    {
                        RunInMainThread(() => { SelectFirstFoundBook()?.Set(Book); });
                    }

                    IsBooksCollectionViewVisible = true;
                    IsEditBookViewVisible = false;
                    break;
                }

                case Constants.EDIT_BOOK:
                {
                    if (!ValidSelectedBooks())
                    {
                        RefreshControlsOnAppearing();
                        return;
                    }

                    RunInMainThread(() => Book = (Book)SelectFirstFoundBook().Clone());

                    IsBooksCollectionViewVisible = false;
                    IsEditBookViewVisible = true;
                    await UpdateButtonContentState(Book.Content?.IsLoaded ?? false);
                    break;
                }

                case Constants.DELETE_BOOK:
                {
                    List<Book> selectedBooks = null;
                    RunInMainThread(() => selectedBooks = SelectedBooks.ToList());
                    // Performing actions by the BooksManager
                    await _bookManageable.RunCommand(commandParameter, SelectedBooks);

                    RunInMainThread(() =>
                        {
                            foreach (var selectedBook in selectedBooks)
                            {
                                FoundBookList.Remove(selectedBook);
                            }
                        }
                    );
                    break;
                }

                case Constants.LOAD_CONTENT:
                case Constants.CLEAR_CONTENT:
                    await _bookManageable.RunCommand(commandParameter, new List<Book>() { Book });
                    await UpdateButtonContentState(Book.Content?.IsLoaded ?? false);
                    break;
                case Constants.SAVE_CONTENT:
                    await _bookManageable.RunCommand(commandParameter, new List<Book>() { Book });
                    break;

                default: //jobs perform without creating views
                {
                    // Performing actions by the BooksManager
                    await _bookManageable.RunCommand(commandParameter, SelectedBooks);

                    break;
                }
            }
        }
        else
        {
            await ShowNavigationErrorInDebug(commandParameter, nameof(FindBooksViewModel));
        }

        RunInMainThread(() => RaisePropertyChanged(nameof(Library)));
    }


    public void RefreshControlsOnAppearing()
    {
        CanOperateWithBooks = ValidLibrary();
        SelectedBooks?.Clear();
        CanEditBook = false;
        RaisePropertyChanged(nameof(Library));
        RaisePropertyChanged(nameof(CanOperateWithBooks));
        RaisePropertyChanged(nameof(CanEditBook));
    }

    public async Task RefreshControlsOnDisappearing()
    {
        SelectedBooks?.Clear();
        Book = null;
        CanEditBook = false;
    }
    #endregion


    #region Private methods
    private bool ValidSelectedBooks() => NotZero(SelectedBooks?.Count);

    private bool ValidLibrary() => NotZero(Library?.Id);

    private bool NotZero(int? number)
    {
        if (number == 0)
            return false;

        return true;
    }

    /// <summary>
    /// Finds books based on the search text. Updates <see cref="FoundBookList"/>.
    /// </summary>
    private Task FindBooksTask()
    {
        RunInMainThread(FindBooks);

        return Task.CompletedTask;
    }

    private void FindBooks()
    {
        var foundBooks = _bookManageable.FindBooksByKind(SelectedSearchField, SearchText);
        FoundBookList.ResetAndAddRange(foundBooks);
    }

    private void Handle_TotalBooksChanged(object? sender, TotalBooksEventArgs e)
    {
        FoundBookList.Clear();
        CanOperateWithBooks = ValidLibrary();
    }

    /// <summary>
    /// Handles the CollectionChanged event of the BookList.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The NotifyCollectionChangedEventArgs instance containing the event data.</param>
    private void Handle_FoundBookListChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SelectedBooks?.Clear();
        SelectedBooks = null;
        Book = null;
        CanEditBook = false;
        ;
    }

    private void Handle_OnCollectionViewSelectionChanged(IList<object> obj)
    {
        SelectedBooks?.Clear();
        SelectedBooks = obj?.Select(b => b as Book)?.ToList();
        CanEditBook = ValidSelectedBooks();
    }

    private Book? SelectFirstFoundBook() => ValidSelectedBooks() ? SelectedBooks[0] : null;

    private Task CleanSelectedBooksTask()
    {
        CleanSelectedBooks();
        return Task.CompletedTask;
    }

    private void CleanSelectedBooks()
    {
        SelectedBooks?.Clear();
        SelectedBooks = null;
        Book = null;
        CanEditBook = false;
    }

    private Task UpdateButtonContentState(bool isContentLoaded)
    {
        ContentState = isContentLoaded ? Constants.SAVE_CONTENT : Constants.LOAD_CONTENT;
        CanClearContent = isContentLoaded;
        return Task.CompletedTask;
    }
    #endregion


    #region Private fields
    private ObservableCollection<Book> _foundBookList = new();
    private IList<Book> _selectedBooks = new List<Book>();
    private Book _book;
    private readonly BookManagerModel _bookManageable;
    private string _searchText;
    private bool _searchOnFly;
    private EBibliographicKindInformation _searchField;
    private bool _canEditBook;
    private bool _isEditBookViewVisible;
    private bool _isBooksCollectionViewVisible;
    private bool _canOperateWithBooks;
    private string _contentState;
    private bool _canClearContent;
    private string _clearingState;
    private readonly SettingsViewModel _settings;
    private IStatusBar _statusBar;
    #endregion
}