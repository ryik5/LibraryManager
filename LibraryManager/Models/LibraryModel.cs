using System.Collections.ObjectModel;

namespace LibraryManager.Models;

public class LibraryModel
{
    public LibraryModel()
    {
        Id =new Random().Next();
        Name ="";
        Description = "";
    }

    public LibraryModel(ILibrary library)
    {
        Set(library);
    }
    
    // <summary>
    /// Sets the library.
    /// </summary>
    /// <param name="library">The library to set.</param>
    public void Set(ILibrary library)
    {
        Id = library.Id;
        Name = library.Name;
        Description = library.Description;
        BookList = library.BookList;
    }

    /// <summary>
    /// Gets or sets the unique identifier for the <see cref="Library"/>.
    /// </summary>
    public int Id;

    /// <summary>
    /// <see cref="Library"/> name.
    /// </summary>
    public string Name;

    /// <summary>
    /// Short description of the <see cref="Library"/>.
    /// </summary>
    public string Description;

    /// <summary>
    /// Gets or sets the collection of books in the <see cref="Library"/>.
    /// </summary>
    public ObservableCollection<Book> BookList=new();

    /// <summary>
    /// Total numbers of books in the <see cref="BookList"/> 
    /// </summary>
    public int TotalBooks => BookList.Count;

    /// <summary>
    /// Occurs when the library  <see cref="Id"/>  changes.
    /// </summary>
    public event EventHandler<EventArgs>? LibraryIdChanged;

    public override string ToString()
    {
        return $"{Id}, {Name},{Description},{string.Join(",", BookList)}";
    }

    public override int GetHashCode()
    {
        unchecked
        {
            if (TotalBooks == 0)
                return 0;

            int hash = 13;

            hash = (hash * 7) + (!ReferenceEquals(null, ToString()) ? ToString().GetHashCode() : 0);

            return hash;
        }
    }
}