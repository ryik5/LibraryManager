using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using LibraryManager.AbstractObjects;
using LibraryManager.Extensions;
using LibraryManager.Models;
using LibraryManager.Views;
using Mopups.PreBaked.PopupPages.DualResponse;
using Mopups.PreBaked.PopupPages.EntryInput;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace LibraryManager.ViewModels;

/// <author>YR 2025-02-09</author>
public sealed partial class FindBooksViewModel : AbstractBookViewModel, IRefreshable
{
    public FindBooksViewModel(ILibrary library, SettingsViewModel settings, IStatusBar statusBar)
        : base(library, statusBar)
    {
         Library.LibraryIdChanged += Handle_LibraryIdChanged;
        Library.TotalBooksChanged += Handle_TotalBooksChanged;
        FoundBookList = new ObservableCollection<Book>();
        SelectedBooks=new ObservableCollection<object>();
        FoundBookList.CollectionChanged += Handle_FoundBookListChanged;
        
       SearchFields = Enum.GetValues(typeof(EBibliographicKindInformation)).Cast<EBibliographicKindInformation>()
            .ToList();
       
        ContentState = Constants.LOAD_CONTENT;
        ClearingState = Constants.CLEAR_CONTENT;
        LoadCover = Constants.LOAD_COVER;
        OK = Constants.SAVE_CHANGES;

        _bookManageable = new BookManagerModel(Library, settings, statusBar);

        HandlePostRefreshControlsOnAppearingTask();
    }
    


    #region Public Properties
    [ObservableProperty] private ObservableCollection<Book> _foundBookList = new();

    /// <summary>
    /// The fields of the book to perform search.
    /// </summary>
    [ObservableProperty] private List<EBibliographicKindInformation> _searchFields;

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
    [ObservableProperty] private string _contentState;

    [ObservableProperty] private string _clearingState;

    [ObservableProperty] private bool _canClearContent;

    [ObservableProperty] private string _loadCover;
    #endregion

    #region Public Methods
    protected override async Task PerformAction(string? commandParameter)
    {
        #if DEBUG
        await ShowNavigationCommandInDebug(commandParameter, nameof(FindBooksPage));
        #endif
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
                        await RefreshControlsOnAppearing();
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

            await UpdateButtonContentState(Book.Content?.IsLoaded ?? false);
        }
        else
        {
            #if DEBUG
            await ShowNavigationErrorInDebug(commandParameter, nameof(FindBooksViewModel));
            #endif
        }
    }


    public override async Task HandleBeforeRefreshControlsOnAppearingTask()
    {
        await RunInMainThreadAsync(() => SelectedBooks.Clear());
    }


    protected override async Task HandlePostRefreshControlsOnAppearingTask()
    {
        IsBooksCollectionViewVisible = true;
        IsEditBookViewVisible = false;
        CanOperateWithBooks = CanSearchBooks;
        CanEditBook = IsBookSelected;
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

    private async Task ValidateOperations()
    {
        await Task.Run(() =>
        {
            IsBooksCollectionViewVisible = true;
            IsEditBookViewVisible = false;
            
        });
    }
    
    private void FindBooks()
    {
        var foundBooks = _bookManageable.FindBooksByKind(SelectedSearchField, SearchText);
        FoundBookList.ResetAndAddRange(foundBooks);
    }

    private bool CanSearchBooks => IsValidLibrary && IsNotEmptyLibrary;

    private bool IsBookSelected => IsNotZero(SelectedBooks?.Count);


    private void Handle_TotalBooksChanged(object? sender, TotalBooksEventArgs e)
    {
        RefreshControlsOnAppearing().GetAwaiter();
    }

    /// <summary>
    /// Handles the CollectionChanged event of the BookList.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The NotifyCollectionChangedEventArgs instance containing the event data.</param>
    private async void Handle_FoundBookListChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SelectedBooks.Clear();
        await Task.Delay(40);
        CanEditBook = false;
    }

    private async void Handle_LibraryIdChanged(object? sender, EventArgs e)
    {
        FoundBookList.Clear();
        SelectedBooks.Clear();
        await HandleLibraryIdChangedTask();
    }

    private async Task HandleLibraryIdChangedTask()
    {
        await RefreshControlsOnAppearing();
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
    private IList<Book> _selectedBooks = new List<Book>();
    private readonly BookManagerModel _bookManageable;
    private string _searchText;
    private bool _searchOnFly;
    private EBibliographicKindInformation _searchField;
    #endregion
}