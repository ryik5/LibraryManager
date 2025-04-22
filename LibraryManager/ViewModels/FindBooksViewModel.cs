using CommunityToolkit.Mvvm.Messaging;
using LibraryManager.AbstractObjects;
using LibraryManager.Extensions;
using LibraryManager.Models;
using LibraryManager.Views;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace LibraryManager.ViewModels;

/// <author>YR 2025-02-09</author>
public sealed class FindBooksViewModel : AbstractBookViewModel, IRefreshable
{
    public FindBooksViewModel(ILibrary library, SettingsViewModel settings, IStatusBar statusBar) : base(library,
        statusBar)
    {
        SearchFields = Enum.GetValues(typeof(EBibliographicKindInformation)).Cast<EBibliographicKindInformation>()
            .ToList();

        Library.LibraryIdChanged += Handle_LibraryIdChanged;
        Library.TotalBooksChanged += Handle_TotalBooksChanged;
        FoundBookList.CollectionChanged += Handle_FoundBookListChanged;
        IsBooksCollectionViewVisible = true;
        IsEditBookViewVisible = false;
        ContentState = Constants.LOAD_CONTENT;
        ClearingState = Constants.CLEAR_CONTENT;
        _bookManageable = new BookManagerModel(Library, settings, statusBar);

        OK = Constants.SAVE_CHANGES;
        HandleOperateWithBooks().GetAwaiter();
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
                    if (!CanSearchBooks)
                    {
                        await RefreshControlsOnAppearingTask();
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
                        await RunInMainThreadAsync(() => { FirstSelectedBook?.Set(Book); });
                    }

                    IsBooksCollectionViewVisible = true;
                    IsEditBookViewVisible = false;
                    break;
                }

//                case Constants.CLICK: // TODO : Handle it
                case Constants.DOUBLECLICK:
                case Constants.EDIT_BOOK:
                {
                    if (!IsBookSelected)
                    {
                        await RefreshControlsOnAppearingTask();
                        return;
                    }

                    await RunInMainThreadAsync(() => Book = FirstSelectedBook.Clone());

                    IsBooksCollectionViewVisible = false;
                    IsEditBookViewVisible = true;
                    await UpdateButtonContentState(Book.Content?.IsLoaded ?? false);
                    break;
                }
                case Constants.DELETE_BOOK:
                {
                    RunInMainThread(() => _selectedBooks = GetSelectedBooks());
                    // Performing actions by the BooksManager
                    await _bookManageable.RunCommand(commandParameter, _selectedBooks);

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
                case Constants.SAVE_CONTENT:
                case Constants.LOAD_COVER:
                    var book = Book;
                    await _bookManageable.RunCommand(commandParameter, new List<Book>() { book });
                    Book = null;
                    Book = book;
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

        await UpdateButtonContentState(Book.Content?.IsLoaded ?? false);
    }

    protected override async Task RefreshControlsOnAppearing()
    {
        await RunInMainThreadAsync(() => SelectedBooks.Clear());
        IsBooksCollectionViewVisible = true;
        IsEditBookViewVisible = false;
        await HandleOperateWithBooks();
    }
    #endregion


    #region Private methods
    /// <summary>
    /// Finds books based on the search text. Updates <see cref="FoundBookList"/>.
    /// </summary>
    private Task FindBooksTask()
    {
        RunInMainThread(FindBooks);

        WeakReferenceMessenger.Default.Send(
            new StatusMessage()
            {
                InfoKind = EInfoKind.CurrentInfo,
                Message = $"Attempt to find books by '{SelectedSearchField}' where part element is '{SearchText}'."
            });

        return Task.CompletedTask;
    }

    private void FindBooks()
    {
        var foundBooks = _bookManageable.FindBooksByKind(SelectedSearchField, SearchText);
        FoundBookList.ResetAndAddRange(foundBooks);
    }

    private async Task HandleOperateWithBooks()
    {
        CanOperateWithBooks = CanSearchBooks;
        CanEditBook = IsBookSelected;
    }

    private bool CanSearchBooks => IsValidLibrary && IsNotEmptyLibrary;

    private bool IsBookSelected => NotZero(SelectedBooks?.Count);


    private void Handle_TotalBooksChanged(object? sender, TotalBooksEventArgs e)
    {
        HandleOperateWithBooks().GetAwaiter();
    }

    /// <summary>
    /// Handles the CollectionChanged event of the BookList.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The NotifyCollectionChangedEventArgs instance containing the event data.</param>
    private void Handle_FoundBookListChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SelectedBooks.Clear();
        CanEditBook = false;
    }

    private void Handle_LibraryIdChanged(object? sender, EventArgs e)
    {
        FoundBookList.Clear();
        SelectedBooks.Clear();
        HandleOperateWithBooks().GetAwaiter();
    }


    private Book? FirstSelectedBook => 0 < SelectedBooks.Count ? SelectedBooks[0] as Book : null;


    private async Task UpdateButtonContentState(bool isContentLoaded)
    {
        ContentState = isContentLoaded ? Constants.SAVE_CONTENT : Constants.LOAD_CONTENT;
        CanClearContent = isContentLoaded;
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