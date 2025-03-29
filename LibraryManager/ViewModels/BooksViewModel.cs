using LibraryManager.AbstractObjects;
using System.Diagnostics;
using LibraryManager.Models;
using LibraryManager.Utils;
using LibraryManager.Views;
using System.Collections.Specialized;

namespace LibraryManager.ViewModels;

public class BooksViewModel : AbstractBookViewModel, IDisposable, IRefreshable
{
    public BooksViewModel(ILibrary library, SettingsViewModel settings, StatusBarViewModel statusBar)
    {
        _settings = settings;
        StatusBar = statusBar;
        Library = library;

        Library.LibraryIdChanged += Handle_LibraryIdChanged;
        Library.BookList.CollectionChanged += Handle_BookListCollectionChanged;
        Library.TotalBooksChanged += Handle_TotalBooksChanged;
        IsBooksCollectionViewVisible = true;
        IsEditBookViewVisible = false;
        _bookManageable = new BookManagerModel(Library);
        ContentState = Constants.LOAD_CONTENT;
        ClearingState = Constants.CLEAR_CONTENT;
        CanOperateWithBooks = ValidLibrary();
        CanEditBook = ValidSelectedBooks();
    }


    #region Public properties
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
                case Constants.SELECTION_CHANGED:
                    CanOperateWithBooks = ValidLibrary();
                    CanEditBook = ValidSelectedBooks();
                    break;
                case Constants.CANCEL:
                {
                    IsBooksCollectionViewVisible = true;
                    IsEditBookViewVisible = false;
                    CanEditBook = ValidSelectedBooks();
                    break;
                }
                case Constants.SAVE:
                {
                    if (Book.IsValid())
                    {
                        await _bookManageable.AddBookTask(Book);
                    }

                    IsBooksCollectionViewVisible = true;
                    IsEditBookViewVisible = false;
                    break;
                }
                case Constants.ADD_BOOK:
                {
                    IsBooksCollectionViewVisible = false;
                    IsEditBookViewVisible = true;
                    OK = Constants.SAVE;
                    ContentState = Constants.LOAD_CONTENT;
                    Book = BookModelMaker.GenerateBook();
                    break;
                }
                case Constants.SAVE_CHANGES:
                {
                    if (Book.IsValid())
                    {
                        await RunInMainThreadAsync(() =>
                        {
                            var book = Library.BookList.FirstOrDefault(b => b.Id == Book.Id);
                            book?.Set(Book);
                        });
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

                    await RunInMainThreadAsync(() => Book = SelectFirstFoundBook().Clone());

                    OK = Constants.SAVE_CHANGES;

                    IsBooksCollectionViewVisible = false;
                    IsEditBookViewVisible = true;

                    break;
                }
                //TODO : move inside BookManager
                case Constants.SORT_BOOKS:
                {
                    if (await MakeSortingList() is { Count: > 0 } props)
                        await _bookManageable.SafetySortBooks(props);
                    break;
                }

                //TODO : move inside BookManager
                case Constants.IMPORT_BOOK:
                {
                    await _bookManageable.TryLoadBook();
                    break;
                }
                case Constants.EXPORT_BOOK:
                {
                    if (!ValidSelectedBooks())
                    {
                        await RefreshControlsOnAppearing();
                        return;
                    }

                    // display window with input a new book name
                    var customDialog = await ShowCustomDialogPage(Constants.EXPORT_BOOK,
                        Constants.INPUT_NAME, true);

                    await RunInMainThreadAsync(() => Book = SelectFirstFoundBook());

                    var bookName = customDialog.IsOk && !string.IsNullOrEmpty(customDialog.InputString)
                        ? customDialog.InputString
                        : $"{Book.Author}. {Book.Title}";

                    await _bookManageable.TrySaveBook(new XmlBookKeeper(), Book, GetPathToFile(bookName));

                    break;
                }
                //TODO : move inside BookManager
                case Constants.LOAD_CONTENT:
                case Constants.CLEAR_CONTENT:
                case Constants.SAVE_CONTENT:
                case Constants.LOAD_COVER:
                    await _bookManageable.RunCommand(commandParameter, new List<Book>() { Book });
                    RaisePropertyChanged(nameof(Book));
                    break;
                default: //jobs perform without creating views
                {
                    // Performing actions at the BooksManager
                    await RunInMainThreadAsync(() => _selectedBooks = GetSelectedBooks());
                    await _bookManageable.RunCommand(commandParameter, _selectedBooks);
                    break;
                }
            }
        }
        else
        {
            await ShowNavigationErrorInDebug(commandParameter, nameof(BooksViewModel));
        }

        await UpdateButtonContentState(Book.Content?.IsLoaded ?? false);
        await RunInMainThreadAsync(() =>
            {
                RaisePropertyChanged(nameof(Library));
                RaisePropertyChanged(nameof(Library.BookList));
                RaisePropertyChanged(nameof(Book));
            }
        );
    }

    public async Task RefreshControlsOnAppearing()
    {
        await RunInMainThreadAsync(() => SelectedBooks.Clear());
        Book = null;
        CanOperateWithBooks = ValidLibrary();
        CanEditBook = ValidSelectedBooks();
    }

    public async Task RefreshControlsOnDisappearing()
    {
        await RunInMainThreadAsync(() => SelectedBooks.Clear());
        Book = null;
        CanEditBook = false;
        RaisePropertyChanged(nameof(CanOperateWithBooks));
        RaisePropertyChanged(nameof(CanEditBook));
    }


    // Dispose method for external calls
    public void Dispose()
    {
        if (_disposed)
            return; // Safeguard against multiple Dispose calls.

        _disposed = true;

        // Perform cleanup: Unsubscribe from any events
        Library.LibraryIdChanged -= Handle_LibraryIdChanged;
        SelectedBooks?.Clear();

        #if DEBUG
        Debug.WriteLine($"{nameof(BooksViewModel)} disposed successfully.");
        #endif
    }

    ~BooksViewModel()
    {
        Dispose(); // Safeguard cleanup in destructor (if proper disposal is skipped)
    }
    #endregion


    #region Private methods
    private void Handle_LibraryIdChanged(object? sender, EventArgs e)
    {
        SelectedBooks?.Clear();
        Book = null;
        RefreshControlsOnAppearing();
        Library.TotalBooks = Library.BookList.Count;
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

    private void Handle_BookListCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Library.TotalBooks = Library.BookList.Count;
        SelectedBooks.Clear();
    }

    private async void Handle_TotalBooksChanged(object? sender, TotalBooksEventArgs e)
    {
        await StatusBar.SetTotalBooks(Library.TotalBooks);
    }

    private Book? SelectFirstFoundBook() => GetSelectedBooks()[0];
    private bool ValidSelectedBooks() => MoreZero(SelectedBooks?.Count ?? 0);

    private bool ValidLibrary() => MoreZero(Library?.Id ?? 0);

    private bool MoreZero(int number)
    {
        if (number == 0)
            return false;

        return true;
    }

    private Task UpdateButtonContentState(bool isContentLoaded)
    {
        ContentState = isContentLoaded ? Constants.SAVE_CONTENT : Constants.LOAD_CONTENT;
        CanClearContent = isContentLoaded;
        return Task.CompletedTask;
    }

    private IList<Book> GetSelectedBooks() => SelectedBooks?.Select(b => b as Book)?.ToList();
    #endregion


    #region Private fields
    private readonly IBookManageable _bookManageable;
    private IList<Book> _selectedBooks;
    private SettingsViewModel _settings;
    private bool _disposed; // Safeguard for multiple calls to Dispose.
    private string _contentState;
    private string _clearingState;
    private bool _canClearContent;
    #endregion
}