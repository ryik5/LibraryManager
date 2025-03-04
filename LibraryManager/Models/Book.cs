using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace LibraryManager.Models;

[Serializable]
public class Book : ICloneable, INotifyPropertyChanged, IXmlSerializable
{
     /// <summary>
    /// Dump property to sort nothing
    /// </summary>
    [XmlIgnore]
    [BookProperty]
    public string None
    {
        get;
    }

    /// <summary>
    /// Gets or sets the unique identifier for the <see cref="Book"/>.
    /// </summary>
    [BookProperty]
    public required int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }
    private int _id;

    /// <summary>
    /// Gets or sets the author of the <see cref="Book"/>.
    /// </summary>
    [BookProperty]
    public required string Author
    {
        get => _author;
        set => SetProperty(ref _author, value);
    }
    private string _author;

    /// <summary>
    /// Gets or sets the title of the <see cref="Book"/>.
    /// </summary>
    [BookProperty]
    public required string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }
    private string _title;

    /// <summary>
    /// Gets or sets the publication date of the <see cref="Book"/>.
    /// </summary>
    [BookProperty]
    public int Year
    {
        get => _year;
        set => SetProperty(ref _year, value);
    }
    private int _year;

    /// <summary>
    /// Gets or sets the number of total pages in the <see cref="Book"/>.
    /// </summary>
    [BookProperty]
    public required int TotalPages
    {
        get => _totalPages;
        set => SetProperty(ref _totalPages, value);
    }
    private int _totalPages;

    /// <summary>
    /// Gets or sets the description of the <see cref="Book"/>.
    /// </summary>
    [BookProperty]
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }
    private string _description;

    /// <summary>
    /// Gets or sets the genre of the <see cref="Book"/>.
    /// </summary>
    [BookProperty]
    public string Genre
    {
        get => _genre;
        set => SetProperty(ref _genre, value);
    }
    private string _genre;

    /// <summary>
    /// Gets or sets the ISBN of the <see cref="Book"/>.
    /// </summary>
    [BookProperty]
    public string ISBN
    {
        get => _isbn;
        set => SetProperty(ref _isbn, value);
    }
    private string _isbn;

    /// <summary>
    /// Gets or sets the media content of the <see cref="Book"/>.
    /// </summary>
    public MediaData Content
    {
        get => _content;
        set => SetProperty(ref _content, value);
    }
    private MediaData _content;

    /// <summary>
    /// Returns a string that represents the current <see cref="Book"/>.
    /// </summary>
    /// <returns>A string that contains the <see cref="Author"/>, <see cref="Title"/>, <see cref="TotalPages"/> and <see cref="Year"/>
    /// of the <see cref="Book"/>.</returns>
    public override string ToString()
    {
        return $"{Id},{Author},{Title},{Year},{TotalPages},{Description},{Genre},{ISBN},{Content}";
    }

    /// <summary>
    /// Creates a deep copy of the current <see cref="Book"/> object.
    /// </summary>
    /// <returns>A new <see cref="Book"/> object that is a copy of the current object.</returns>
    public object Clone()
    {
        Book clone = new()
        {
            Id = Id,
            Author = Author,
            Title = Title,
            TotalPages = TotalPages,
            Year = Year,
            Description = Description,
            Content = Content is null ? null : new()
            {
                Name = Content.Name,
                Ext = Content.Ext,
                OriginalPath = Content.OriginalPath,
                ObjectByteArray = Content.ObjectByteArray
            },
            Genre = Genre,
            ISBN = ISBN
        };
        return clone;
    }


    public XmlSchema? GetSchema() => throw new NotImplementedException();

    /// <summary>
    /// Reads the <see cref="Book"/> from the specified XML reader.
    /// </summary>
    /// <param name="reader">The XML reader to read the <see cref="Book"/> from.</param>
    public void ReadXml(XmlReader reader)
    {
        var isRead = true;
        while (reader.Read() && isRead)
        {
            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    switch (reader.Name)
                    {
                        case "Id":
                            Id = reader.ReadElementContentAsInt();
                            break;
                        case "Author":
                            Author = reader.ReadElementContentAsString();
                            break;
                        case "Title":
                            Title = reader.ReadElementContentAsString();
                            break;
                        case "TotalPages":
                            TotalPages = reader.ReadElementContentAsInt();
                            break;
                        case "PublishDate":
                            Year = reader.ReadElementContentAsInt();
                            break;
                        case "Description":
                            Description = reader.ReadElementContentAsString();
                            break;
                        case "Genre":
                            Genre = reader.ReadElementContentAsString();
                            break;
                        case "ISBN":
                            ISBN = reader.ReadElementContentAsString();
                            break;
                        case "Content":
                            if (reader.IsEmptyElement)
                            {
                                Content = null;
                                reader.Read();
                            }
                            else
                            {
                                Content = new MediaData();
                                Content.ReadXml(reader);
                            }
                            isRead = false;
                            reader.ReadEndElement(); // Added to ensure proper reading of the 'Source' element
                            break;
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Writes the <see cref="Book"/> to the specified XML writer.
    /// </summary>
    /// <param name="writer">The XML writer to write the <see cref="Book"/> to.</param>
    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement("Book");
        writer.WriteElementString("Id", Id.ToString());
        writer.WriteElementString("Author", Author);
        writer.WriteElementString("Title", Title);
        writer.WriteElementString("TotalPages", TotalPages.ToString());
        writer.WriteElementString("PublishDate", Year.ToString());
        writer.WriteElementString("Description", Description);
        writer.WriteElementString("Genre", Genre);
        writer.WriteElementString("ISBN", ISBN);

        writer.WriteStartElement("Content");
        try { Content?.WriteXml(writer); } catch { }
        writer.WriteEndElement();

        writer.WriteEndElement();
    }

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

/// <summary>
/// Custom attribute to mark properties that should be included in the book properties list.
/// </summary>
/// <author>YR 2025-02-24</author>
[AttributeUsage(AttributeTargets.Property)]
public class BookPropertyAttribute : Attribute
{
}