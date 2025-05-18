using CommunityToolkit.Mvvm.ComponentModel;
using LibraryManager.Models;
using System.Collections.ObjectModel;

namespace LibraryManager.AbstractObjects;

/// <summary>
/// Abstract base class for book view models.
/// </summary>
/// <author>YR 2025-03-09</author>
public abstract partial class AbstractBookViewModel : AbstractViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractBookViewModel"/> class.
    /// </summary>
    /// <param name="library">The library instance.</param>
    /// <param name="statusBar">The status bar instance.</param>
    public AbstractBookViewModel(ILibrary library, IStatusBar statusBar)
    {
        StatusBar = statusBar;
        Library = library;

        ContentState = Constants.LOAD_CONTENT;
        ClearingState = Constants.CLEAR_CONTENT;
        LoadCover = Constants.LOAD_COVER;
    }


    #region Public properties
    /// <summary>
    /// Gets or sets the library instance.
    /// </summary>
    [ObservableProperty]
    ILibrary _library;

    /// <summary>
    /// Gets or sets the collection of selected books.
    /// </summary>
    [ObservableProperty] ObservableCollection<object> _selectedBooks;

    /// <summary>
    /// Gets or sets the book instance.
    /// </summary>
    [ObservableProperty] private Book _book;

    /// <summary>
    /// Gets or sets a value indicating whether the books collection view is visible.
    /// </summary>
    [ObservableProperty] private bool _isBooksCollectionViewVisible;

    /// <summary>
    /// Gets or sets a value indicating whether the edit book view is visible.
    /// </summary>
    [ObservableProperty] private bool _isEditBookViewVisible;

    /// <summary>
    /// Gets or sets a value indicating whether the book can be edited.
    /// </summary>
    [ObservableProperty] private bool _canEditBook;

    /// <summary>
    /// Gets or sets a value indicating whether operations can be performed on books.
    /// </summary>
    public bool CanOperateWithBooks
    {
        get => _canOperateWithBooks;
        set
        {
            if (SetProperty(ref _canOperateWithBooks, value))
            {
                if (!value)
                    CanEditBook = false;
            }
        }
    }

    [ObservableProperty] string _contentState;

    /// <summary>
    /// Gets or sets the clearing state.
    /// </summary>
    [ObservableProperty] string _clearingState;

    [ObservableProperty] string _loadCover;
    #endregion

    #region CommandParameters
    /// <summary>
    /// Gets or sets the OK command parameter.
    /// </summary>
    [ObservableProperty] string _oK = Constants.OK;

    /// <summary>
    /// Gets the cancel command parameter.
    /// </summary>
    [ObservableProperty] string _cancel = Constants.CANCEL;
    #endregion

    #region Public Methods
    public async Task<string> GetSavingNameForLibrary()
    {
        // display window with input a new library name
        var castomDialog = await ShowCustomDialogPage(Constants.LIBRARY_SAVE_WITH_NAME,
            Constants.LIBRARY_NAME, true, Library.Id.ToString());

        return castomDialog.IsOk && !string.IsNullOrEmpty(castomDialog.InputString)
            ? castomDialog.InputString
            : Library.Id.ToString();
    }

    /// <summary>
    /// Checks if the library is valid based on its ID.
    /// </summary>
    /// <remarks>
    /// A library is considered valid if its ID is not zero.
    /// </remarks>
    protected bool IsValidLibrary => IsNotZero(Library?.Id);

    /// <summary>
    /// Checks if the library is not empty based on its total books count.
    /// </summary>
    /// <remarks>
    /// A library is considered not empty if its total books count is not zero.
    /// </remarks>
    protected bool IsNotEmptyLibrary => IsNotZero(Library?.TotalBooks);
    #endregion

    #region Private fields
    private bool _canOperateWithBooks;
    #endregion
}