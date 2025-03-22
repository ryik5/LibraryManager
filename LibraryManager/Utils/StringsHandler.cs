namespace LibraryManager.Utils;

/// <author>YR 2025-02-16</author>
internal static class StringsHandler
{

    #region LibraryViewModel
    public static string LibraryChangedMessage() => $"Library was changed.{Environment.NewLine}Do you want to save changes?";

    /// <summary>
    /// Creates an XML file name by appending '.xml' to the provided name.
    /// </summary>
    /// <param name="name">The base name of the XML file.</param>
    /// <returns>The XML file name.</returns>
    public static string CreateXmlFileName(string name) => $"{name}.xml";
    public static string CreateXmlFileName(string name, string extension) => $"{name}{extension}";

    #endregion
    
}
