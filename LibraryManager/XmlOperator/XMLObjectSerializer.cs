using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace LibraryManager.Models;

/// <summary>
/// A utility class for serializing and deserializing objects to and from XML files.
/// </summary>
/// <author>YR 2025-01-26</author>
public static class XmlObjectSerializer
{
    /// <summary>
    /// Saves an object of type <typeparamref name="T"/> to an XML file.
    /// </summary>
    /// <typeparam name="T">The type of object to serialize.</typeparam>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="fileName">The path to the XML file to write to.</param>
    public static void Save<T>(T obj, string flieName)
    {
        var serializer = new XmlSerializer(typeof(T));
        //Create a FileStream object connected to the target file
        var fileStream = new FileStream(flieName, FileMode.Create);
        serializer.Serialize(fileStream, obj);
        fileStream.Flush();
        fileStream.Close();
    }

    /// <summary>
    /// Loads an object of type <typeparamref name="T"/> from an XML file.
    /// </summary>
    /// <typeparam name="T">The type of object to deserialize.</typeparam>
    /// <param name="fileName">The path to the XML file to read from.</param>
    /// <returns>The deserialized object, or null if the file is empty or cannot be deserialized.</returns>
    public static T? Load<T>(string fileName) where T : class
    {
        var deserializer = new XmlSerializer(typeof(T));
        var settings = new XmlReaderSettings
        {
            IgnoreWhitespace = false,
            IgnoreComments = false
        };
        using var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var xmlReader = XmlReader.Create(fileStream, settings);

        return (T?)deserializer.Deserialize(xmlReader);
    }
}
