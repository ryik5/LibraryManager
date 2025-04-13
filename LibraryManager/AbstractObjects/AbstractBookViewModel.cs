using LibraryManager.Models;
using System.Collections.ObjectModel;

namespace LibraryManager.AbstractObjects;

public abstract class AbstractBookViewModel : AbstractViewModel
{
    public AbstractBookViewModel(ILibrary library, IStatusBar statusBar)
    {
        StatusBar = statusBar;
        Library = library;
    }


    #region Public properties
    public IStatusBar StatusBar
    {
        get => _statusBar;
        set => SetProperty(ref _statusBar, value);
    }

    public ILibrary Library
    {
        get => _library;
        set => SetProperty(ref _library, value);
    }

    public ObservableCollection<object> SelectedBooks
    {
        get => _selectedObjects;
        set => SetProperty(ref _selectedObjects, value);
    }

    public Book Book
    {
        get => _book;
        set => SetProperty(ref _book, value);
    }

    public bool IsBooksCollectionViewVisible
    {
        get => _isBooksCollectionViewVisible;
        set => SetProperty(ref _isBooksCollectionViewVisible, value);
    }

    public bool IsEditBookViewVisible
    {
        get => _isEditBookViewVisible;
        set => SetProperty(ref _isEditBookViewVisible, value);
    }

    public bool CanEditBook
    {
        get => _canEditBook;
        set => SetProperty(ref _canEditBook, value);
    }

    public bool CanOperateWithBooks
    {
        get => _canOperateWithBooks;
        set => SetProperty(ref _canOperateWithBooks, value);
    }
    #endregion


    #region CommandParameters
    public string OK
    {
        get => _ok;
        set => SetProperty(ref _ok, value);
    }

    public string Cancel => Constants.CANCEL;
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