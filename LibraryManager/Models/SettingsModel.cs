namespace LibraryManager.Models;

/// <summary>
/// Represents a model for storing application settings.
/// </summary>
/// <author>YR 2025-02-14</author>
internal sealed class SettingsModel
{
    public SettingsModel()
    {
        SearchFields = Enum.GetValues(typeof(EBibliographicKindInformation)).Cast<EBibliographicKindInformation>()
            .ToArray();
        Booleans = new[] { true, false };
        BookProperties = new Library().GetBookProperties();
        SortingDirections = new[] { Constants.SORTING_ASCENDING, Constants.SORTING_DESCENDING };
    }

    #region Dictionaries
    /// <summary>
    /// Gets or sets an array of search fields available for the FindBooks page.
    /// </summary>
    public EBibliographicKindInformation[] SearchFields;

    /// <summary>
    /// Gets or sets an array of boolean values representing the state of various settings.
    /// </summary>
    public bool[] Booleans;

    public readonly string[] SortingDirections;

    /// <summary>
    /// Gets or sets an array of PropertyInfo's Names representing the properties of a book.
    /// </summary>
    public string[] BookProperties;
    #endregion

    #region Book details
    public long Book_MaxContentLength;
    #endregion


    #region MessageBox
    public double MessageBox_FontSize;
    #endregion


    #region BooksViewModel Page
    /// <summary>
    /// Gets or sets the primary property used for sorting books.
    /// </summary>
    public string FirstSortBookProperty;

    public bool FirstSortProperty_ByDescend;

    /// <summary>
    /// Gets or sets the secondary property used for sorting books.
    /// </summary>
    public string SecondSortBookProperty;

    public bool SecondSortProperty_ByDescend;

    /// <summary>
    /// Gets or sets the tertiary property used for sorting books.
    /// </summary>
    public string ThirdSortBookProperty;

    public bool ThirdSortProperty_ByDescend;

    public string SortBookByPropertiesTooltip;
    #endregion


    #region BookDetails Page
    /// <summary>
    /// Gets or sets a value indicating whether to show book details.
    /// </summary>
    public bool ShowBookDetails = false;
    #endregion


    #region FindBooks Page
    /// <summary>
    /// Gets or sets the currently selected search field for the FindBooks page.
    public EBibliographicKindInformation SearchField = EBibliographicKindInformation.All;

    /// <summary>
    /// Gets or sets a value indicating whether to search on the fly for the FindBooks page.
    /// </summary>
    public bool SearchOnFly = false;
    #endregion


    #region Debug Page
    /// <summary>
    /// Gets or sets the font size for the debug text.
    /// </summary>
    public double Debug_TextFontSize = 12;
    #endregion
}