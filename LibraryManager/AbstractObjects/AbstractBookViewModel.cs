using LibraryManager.Models;
using System.Collections.ObjectModel;

namespace LibraryManager.AbstractObjects;

/// <summary>
/// Abstract base class for book view models.
/// </summary>
/// <author>YR 2025-03-09</author>
public abstract class AbstractBookViewModel : AbstractViewModel
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
    }

    #region Public properties
    /// <summary>
    /// Gets or sets the status bar instance.
    /// </summary>
    public IStatusBar StatusBar
    {
        get => _statusBar;
        set => SetProperty(ref _statusBar, value);
    }

    /// <summary>
    /// Gets or sets the library instance.
    /// </summary>
    public ILibrary Library
    {
        get => _library;
        set => SetProperty(ref _library, value);
    }

    /// <summary>
    /// Gets or sets the collection of selected books.
    /// </summary>
    public ObservableCollection<object> SelectedBooks
    {
        get => _selectedObjects;
        set => SetProperty(ref _selectedObjects, value);
    }

    /// <summary>
    /// Gets or sets the book instance.
    /// </summary>
    public Book Book
    {
        get => _book;
        set => SetProperty(ref _book, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the books collection view is visible.
    /// </summary>
    public bool IsBooksCollectionViewVisible
    {
        get => _isBooksCollectionViewVisible;
        set => SetProperty(ref _isBooksCollectionViewVisible, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the edit book view is visible.
    /// </summary>
    public bool IsEditBookViewVisible
    {
        get => _isEditBookViewVisible;
        set => SetProperty(ref _isEditBookViewVisible, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the book can be edited.
    /// </summary>
    public bool CanEditBook
    {
        get => _canEditBook;
        set => SetProperty(ref _canEditBook, value);
    }

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
    #endregion


    #region CommandParameters
    /// <summary>
    /// Gets or sets the OK command parameter.
    /// </summary>
    public string OK
    {
        get => _ok;
        set => SetProperty(ref _ok, value);
    }

    /// <summary>
    /// Gets the cancel command parameter.
    /// </summary>
    public string Cancel => Constants.CANCEL;
    #endregion

    #region Public Methods
    /// <summary>
    /// Checks if the library is valid based on its ID.
    /// </summary>
    /// <remarks>
    /// A library is considered valid if its ID is not zero.
    /// </remarks>
    protected bool IsValidLibrary => NotZero(Library?.Id);

    /// <summary>
    /// Checks if the library is not empty based on its total books count.
    /// </summary>
    /// <remarks>
    /// A library is considered not empty if its total books count is not zero.
    /// </remarks>
    protected bool IsNotEmptyLibrary => NotZero(Library?.TotalBooks);
    #endregion


    #region Private fields
    private ILibrary _library;
    private ObservableCollection<object> _selectedObjects = new();
    private Book _book;
    private string _ok = Constants.OK;
    private bool _canOperateWithBooks;
    private bool _canEditBook;
    private bool _isEditBookViewVisible;
    private bool _isBooksCollectionViewVisible;
    private IStatusBar _statusBar;
    #endregion
}