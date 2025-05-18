using CommunityToolkit.Mvvm.Messaging;
using System.Collections.Immutable;
using LibraryManager.AbstractObjects;
using LibraryManager.Extensions;
using LibraryManager.Utils;
using LibraryManager.ViewModels;
using System.Diagnostics;
using System.Xml;

namespace LibraryManager.Models;

/// <summary>
/// Represents a model for managing books in a library.
/// </summary>
/// <author>YR 2025-01-09</author>
public class BookManagerModel : AbstractBindableModel, IBookManageable
{
    public BookManagerModel(ILibrary library, SettingsViewModel settings, IStatusBar statusBar)
    {
        _settings = settings;
        _statusBar = statusBar;

        if (library is null)
            throw new ArgumentNullException(nameof(library));

        _library = library;
    }


    #region public methods
    public async Task RunCommand(string commandParameter, IList<Book>? selectedBooks)
    {
        Book book;
        switch (commandParameter)
        {
            case Constants.IMPORT_BOOK:
            {
                var result = await TryLoadBook();
                WeakReferenceMessenger.Default.Send(
                    new StatusMessage()
                    {
                        InfoKind = EInfoKind.CurrentInfo,
                        Message = result
                            ? $"Book of '{_temporaryBook.Author}' was successfully imported."
                            : "Book was not loaded."
                    });

                break;
            }
            case Constants.ADD_BOOK:
            {
                await AddBookTask(BookModelMaker.GenerateDemoBook());
                WeakReferenceMessenger.Default.Send(
                    new StatusMessage() { InfoKind = EInfoKind.CurrentInfo, Message = $"Book was added." });
                break;
            }
            case Constants.DEMO_ADD_BOOKS:
            {
                for (var b = 0; b < 10; b++)
                    await AddBookTask(BookModelMaker.GenerateDemoBook());
                WeakReferenceMessenger.Default.Send(
                    new StatusMessage() { InfoKind = EInfoKind.CurrentInfo, Message = "10 Books were added." });
                break;
            }
            case Constants.DELETE_BOOK:
            {
                if (selectedBooks is not null && 0 < selectedBooks.Count)
                {
                    foreach (var bookToDelete in selectedBooks)
                    {
                        TryRemoveBook(bookToDelete).ConfigureAwait(false);
                    }

                    WeakReferenceMessenger.Default.Send(
                        new StatusMessage()
                            { InfoKind = EInfoKind.CurrentInfo, Message = $"Deleted {selectedBooks.Count} book(s)." });
                }

                break;
            }

            case Constants.LOAD_CONTENT:
            case Constants.LOAD_COVER:
            case Constants.SAVE_CONTENT:
            case Constants.CLEAR_CONTENT:
            {
                await EditBook(commandParameter, selectedBooks[0]);
                break;
            }
            case Constants.SORT_BOOKS:
            {
                if (await MakeSortingList() is { Count: > 0 } props)
                {
                    await SafetySortBooks(props);

                    string msg = String.Empty;
                    foreach (var prop in props)
                    {
                        if (prop.PropertyInfo.Name != nameof(Book.None))
                        {
                            if (!string.IsNullOrWhiteSpace(msg = String.Empty))
                                msg += ", then ";
                            msg +=
                                $"sorted by {prop.PropertyInfo.Name} {(prop.DescendingOrder ? "descending" : "ascending")}";
                        }
                    }

                    WeakReferenceMessenger.Default.Send(new StatusMessage()
                    {
                        InfoKind = EInfoKind.DebugInfo,
                        LogLevel = ELogLevel.Info,
                        Message = $"Library ID:'{_library.Id}' was {msg}"
                    });
                }

                break;
            }
        }
    }

    public async Task<Book> EditBook(string commandParameter, Book book)
    {
        switch (commandParameter)
        {
            case Constants.LOAD_CONTENT:
            {
                var result = await TryLoadContentToBookTask(book);
                if (result)
                {
                    WeakReferenceMessenger.Default.Send(new StatusMessage()
                    {
                        InfoKind = EInfoKind.DebugInfo,
                        LogLevel = ELogLevel.Info,
                        Message =
                            $"It was loaded a new content '{book.Content.Name}' into a book ID:'{book.Id}' with title '{book.Title}' by author '{book.Author}'."
                    });
                }

                break;
            }
            case Constants.LOAD_COVER:
            {
                var result = await TryLoadCoverBookTask(book);
                if (result)
                {
                    WeakReferenceMessenger.Default.Send(new StatusMessage()
                    {
                        InfoKind = EInfoKind.DebugInfo,
                        LogLevel = ELogLevel.Info,
                        Message =
                            $"It was added a new cover into a book ID:'{book.Id}' with a title '{book.Title}' by author '{book.Author}'."
                    });
                }

                break;
            }
            case Constants.SAVE_CONTENT:
            {
                var result = await TrySaveBookContentTask(book);
                if (result)
                {
                    WeakReferenceMessenger.Default.Send(new StatusMessage()
                    {
                        InfoKind = EInfoKind.DebugInfo,
                        LogLevel = ELogLevel.Info,
                        Message =
                            $"It was saved content on a storage of the book ID:'{book.Id}' with a title '{book.Title}' by author '{book.Author}'."
                    });
                }

                break;
            }
            case Constants.CLEAR_CONTENT:
            {
                book.Content = null;
                WeakReferenceMessenger.Default.Send(new StatusMessage()
                {
                    InfoKind = EInfoKind.DebugInfo,
                    LogLevel = ELogLevel.Info,
                    Message =
                        $"It was removed content from the book ID:'{book.Id}' with a title '{book.Title}' by author '{book.Author}'."
                });

                break;
            }
        }

        return book;
    }

    /// <summary>
    /// Try ti import a book from the specified file path into the Library.
    /// </summary>
    /// <param name="bookLoader">The loader responsible for loading the book.</param>
    /// <param name="pathToFile">The path to the file containing the book data.</param>
    /// <returns>True if the book was successfully loaded; otherwise, false.</returns>
    public bool TryLoadBook(IBookLoader bookLoader, string pathToFile)
    {
        var result = bookLoader.TryLoadBook(pathToFile, out var book);
        if (result)
            Task.Run(async () => await AddBookTask(book));

        return result;
    }


    private async Task<bool> TryLoadBook()
    {
        return await DeserializeBookTask(await TryPickFileUpTask("Please select a book file", new string[] { "xml" }));
    }

    private async Task<bool> DeserializeBookTask(FileResult fileResult)
    {
        await using var fileStream = await fileResult.OpenReadAsync();
        using var xmlReader = XmlReader.Create(fileStream, new XmlReaderSettings
        {
            IgnoreWhitespace = false,
            IgnoreComments = false
        });

        _temporaryBook = await Task.Run(() =>
            (Book)(new System.Xml.Serialization.XmlSerializer(typeof(Book))).Deserialize(xmlReader));
        if (_temporaryBook?.IsValid() ?? false)
        {
            await AddBookTask(_temporaryBook);
            return true;
        }

        return false;
    }


    /// <summary>
    /// Saves the selected book to the specified folder.
    /// </summary>
    /// <param name="keeper">The keeper responsible for saving the book.</param>
    /// <param name="pathToFile">The path to the folder where the book will be saved.</param>
    /// <returns>True if the book was successfully saved; otherwise, false.</returns>
    public Task<bool> TrySaveBook(IBookKeeper keeper, Book book, string pathToFile)
    {
        var result = keeper.TrySaveBook(book, pathToFile);
        WeakReferenceMessenger.Default.Send(
            new StatusMessage()
            {
                InfoKind = EInfoKind.CurrentInfo,
                Message =
                    $"Export a book named '{book.Title}' by '{book.Author}' in '{pathToFile}' was {(result ? "Successful" : "Failed")}"
            });

        return Task.FromResult(result);
    }

    /// <summary>
    /// Sorts the book collection by dedicated properties.
    /// </summary>
    public Task SafetySortBooks(List<PropertyCustomInfo> sortProperties)
    {
        RunInMainThread(() =>
            Library.BookList.ResetAndAddRange(GetSortedBookList(sortProperties)));
        return Task.CompletedTask;
    }


    /// <summary>
    /// Adds a new book to the <see cref="Library.BookList"/>
    /// </summary>
    /// <param name="book">The book to add.</param>
    public Task AddBookTask(Book book)
    {
        RunInMainThread(() =>
        {
            book.Id = _library.BookList.Count == 0 ? 1 : _library.BookList.Max(b => b.Id) + 1;
            _library.BookList.Insert(0, book);
        });
        return Task.CompletedTask;
    }

    /// <summary>
    /// Removes a book from the library.
    /// </summary>
    /// <param name="book">The book to remove.</param>
    /// <returns>True if the book was successfully removed; otherwise, false.</returns>
    public async Task TryRemoveBook(Book book) => await RunInMainThreadAsync(() =>
    {
        try
        {
            Library.BookList.RemoveItem(book);
        }
        catch
        {
        }
    });

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

        RunInMainThread(() => tmpResult = FindBooks(bookElement, strElement));

        WeakReferenceMessenger.Default.Send(
            new StatusMessage()
            {
                InfoKind = EInfoKind.CurrentInfo,
                Message = $"Find books by '{bookElement}'where part of the element is {partElement}."
            });

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
        ? Library.BookList.Where(b => b.Author.Contains(strElement, CURRENT_COMPARISION_RULE))
        : ImmutableArray<Book>.Empty;

    /// <summary>
    /// Finds books in the library by title.
    /// </summary>
    /// <param name="strElement">The title to search for.</param>
    /// <returns>An enumerable collection of books that match the search criteria.</returns>
    private IEnumerable<Book> FindBooksByTitle(string? strElement)
        => IsNotNullOrEmpty(strElement)
            ? Library.BookList.Where(b => b.Title.Contains(strElement, CURRENT_COMPARISION_RULE))
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
                (b.Author?.Contains(strElement, CURRENT_COMPARISION_RULE) ?? false) ||
                (b.Title?.Contains(strElement, CURRENT_COMPARISION_RULE) ?? false));
        else
            tmpResult = isString
                ? Library.BookList.Where(b =>
                    (b.Author?.Contains(strElement, CURRENT_COMPARISION_RULE) ?? false) ||
                    (b.Description?.Contains(strElement, CURRENT_COMPARISION_RULE) ?? false) ||
                    (b.Genre?.Contains(strElement, CURRENT_COMPARISION_RULE) ?? false) ||
                    (b.ISBN?.Contains(strElement, CURRENT_COMPARISION_RULE) ?? false) ||
                    (b.Title?.Contains(strElement, CURRENT_COMPARISION_RULE) ?? false))
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

    private async Task<bool> TrySaveBookContentTask(Book book)
    {
        var customDialog = await ShowCustomDialogPage(Constants.SAVE_CONTENT,
            Constants.INPUT_NAME, true);

        var contentName = customDialog.IsOk && !string.IsNullOrEmpty(customDialog.InputString)
            ? customDialog.InputString
            : $"{book.Content.Name}";

        var pathToContent = GetPathToFile(contentName, book.Content.Ext);

        return await SaveContentToDiskTask(book.Content, pathToContent);
    }

    private async Task<bool> SaveContentToDiskTask(MediaData bookContent, string pathToContent)
    {
        try
        {
            await File.WriteAllBytesAsync($"{pathToContent}", bookContent.ObjectByteArray);
            return true;
        }
        catch (Exception ex)
        {
            #if DEBUG
            Debug.WriteLine(ex.Message);
            #endif
            return false;
        }
    }

    private async Task<bool> TryLoadContentToBookTask(Book book)
    {
        var settings = new SettingsViewModel();
        var length = settings.Book_MaxContentLength;
        var fileResult = await TryPickFileUpTask("Select book content", null);
        var readContentTask = await ReadContentFromDiskTask(book, fileResult, maxContentLength: length);
 
        return readContentTask.IsSuccess;
    }

    private async Task<ResultBook> ReadContentFromDiskTask(Book book, FileResult fileResult, long maxContentLength)
    {
        var fileInfo = new FileInfo(fileResult.FileName);
        if (book.Content is null)
        {
            book.Content = new MediaData
            {
                Name = fileInfo.Name,
                OriginalPath = fileResult.FullPath,
                Ext = fileInfo.Extension,
                IsContentStoredSeparately = true,
                IsLoaded = false
            };
        }
        else
        {
            book.Content.Name = fileInfo.Name;
            book.Content.OriginalPath = fileResult.FullPath;
            book.Content.Ext = fileInfo.Extension;
            book.Content.IsContentStoredSeparately = true;
            book.Content.IsLoaded = false;
        }

        try
        {
            var stream = await fileResult.OpenReadAsync();
            if (stream.Length < maxContentLength)
            {
                book.Content.ObjectByteArray = await ConvertStreamToByteArray(stream);
                book.Content.IsLoaded = true;
                book.Content.IsContentStoredSeparately = false;
            }
            else
            {
                book.Content.IsLoaded = true;
                book.Content.IsContentStoredSeparately = true;
            }

            return new ResultBook { Book = book, IsSuccess = true };
        }
        catch
        {
            book.Content.ObjectByteArray = null;
        }

        return new ResultBook { Book = book, IsSuccess = false };
    }


    private async Task<bool> TryLoadCoverBookTask(Book book)
    {
        // TODO : add parameter for a book cover length
        var length = 10_000_000; // _settings.Book_MaxContentLength;
        var fileResult =
            await TryPickFileUpTask("Load book cover", new[] { "jpg", "png", "jpeg", "gif", "bmp", "tiff", "" });
        var readContentTask = await LoadDataFromDiskTask(book, fileResult, maxContentLength: length);
        if (readContentTask.IsSuccess)
            book.Set(readContentTask.Book);

        return readContentTask.IsSuccess;
    }

    private async Task<ResultBook> LoadDataFromDiskTask(Book? book, FileResult fileResult, long maxContentLength)
    {
        book.Content ??= new MediaData();
        var result = false;
        var oldCover = book.Content.BookCoverByteArray;
        try
        {
            var stream = await fileResult.OpenReadAsync();
            if (stream.Length < maxContentLength)
            {
                book.Content.BookCoverByteArray = await ConvertStreamToByteArray(stream);
                result = true;
            }
        }
        catch
        {
            book.Content.BookCoverByteArray = oldCover;
            result = false;
        }

        return new ResultBook { Book = book, IsSuccess = result };
    }


    private Task<byte[]> ConvertStreamToByteArray(Stream input)
    {
        byte[] buffer = new byte[16 * 1024];
        using (MemoryStream ms = new MemoryStream())
        {
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }

            return Task.FromResult(ms.ToArray());
        }
    }


    /// <summary>
    /// Makes sorting property name list.
    /// </summary>
    private Task<List<PropertyCustomInfo>> MakeSortingList()
    {
        var props = new List<PropertyCustomInfo>();

        MakeBookCustomPropertyList(props, _settings.FirstSortBookProperty, _settings.FirstSortProperty_ByDescend);
        MakeBookCustomPropertyList(props, _settings.SecondSortBookProperty, _settings.SecondSortProperty_ByDescend);
        MakeBookCustomPropertyList(props, _settings.ThirdSortBookProperty, _settings.ThirdSortProperty_ByDescend);

        return Task.FromResult(props);

        void MakeBookCustomPropertyList(List<PropertyCustomInfo> props, string name, bool byDescend)
        {
            var prop = Library.FindBookPropertyInfo(name);
            var customProp = new PropertyCustomInfo { PropertyInfo = prop, DescendingOrder = byDescend };
            if (prop.Name != nameof(Book.None))
                props.Add(customProp);
        }
    }

    private const StringComparison CURRENT_COMPARISION_RULE = StringComparison.OrdinalIgnoreCase;
    private ILibrary _library;
    private SettingsViewModel _settings;
    private readonly IStatusBar _statusBar;
    #endregion

    private Book _temporaryBook;
}