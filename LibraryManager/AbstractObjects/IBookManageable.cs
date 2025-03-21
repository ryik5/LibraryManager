namespace LibraryManager.Models;

/// <summary>
/// Represents a Book interface that provides functionalities to add, remove, sort, and display books.
/// </summary>
/// <author>YR 2025-01-09</author>
public interface IBookManageable
{
    public event EventHandler<TotalBooksEventArgs> TotalBooksChanged;
    public event EventHandler<ActionFinishedEventArgs> LoadingFinished;

    /// <summary>
    /// Executes a specified command within the context of book management.
    /// </summary>
    /// <param name="commandParameter">The parameter or name of the command to execute.</param>
    /// <param name="selectedBooks">List of Books to operate</param>
    /// <remarks>
    /// This method provides a way to run specific functions or operations using a command parameter,
    /// enabling additional extensibility or dynamic behaviors based on the provided input.
    /// </remarks>
    Task RunCommand(string commandParameter, IList<Book>? selectedBooks);

    /// <summary>
    /// Sorts books in the library.
    /// </summary>
    void SortBooks();

    /// <summary>
    /// Adds a book to the library.
    /// </summary>
    /// <param name="book">The book to add.</param>
    Task AddBookTask(Book book);

    /// <summary>
    /// Removes the specified book from the library.
    /// </summary>
    /// <param name="book">The book to remove.</param>
    /// <returns>true if the book was successfully removed; otherwise, false.</returns>
    bool TryRemoveBook(Book book);

    /// <summary>
    /// Loads a book from the specified file path.
    /// </summary>
    /// <param name="bookLoader">The loader responsible for loading the book.</param>
    /// <param name="pathToFile">The path to the file containing the book data.</param>
    /// <returns>True if the book was successfully loaded; otherwise, false.</returns>
    bool TryLoadBook(IBookLoader bookLoader, string pathToFile);

    Task<bool> TryLoadBook();

    /// <summary>
    /// Saves the selected book to the specified folder.
    /// </summary>
    /// <param name="keeper">The keeper responsible for saving the book.</param>
    /// <param name="pathToFile">The path to the folder where the book will be saved.</param>
    /// <returns>True if the book was successfully saved; otherwise, false.</returns>
    Task<bool> TrySaveBook(IBookKeeper keeper, Book book, string pathToFile);

    /// <summary>
    /// Finds books by a specific element of the <see cref="Book"/>.
    /// </summary>
    /// <param name="bookElement">The element of the book to search by.</param>
    /// <param name="partOfElement">The value to search for within the specified element.</param>
    /// <returns>A list of books that match the search criteria.</returns>
    List<Book> FindBooksByKind(EBibliographicKindInformation bookElement, object partOfElement);

    /// <summary>
    /// Sorts the books in the <see cref="ILibrary"/> based on the provided properties.
    /// </summary>
    /// <param name="sortProperties">The properties to sort the books by.</param>
    void SafetySortBooks(List<PropertyCustomInfo> sortProperties);

    /// <summary>
    /// Gets or sets the library.
    /// </summary>
    ILibrary Library { get; set; }
}