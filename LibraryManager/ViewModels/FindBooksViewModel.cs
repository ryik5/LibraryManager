using LibraryManager.Extensions;
using LibraryManager.Models;
using LibraryManager.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace LibraryManager.ViewModels;

public class FindBooksViewModel : AbstractViewModel
{
    public FindBooksViewModel(ILibrary library)
    {
        Library = library;
       //  Library.TotalBooksChanged += BookList_CollectionChanged;
        SearchFields = Enum.GetValues(typeof(EBibliographicKindInformation)).Cast<EBibliographicKindInformation>()
            .ToList();
        FoundBookList.CollectionChanged += HandleFoundBookListChanged;
        CanEditBook = true;
        _bookManageable = new BookManagerModel(Library);
    }


    #region Public Properties
    public ILibrary Library { get; set; }

    public ObservableCollection<Book> FoundBookList
    {
        get => _foundBookList;
        set => SetProperty(ref _foundBookList, value);
    }

    public ObservableCollection<Book> SelectedBooks
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

    public bool CanEditBook
    {
        get => _canEditBook;
        set => SetProperty(ref _canEditBook, value);
    }
    #endregion

    #region Public Methods
    protected override async Task PerformAction(string? commandParameter)
    {
        Debug.WriteLine($"NavigateCommand triggered with commandParameter: {commandParameter}");

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
                {
                    try
                    {
                        // Dynamically navigate using the provided commandParameter
                        // begins '//' added in the beginning to switch a Menu as well as Page. without '//' it switch only Page
                        Shell.Current.GoToAsync($"//{commandParameter}").ConfigureAwait(false);
                    }
                    catch (Exception ex) // Handle any issues with navigation
                    {
                        #if DEBUG
                        Debug.WriteLine($"Navigation error: {ex.Message}");
                        #endif
                    }

                    break;
                }
                case Constants.FIND_BOOKS:
                    if (!ValidLibary())
                        return;

                    Book = null;
                    await FindBooksTask();

                    break;

                case Constants.EDIT_BOOK:
                    if (!ValidSelectedBooks())
                        return;
                    RunInMainThread(() => Book = SelectFirstFoundBook());

                    var editBookVM = new EditBookViewModel((Book)Book.Clone());
                    var editBookPage = new EditBookPage() { BindingContext = editBookVM };
                    await Application.Current?.MainPage?.Navigation.PushModalAsync(editBookPage)!;

                    if (await editBookPage.DialogResultTask.Task)
                    {
                        Book.Set(editBookVM.Book);
                    }

                    break;

                case Constants.DELETE_BOOK:
                    List<Book> selectedBooks=null;
                    RunInMainThread(() => selectedBooks = SelectedBooks.ToList() );
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
            #if DEBUG
            Debug.WriteLine(
                $"Navigation error path '{commandParameter}' in class '{nameof(FindBooksViewModel)}' by method '{nameof(PerformAction)}'");
            #endif
        }
        RunInMainThread(() =>
            {
                RaisePropertyChanged(nameof(Library));
                RaisePropertyChanged(nameof(Library.BookList));
            }
        );
    }
    #endregion


    #region Private methods
    private bool ValidSelectedBooks() => MoreZero(SelectedBooks?.Count ?? 0);

    private bool ValidLibary() => MoreZero(Library.TotalBooks);

    private bool MoreZero(int number)
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

    /// <summary>
    /// Handles the CollectionChanged event of the BookList.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The NotifyCollectionChangedEventArgs instance containing the event data.</param>
    private void HandleFoundBookListChanged(object? sender,
        System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        RunInMainThread(() =>
        {
            SelectedBooks.Clear();
            CanEditBook = 0 < SelectedBooks?.Count;
        });
    }

    private Book? SelectFirstFoundBook() => ValidSelectedBooks() ? SelectedBooks[0] : null;

    private async void BookList_CollectionChanged(object? sender, TotalBooksEventArgs e)
    {
        await FindBooksTask();
    }
    #endregion

    #region Private fields
    private ObservableCollection<Book> _selectedBooks = new();
    private Book _book;
    private ObservableCollection<Book> _foundBookList = new();
    private readonly BookManagerModel _bookManageable;
    private string _searchText;
    private bool _searchOnFly;
    private EBibliographicKindInformation _searchField;
    private bool _canEditBook;
    #endregion
}