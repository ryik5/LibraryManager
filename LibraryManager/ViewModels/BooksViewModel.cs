using CommunityToolkit.Mvvm.Messaging;
using LibraryManager.AbstractObjects;
using LibraryManager.Models;
using LibraryManager.Utils;
using LibraryManager.Views;
using System.Collections.Specialized;

namespace LibraryManager.ViewModels;

/// <author>YR 2025-02-09</author>
public sealed class BooksViewModel : AbstractBookViewModel, IRefreshable
{
    public BooksViewModel(ILibrary library, SettingsViewModel settings, IStatusBar statusBar,
        IPopupService popupService) : base(library, statusBar, popupService)
    {
        Library.LibraryIdChanged += Handle_LibraryIdChanged;
        Library.BookList.CollectionChanged += Handle_BookListCollectionChanged;
        Library.TotalBooksChanged += Handle_TotalBooksChanged;
        IsBooksCollectionViewVisible = true;
        IsEditBookViewVisible = false;
        _bookManageable = new BookManagerModel(Library, settings, statusBar, popupService);
        ContentState = Constants.LOAD_CONTENT;
        ClearingState = Constants.CLEAR_CONTENT;
        ValidateOperations().GetAwaiter();
    }


    #region Public properties
    /// <summary>
    /// Gets or sets the content state.
    /// </summary>
    public string ContentState
    {
        get => _contentState;
        set => SetProperty(ref _contentState, value);
    }

    /// <summary>
    /// Gets or sets the clearing state.
    /// </summary>
    public string ClearingState
    {
        get => _clearingState;
        set => SetProperty(ref _clearingState, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the content can be cleared.
    /// </summary>
    public bool CanClearContent
    {
        get => _canClearContent;
        set => SetProperty(ref _canClearContent, value);
    }
    #endregion

    #region Public Methods
    protected override async Task PerformAction(string? commandParameter)
    {
        #if DEBUG
        await ShowNavigationCommandInDebug(commandParameter, nameof(BooksPage));
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
                    await TryGoToPage(commandParameter);
                    break;
                }
                case Constants.SELECTION_CHANGED:
                    CanOperateWithBooks = IsValidLibrary;
                    CanEditBook = IsBookSelected;
                    break;
                case Constants.CANCEL:
                {
                    IsBooksCollectionViewVisible = true;
                    IsEditBookViewVisible = false;
                    CanEditBook = IsBookSelected;
                    break;
                }
                case Constants.SAVE:
                {
                    if (Book.IsValid())
                    {
                        await _bookManageable.AddBookTask(Book);
                        WeakReferenceMessenger.Default.Send(new StatusMessage()
                        {
                            InfoKind = EInfoKind.CurrentInfo,
                            Message = $"Added the Book of '{Book.Author}'"
                        });
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
                    Book = BookModelMaker.BuildEmptyBook();
                    break;
                }
                case Constants.SAVE_CHANGES:
                {
                    if (Book.IsValid())
                    {
                        Book _book = null;
                        await RunInMainThreadAsync(() =>
                        {
                            _book = Library.BookList.FirstOrDefault(b => b.Id == Book.Id);
                            _book?.Set(Book);
                        });
                        WeakReferenceMessenger.Default.Send(new StatusMessage()
                        {
                            InfoKind = EInfoKind.CurrentInfo,
                            Message = $"Updated the Book of '{_book.Author}'"
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
                    if (!IsBookSelected)
                    {
                        await RefreshControlsOnAppearing();
                        return;
                    }

                    await RunInMainThreadAsync(() => Book = FirstSelectedBook.Clone());

                    OK = Constants.SAVE_CHANGES;

                    IsBooksCollectionViewVisible = false;
                    IsEditBookViewVisible = true;

                    break;
                }
                case Constants.EXPORT_BOOK:
                {
                    if (!IsBookSelected)
                    {
                        await RefreshControlsOnAppearing();
                        return;
                    }

                    // display window with input a new book name
                    var customDialog = await ShowCustomDialogPage(Constants.EXPORT_BOOK,
                        Constants.INPUT_NAME, true);

                    await RunInMainThreadAsync(() => Book = FirstSelectedBook);

                    var bookName = customDialog.IsOk && !string.IsNullOrEmpty(customDialog.InputString)
                        ? customDialog.InputString
                        : $"{Book.Author}. {Book.Title}";

                    var pathToContent = GetPathToFile(bookName);

                    if (await _bookManageable.TrySaveBook(new XmlBookKeeper(), Book, pathToContent))
                    {
                        WeakReferenceMessenger.Default.Send(new StatusMessage()
                        {
                            InfoKind = EInfoKind.CurrentInfo,
                            Message = $"Exported the Book of '{Book.Author}' by the path '{pathToContent}'"
                        });
                    }

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
                    // Performing actions at the BooksManager
                    await RunInMainThreadAsync(() => _selectedBooks = GetSelectedBooks());
                    await _bookManageable.RunCommand(commandParameter, _selectedBooks);
                    break;
                }
            }
        }
        else
        {
            #if DEBUG
            await ShowNavigationErrorInDebug(commandParameter, nameof(BooksViewModel));
            #endif
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

    protected override async Task HandlePostRefreshControlsOnAppearingTask()
    {
        await RunInMainThreadAsync(() =>
        {
            if (0 < SelectedBooks.Count)
                SelectedBooks.Clear();
        });
        await ValidateOperations();

        IsBooksCollectionViewVisible = true;
        IsEditBookViewVisible = false;
    }
    #endregion

    #region Private methods
    private void Handle_LibraryIdChanged(object? sender, EventArgs e)
    {
        SelectedBooks.Clear();
        ValidateOperations().GetAwaiter();
    }

    private async Task ValidateOperations()
    {
        await Task.Run(() =>
        {
            CanOperateWithBooks = IsValidLibrary;
            CanEditBook = IsBookSelected;
        });
    }

    private async void Handle_BookListCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Library.TotalBooks = Library.BookList.Count;
        SelectedBooks.Clear();
        await Task.Delay(50);
        await ValidateOperations();
    }

    private void Handle_TotalBooksChanged(object? sender, TotalBooksEventArgs e)
    {
        WeakReferenceMessenger.Default.Send(new StatusMessage()
            { InfoKind = EInfoKind.TotalBooks, Message = Library.TotalBooks.ToString() });
    }

    private Book? FirstSelectedBook => GetSelectedBooks()[0];
    private bool IsBookSelected => IsNotZero(SelectedBooks?.Count);


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
    private string _contentState;
    private string _clearingState;
    private bool _canClearContent;
    #endregion
}