using LibraryManager.AbstractObjects;
using LibraryManager.Extensions;
using LibraryManager.Models;
using LibraryManager.Views;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace LibraryManager.ViewModels;

public class FindBooksViewModel : AbstractBookViewModel, IRefreshable
{
    public FindBooksViewModel(ILibrary library, SettingsViewModel settings, StatusBarViewModel statusBar)
    {
        SearchFields = Enum.GetValues(typeof(EBibliographicKindInformation)).Cast<EBibliographicKindInformation>()
            .ToList();
        StatusBar = statusBar;
        Library = library;

        Library.TotalBooksChanged += Handle_TotalBooksChanged;
        FoundBookList.CollectionChanged += Handle_FoundBookListChanged;
        IsBooksCollectionViewVisible = true;
        IsEditBookViewVisible = false;
        ContentState = Constants.LOAD_CONTENT;
        ClearingState = Constants.CLEAR_CONTENT;
        _bookManageable = new BookManagerModel(Library,settings);

        CanOperateWithBooks = ValidLibrary();
        CanEditBook = false;
        OK = Constants.SAVE_CHANGES;
    }


    #region Public Properties
    public ObservableCollection<Book> FoundBookList
    {
        get => _foundBookList;
        set => SetProperty(ref _foundBookList, value);
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
    #endregion

    
    #region CommandParameters
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
                        await RefreshControlsOnAppearing();
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
                        await RunInMainThreadAsync(() => { SelectFirstFoundBook()?.Set(Book); });
                    }

                    IsBooksCollectionViewVisible = true;
                    IsEditBookViewVisible = false;
                    break;
                }

//                case Constants.CLICK: // TODO : Handle it
                case Constants.DOUBLECLICK:
                case Constants.EDIT_BOOK:
                {
                    if (!ValidSelectedBooks())
                    {
                        await RefreshControlsOnAppearing();
                        return;
                    }

                    await RunInMainThreadAsync(() => Book = (Book)SelectFirstFoundBook().Clone());

                    IsBooksCollectionViewVisible = false;
                    IsEditBookViewVisible = true;
                    await UpdateButtonContentState(Book.Content?.IsLoaded ?? false);
                    break;
                }

                case Constants.DELETE_BOOK:
                {
                    RunInMainThread(() => _selectedBooks = GetSelectedBooks());
                    // Performing actions by the BooksManager
                    _bookManageable.RunCommand(commandParameter, _selectedBooks).ConfigureAwait(false);

                    RunInMainThread(() =>
                        {
                            foreach (var selectedBook in _selectedBooks)
                            {
                                FoundBookList.Remove(selectedBook);
                            }
                        }
                    );
                    break;
                }

                //TODO : move inside BookManager
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
                    await RunInMainThreadAsync(() => _selectedBooks = GetSelectedBooks());
                    await _bookManageable.RunCommand(commandParameter, _selectedBooks);

                    break;
                }
            }
        }
        else
        {
            await ShowNavigationErrorInDebug(commandParameter, nameof(FindBooksViewModel));
        }

        await RunInMainThreadAsync(() => RaisePropertyChanged(nameof(Library)));
    }

    public async Task RefreshControlsOnAppearing()
    {
        CanOperateWithBooks = ValidLibrary();
        await RunInMainThreadAsync(() => SelectedBooks.Clear());
        CanEditBook = false;
        RaisePropertyChanged(nameof(Library));
        RaisePropertyChanged(nameof(CanOperateWithBooks));
        RaisePropertyChanged(nameof(CanEditBook));
    }

    public async Task RefreshControlsOnDisappearing()
    {
        await RunInMainThreadAsync(() => SelectedBooks.Clear());
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
        // FoundBookList.Clear();
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
        Book = null;
        CanEditBook = false;
        ;
    }

    private Book? SelectFirstFoundBook() => 0 < SelectedBooks.Count ? SelectedBooks[0] as Book : null;


    private Task UpdateButtonContentState(bool isContentLoaded)
    {
        ContentState = isContentLoaded ? Constants.SAVE_CONTENT : Constants.LOAD_CONTENT;
        CanClearContent = isContentLoaded;
        return Task.CompletedTask;
    }

    private IList<Book> GetSelectedBooks() => SelectedBooks.Select(b => b as Book)?.ToList();
    #endregion


    #region Private fields
    private ObservableCollection<Book> _foundBookList = new();
    private IList<Book> _selectedBooks = new List<Book>();
    private readonly BookManagerModel _bookManageable;
    private string _searchText;
    private bool _searchOnFly;
    private EBibliographicKindInformation _searchField;
    private string _contentState;
    private bool _canClearContent;
    private string _clearingState;
    #endregion
}