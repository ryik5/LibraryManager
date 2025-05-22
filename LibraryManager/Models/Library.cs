using CommunityToolkit.Mvvm.ComponentModel;
using LibraryManager.AbstractObjects;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using LibraryManager.Extensions;

namespace LibraryManager.Models;

[Serializable]
public class Library : ObservableObject, ILibrary, IXmlSerializable
{
    /// <summary>
    /// Sets the library.
    /// </summary>
    /// <param name="library">The library to set.</param>
    public void Set(ILibrary library)
    {
        Id = library.Id;
        Name = library.Name;
        Description = library.Description;
        _bookList.ResetAndAddRange(library.BookList);
    }

    /// <summary>
    /// Gets or sets the unique identifier for the <see cref="Library"/>.
    /// </summary>
    public int Id
    {
        get => _id;
        set
        {
            if (SetProperty(ref _id, value))
            {
                LibraryIdChanged?.Invoke(this, new EventArgs());
            }
        }
    }

    /// <summary>
    /// <see cref="Library"/> name.
    /// </summary>
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    /// <summary>
    /// Short description of the <see cref="Library"/>.
    /// </summary>
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    /// <summary>
    /// Gets or sets the collection of books in the <see cref="Library"/>.
    /// </summary>
    [XmlArray]
    public ObservableCollection<Book> BookList
    {
        get => _bookList;
        set => SetProperty(ref _bookList, value);
    }

    /// <summary>
    /// Total numbers of books in the <see cref="BookList"/> 
    /// </summary>
    [XmlIgnore]
    public int TotalBooks
    {
        get => _totalBooks;
        set
        {
            _totalBooks = value;
            TotalBooksChanged?.Invoke(this, new TotalBooksEventArgs { TotalBooks = value });
        }
    }


    /// <summary>
    /// Occurs when the library  <see cref="Id"/>  changes.
    /// </summary>
    public event EventHandler<EventArgs>? LibraryIdChanged;

    public event EventHandler<TotalBooksEventArgs>? TotalBooksChanged;

    public XmlSchema? GetSchema() => throw new NotImplementedException();

    /// <summary>
    /// Writes the <see cref="Library"/> to the specified XML writer.
    /// </summary>
    /// <param name="writer">The XML writer to write the <see cref="Library"/> to.</param>
    public void WriteXml(XmlWriter writer)
    {
        writer.WriteElementString(nameof(Id), Id.ToString());
        writer.WriteElementString(nameof(Name), Name);
        writer.WriteElementString(nameof(Description), Description);

        writer.WriteStartElement(nameof(BookList));
        foreach (var book in BookList)
            book.WriteXml(writer);

        writer.WriteEndElement();
    }

    /// <summary>
    /// Reads the <see cref="Library"/> from the specified XML reader.
    /// </summary>
    /// <param name="reader">The XML reader to read the <see cref="Library"/> from.</param>
    public void ReadXml(XmlReader reader)
    {
        do
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.Name)
                {
                    case nameof(Id):
                        Id = reader.ReadElementContentAsInt();
                        break;
                    case nameof(Name):
                        Name = reader.ReadElementContentAsString();
                        break;
                    case nameof(Description):
                        Description = reader.ReadElementContentAsString();
                        break;
                    case nameof(BookList):
                        BookList = new ObservableCollection<Book>();
                        while (reader.Name == "Book" || reader.Name == nameof(BookList) || !reader.EOF)
                        {
                            var book = new Book() { Author = "", Title = "", TotalPages = 0, Id = 0 };
                            book.ReadXml(reader);
                            if (book.Id != 0)
                                BookList.Add(book);
                        }

                        break;
                }
            }
        } while (!reader.EOF && reader.Read());
    }

    public bool IsNew
    {
        get => _isNew;
        set => SetProperty(ref _isNew, value);
    }


    /// <summary>
    /// Gets or sets an array of <see cref="PropertyInfo"/> objects representing the properties of <see cref="Book"/>.
    /// </summary>
    public PropertyInfo[] GetBookPropertiesInfo() => BookPropertiesInfo;

    /// <summary>
    /// Gets an array of strings representing the property's Name of <see cref="Book"/>.
    /// </summary>
    /// <returns>An array of strings containing the property's Name of <see cref="Book"/>.</returns>
    public string[] GetBookProperties() => BookPropertiesString;

    /// <summary>
    /// Finds the <see cref="PropertyInfo"/> of a <see cref="Book"/> property by its name.
    /// </summary>
    /// <param name="name">The name of the book property to find.</param>
    /// <returns>The PropertyInfo of the book property, or null if not found.</returns>
    public PropertyInfo FindBookPropertyInfo(string? name)
        => BookPropertiesInfo.FirstOrDefault(p => p.Name == (name ?? nameof(Book.None)))
           ?? BookPropertiesInfo.FirstOrDefault(p => p.Name == nameof(Book.None));


    private PropertyInfo[] BookPropertiesInfo
    {
        get
        {
            if (_bookPropertiesInfo is null)
            {
                lock (_locker)
                    _bookPropertiesInfo ??= BuildBookPropertiesArray();
            }

            return _bookPropertiesInfo;
        }
    }

    private string[] BookPropertiesString
    {
        get
        {
            if (_bookPropertiesInfo is null)
            {
                lock (_locker)
                {
                    _bookPropertiesInfo ??= BuildBookPropertiesArray();
                    _bookProperties ??= _bookPropertiesInfo.Select(p => p.Name).ToArray();
                }
            }

            return _bookProperties;
        }
    }

    private PropertyInfo[] BuildBookPropertiesArray()
    {
        var bookType = typeof(Book);
        return bookType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute<BookPropertyAttribute>() != null)
            .ToArray();
    }


    #region private fields
    private int _id;
    private int _totalBooks;
    private string _name = string.Empty;
    private string _description = string.Empty;
    private ObservableCollection<Book> _bookList = new();
    private PropertyInfo[] _bookPropertiesInfo;
    private string[] _bookProperties;
    private readonly object _locker = new();
    private bool _isNew = true;
    #endregion
}