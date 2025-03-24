using LibraryManager.Models;
using System.Collections.ObjectModel;
using System.Reflection;

namespace LibraryManager.AbstractObjects;
/// <summary>
/// Represents a library interface.
/// </summary>
/// <author>YR 2025-01-09</author>
public interface ILibrary 
{
    /// <summary>
    /// Sets the library.
    /// </summary>
    /// <param name="library">The library to set.</param>
    void Set(ILibrary? library);

    /// <summary>
    /// Unique identifier for the library.
    /// </summary>
    int Id
    {
        get; set;
    }

    /// <summary>
    /// Occurs when the library ID changes.
    /// </summary>
    event EventHandler<EventArgs> LibraryIdChanged;

    public event EventHandler<TotalBooksEventArgs>? TotalBooksChanged;

    /// <summary>
    /// Library name.
    /// </summary>
    string Name
    {
        get; set;
    }

    /// <summary>
    /// Description of the library.
    /// </summary>
    string Description
    {
        get; set;
    }

    /// <summary>
    /// total number of books in the library.
    /// </summary>
    int TotalBooks
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the collection of books in the library.
    /// </summary>
    ObservableCollection<Book> BookList
    {
        get; set;
    }

    /// <summary>
    /// Gets or sets an array of <see cref="PropertyInfo"/> objects representing the properties of <see cref="Book"/>.
    /// </summary>
    PropertyInfo[] GetBookPropertiesInfo();

    /// <summary>
    /// Gets an array of strings representing the property's Name of <see cref="Book"/>.
    /// </summary>
    /// <returns>An array of strings containing the property's Name of <see cref="Book"/>.</returns>
    string[] GetBookProperties();

    /// <summary>
    /// Finds the PropertyInfo of a book property by its name.
    /// </summary>
    /// <param name="name">The name of the book property to find.</param>
    /// <returns>The PropertyInfo of the book property, or null if not found.</returns>
    PropertyInfo FindBookPropertyInfo(string? name);
}
