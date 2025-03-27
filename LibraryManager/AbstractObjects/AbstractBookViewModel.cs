using LibraryManager.Models;
using System.Collections.ObjectModel;

namespace LibraryManager.AbstractObjects;

public abstract class AbstractBookViewModel : AbstractViewModel
{
    #region Public properties
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
    #endregion

    #region CommandParameters
    public string OK
    {
        get => _ok;
        set => SetProperty(ref _ok, value);
    }

    public string Cancel => Constants.CANCEL;
    #endregion

    #region MyRegion
    private ILibrary _library;
    private ObservableCollection<object> _selectedObjects = new();
    private Book _book;
    private string _ok = Constants.OK;
    #endregion
}