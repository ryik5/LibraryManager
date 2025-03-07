using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using LibraryManager.Extensions;
using LibraryManager.Utils;

namespace LibraryManager.Models;

/// <summary>
/// Represents a model for managing books in a library.
/// </summary>
/// <author>YR 2025-01-09</author>
public class BookManagerModel : INotifyPropertyChanged, IBookManageable
{
    public BookManagerModel(ILibrary library)
    {
        if (library is null)
            throw new ArgumentNullException(nameof(library));

        _library = library;
    }


    #region public methods

    public Task RunCommand(string commandParameter)
    {
        switch (commandParameter)
        {
            case Constants.ADD_BOOK:
                var book = DemoBookMaker.GenerateBook();

                AddBook(book);

                break;

            default:
                break;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Try ti import a book from the specified file path into the Library.
    /// </summary>
    /// <param name="bookLoader">The loader responsible for loading the book.</param>
    /// <param name="pathToFile">The path to the file containing the book data.</param>
    /// <returns>True if the book was successfully loaded; otherwise, false.</returns>
    public bool TryLoadBook(IBookLoader bookLoader, string pathToFile)
    {
        bookLoader.LoadingFinished += BookLoader_LoadingBookFinished;

        var result = bookLoader.TryLoadBook(pathToFile, out var book);
        if (result)
            InvokeOnUiThread(() => AddBook(book));

        bookLoader.LoadingFinished -= BookLoader_LoadingBookFinished;

        return result;
    }

    /// <summary>
    /// Saves the selected book to the specified folder.
    /// </summary>
    /// <param name="keeper">The keeper responsible for saving the book.</param>
    /// <param name="pathToFolder">The path to the folder where the book will be saved.</param>
    /// <returns>True if the book was successfully saved; otherwise, false.</returns>
    public bool TrySaveBook(IBookKeeper keeper, Book book, string pathToFolder)
    {
        var result = false;
        InvokeOnUiThread(() => { result = keeper.TrySaveBook(book, pathToFolder); });
        return result;
    }

    /// <summary>
    /// Sorts the book collection by author and then by title.
    /// </summary>
    public void SortBooks()
    {
        InvokeOnUiThread(() =>
            Library.BookList.ResetAndAddRange(Library.BookList
                .OrderBy(b => b.Author)
                .ThenBy(b => b.Title)));
    }

    /// <summary>
    /// Sorts the book collection by author and then by title.
    /// </summary>
    public void SafetySortBooks(List<PropertyCustomInfo> sortProperties)
    {
        InvokeOnUiThread(() =>
            Library.BookList.ResetAndAddRange(GetSortedBookList(sortProperties)));
    }

    /// <summary>
    /// Adds a book to the library.
    /// </summary>
    /// <param name="book">The book to add.</param>
    public void AddBook(Book book)
    {
        InvokeOnUiThread(() =>
        {
            book.Id = _library.BookList.Count == 0 ? 1 : _library.BookList.Max(b => b.Id) + 1;
            _library.BookList.Add(book);
        });
    }

    /// <summary>
    /// Removes a book from the library.
    /// </summary>
    /// <param name="book">The book to remove.</param>
    /// <returns>True if the book was successfully removed; otherwise, false.</returns>
    public bool TryRemoveBook(Book book) => Library.BookList.RemoveItem(book);

    /// <summary>
    /// Finds books in the library by a specified book element.
    /// </summary>
    /// <param name="bookElement">The element of the book to search by.</param>
    /// <param name="partElement">The value of the element to search for.</param>
    /// <returns>A list of books that match the search criteria.</returns>
    public List<Book> FindBooksByKind(EBibliographicKindInformation bookElement, object partElement)
    {
        IEnumerable<Book> tmpResult = ImmutableArray<Book>.Empty;
        var strElement = partElement?.ToString();

        InvokeOnUiThread(() => tmpResult = FindBooks(bookElement, strElement));

        return tmpResult?.ToList() ?? new List<Book>(0);
    }

    #endregion


    #region Properties

    /// <summary>
    /// Gets or sets a library.
    /// </summary>
    public ILibrary Library
    {
        get => _library;
        set => SetProperty(ref _library, value);
    }

    public event EventHandler<ActionFinishedEventArgs> LoadingFinished;
    public event EventHandler<TotalBooksEventArgs> TotalBooksChanged;

    #endregion


    #region private methods

    /// <summary>
    /// Returns a sorted list of books based on the provided properties.
    /// </summary>
    /// <param name="sortProperties">The properties to sort the books by.</param>
    /// <returns>A sorted list of Book objects.</returns>
    private IEnumerable<Book> GetSortedBookList(List<PropertyCustomInfo> sortProperties)
    {
        var orderedBooks = Library.BookList.Where(b => 0 < b.Id);

        foreach (var property in sortProperties)
        {
            if (orderedBooks is IOrderedEnumerable<Book>)
            {
                if (property.DescendingOrder)
                    orderedBooks =
                        ((IOrderedEnumerable<Book>)orderedBooks).ThenByDescending(
                            b => property.PropertyInfo.GetValue(b));
                else
                    orderedBooks =
                        ((IOrderedEnumerable<Book>)orderedBooks).ThenBy(b => property.PropertyInfo.GetValue(b));
            }
            else
            {
                if (property.DescendingOrder)
                    orderedBooks = orderedBooks.OrderByDescending(b => property.PropertyInfo.GetValue(b));
                else
                    orderedBooks = orderedBooks.OrderBy(b => property.PropertyInfo.GetValue(b));
            }
        }

        return orderedBooks;
    }

     private void BookList_CollectionChanged(object? sender,
        System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        TotalBooksChanged.Invoke(this, new TotalBooksEventArgs { TotalBooks = Library.BookList?.Count ?? 0 });
    }

    private void BookLoader_LoadingBookFinished(object? sender, ActionFinishedEventArgs e)
    {
        LoadingFinished?.Invoke(this, new ActionFinishedEventArgs { Message = e.Message, IsFinished = e.IsFinished });
    }

    /// <summary>
    /// Finds books in the library by a specified book element.
    /// </summary>
    /// <param name="bookElement">The element of the book to search by.</param>
    /// <param name="strElement">The value of the element to search for.</param>
    /// <returns>An enumerable collection of books that match the search criteria.</returns>
    private IEnumerable<Book> FindBooks(EBibliographicKindInformation bookElement, string? strElement)
    {
        IEnumerable<Book> tmpResult = ImmutableArray<Book>.Empty;
        switch (bookElement)
        {
            case EBibliographicKindInformation.Author:
                tmpResult = FindBooksByAuthor(strElement);
                break;
            case EBibliographicKindInformation.Title:
                tmpResult = FindBooksByTitle(strElement);
                break;
            case EBibliographicKindInformation.TotalPages:
                tmpResult = FindBooksByTotalPages(strElement);
                break;
            case EBibliographicKindInformation.PublishDate:
                tmpResult = FindBooksByPublishDate(strElement);
                break;
            default:
                tmpResult = FindBooksAnyWhere(strElement);
                break;
        }

        return tmpResult;
    }

    /// <summary>
    /// Finds books in the library by author.
    /// </summary>
    /// <param name="strElement">The author to search for.</param>
    /// <returns>An enumerable collection of books that match the search criteria.</returns>
    private IEnumerable<Book> FindBooksByAuthor(string? strElement) => IsNotNullOrEmpty(strElement)
        ? Library.BookList.Where(b => b.Author.Contains(strElement, CurrentComparisionRule))
        : ImmutableArray<Book>.Empty;

    /// <summary>
    /// Finds books in the library by title.
    /// </summary>
    /// <param name="strElement">The title to search for.</param>
    /// <returns>An enumerable collection of books that match the search criteria.</returns>
    private IEnumerable<Book> FindBooksByTitle(string? strElement)
        => IsNotNullOrEmpty(strElement)
            ? Library.BookList.Where(b => b.Title.Contains(strElement, CurrentComparisionRule))
            : ImmutableArray<Book>.Empty;

    /// <summary>
    /// Finds books in the library by total pages.
    /// </summary>
    /// <param name="strElement">The total pages to search for.</param>
    /// <returns>An enumerable collection of books that match the search criteria.</returns>
    private IEnumerable<Book> FindBooksByTotalPages(string? strElement)
        => IsParseable(strElement, out var intElement)
            ? Library.BookList.Where(b => b.TotalPages == intElement)
            : ImmutableArray<Book>.Empty;

    /// <summary>
    /// Finds books in the library by publish date.
    /// </summary>
    /// <param name="strElement">The publishing year to search for.</param>
    /// <returns>An enumerable collection of books that match the search criteria.</returns>
    private IEnumerable<Book> FindBooksByPublishDate(string? strElement)
        => IsParseable(strElement, out var intElement)
            ? Library.BookList.Where(b => b.Year == intElement)
            : ImmutableArray<Book>.Empty;

    private IEnumerable<Book> FindBooksAnyWhere(string? strElement)
    {
        IEnumerable<Book> tmpResult = ImmutableArray<Book>.Empty;
        var isInt = IsParseable(strElement, out var intElement);
        var isString = IsNotNullOrEmpty(strElement);

        if (isInt)
            tmpResult = Library.BookList.Where(b =>
                b.TotalPages == intElement ||
                b.Year == intElement ||
                (b.Author?.Contains(strElement, CurrentComparisionRule) ?? false) ||
                (b.Title?.Contains(strElement, CurrentComparisionRule) ?? false));
        else
            tmpResult = isString
                ? Library.BookList.Where(b =>
                    (b.Author?.Contains(strElement, CurrentComparisionRule) ?? false) ||
                    (b.Description?.Contains(strElement, CurrentComparisionRule) ?? false) ||
                    (b.Genre?.Contains(strElement, CurrentComparisionRule) ?? false) ||
                    (b.ISBN?.Contains(strElement, CurrentComparisionRule) ?? false) ||
                    (b.Title?.Contains(strElement, CurrentComparisionRule) ?? false))
                : ImmutableArray<Book>.Empty;

        return tmpResult ?? ImmutableArray<Book>.Empty;
    }

    /// <summary>
    /// Determines whether the specified string can be parsed to an integer.
    /// </summary>
    /// <param name="strElement">The string to parse.</param>
    /// <param name="intElement">The parsed integer value.</param>
    /// <returns>True if the string can be parsed to an integer; otherwise, false.</returns>
    private bool IsParseable(string? strElement, out int intElement) => int.TryParse(strElement, out intElement);

    /// <summary>
    /// Determines whether the specified string is not null or empty.
    /// </summary>
    /// <param name="strElement">The string to check.</param>
    /// <returns>True if the string is not null or empty; otherwise, false.</returns>
    private bool IsNotNullOrEmpty(string? strElement) => !string.IsNullOrEmpty(strElement);

    private void InvokeOnUiThread(Action action) => MainThread.BeginInvokeOnMainThread(action);

    private const StringComparison CurrentComparisionRule = StringComparison.OrdinalIgnoreCase;

    private ILibrary _library;

    #endregion

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        RaisePropertyChanged(propertyName);
        return true;
    }
}