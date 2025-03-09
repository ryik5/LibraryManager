using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using LibraryManager.Models;
using LibraryManager.Views;

namespace LibraryManager.ViewModels;

public class LibraryViewModel : AbstractViewModel, IDisposable
{
    public LibraryViewModel(ILibrary library)
    {
        Library = library; // Constructor injection ensures proper dependency handling
        Library.TotalBooksChanged += BookList_CollectionChanged;
        _libraryManageable = new LibraryManagerModel(Library);

        // Initialize the generic navigation command
        NavigateCommand = new AsyncRelayCommand<string>(PerformAction);

        /*MessagingCenter.Subscribe<BooksViewModel>(this, "Navigate", async (sender) =>
        {
            Console.WriteLine("Received navigation request from BooksViewModel.");
           // await PerformAction("CreateLibrary").ConfigureAwait(false);
        });*/
    }

    private void BookList_CollectionChanged(object? sender, TotalBooksEventArgs e)
    {
        RaisePropertyChanged(nameof(Library));
        RaisePropertyChanged(nameof(Library.TotalBooks));
    }


    #region Public properties

    public ILibrary Library
    {
        get => _library;
        set => SetProperty(ref _library, value);
    }

    public bool CanOperateWithBooks
    {
        get => _canOperateWithBooks;
        set => SetProperty(ref _canOperateWithBooks, value);
    }

    public Binding LibraryControlsView { get; }
    public event EventHandler<TotalBooksEventArgs> TotalBooksChanged;

    #endregion


    #region Public Methods

    protected override async Task PerformAction(string? commandParameter)
    {
        Debug.WriteLine($"NavigateCommand triggered with commandParameter: {commandParameter}");

        if (string.IsNullOrWhiteSpace(commandParameter))
            return;

        if (CurrentRoute == $"//{nameof(LibraryPage)}")
        {
            switch (commandParameter)
            {
                case nameof(AboutPage):
                case nameof(BooksPage):
                {
                    try
                    {
                        // Dynamically navigate using the provided commandParameter
                        // begins '//' added in the beginning to switch a Menu as well as Page. without '//' it switch only Page
                        await Shell.Current.GoToAsync($"//{commandParameter}").ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Navigation error: {ex.Message}");
                    }
                }
                    break;
                case Constants.CREATE_NEW_LIBRARY:
                    _libraryManageable.CreateNewLibrary();
                    break;

                default:
                {
#if DEBUG
                    Debug.WriteLine($"Commands {commandParameter} on {nameof(LibraryPage)} page.");
#endif
                    // Performing actions at the LibraryManager
                    await _libraryManageable.RunCommand(commandParameter);
                    RunInMainThread(() =>
                        {
                            RaisePropertyChanged(nameof(Library));
                            RaisePropertyChanged(nameof(Library.BookList));
                        }
                    );
                }
                    break;
            }
        }
        else
        {
#if DEBUG
            Debug.WriteLine(
                $"Navigation error path '{commandParameter}' in class '{nameof(BooksViewModel)}' by method '{nameof(PerformAction)}'");
#endif
        }
    }


    // Dispose method for external calls
    public void Dispose()
    {
        if (_disposed) return; // Safeguard against multiple Dispose calls.
        _disposed = true;

        // MessagingCenter cleanup
#if DEBUG
        Console.WriteLine("Cleaning up MessagingCenter resources in LibraryViewModel.");
#endif

        MessagingCenter.Unsubscribe<BooksViewModel>(this, "Navigate");
    }

    ~LibraryViewModel()
    {
        Dispose(); // Safeguard cleanup in destructor (if proper disposal is skipped)
    }

    #endregion


    #region Private methods

    private async Task LibraryCreateNewAsync()
    {
        await ShowDisplayAlertAsync("Message", "Pressed - CreateNewLibrary");
    }

    #endregion


    #region Private fields

    private ILibrary _library;
    private bool _canOperateWithBooks = true;
    private ILibraryManageable _libraryManageable;
    private bool _disposed; // Safeguard for multiple calls to Dispose.

    #endregion
}