using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LibraryManager.Models;
using LibraryManager.Views;

namespace LibraryManager.ViewModels;

public class BooksViewModel : INotifyPropertyChanged, IDisposable
{
    public BooksViewModel(ILibrary library)
    {
        Library = library;
        Library.BookList.CollectionChanged += BookList_CollectionChanged;

        _bookManageable = new BookManagerModel(Library);

        // Initialize the generic navigation command
        NavigateCommand = new AsyncRelayCommand<string>(NavigateToPage);
    }

    // Dispose method for external calls
    public void Dispose()
    {
        if (_disposed) return; // Safeguard against multiple Dispose calls.
        _disposed = true;
        Library.BookList.CollectionChanged -= BookList_CollectionChanged;

        // Perform cleanup: Unsubscribe from any events
        SelectedBooks = null;
        if (_library is INotifyPropertyChanged notifyLibrary)
        {
            notifyLibrary.PropertyChanged -= OnLibraryChanged;
        }

        Debug.WriteLine("BooksViewModel disposed successfully.");
    }

    ~BooksViewModel()
    {
        Dispose(); // Safeguard cleanup in destructor (if proper disposal is skipped)
    }

    private void OnLibraryChanged(object sender, PropertyChangedEventArgs e)
    {
        Debug.WriteLine($"Library property changed: {e.PropertyName}");
    }
    /// <summary>
    /// Handles the CollectionChanged event of the BookList.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The NotifyCollectionChangedEventArgs instance containing the event data.</param>
    private void BookList_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        TotalBooksChanged?.Invoke(this, new TotalBooksEventArgs { TotalBooks = Library?.BookList?.Count ?? 0 });
    }

    #region Public properties

    public ICommand NavigateCommand { get; } 

    public ILibrary Library
    {
        get => _library;
        set => SetProperty(ref _library, value);
    }

    public IList<Book> SelectedBooks
    {
        get => _selectedBooks;
        set => SetProperty(ref _selectedBooks, value);
    }
    
    public event EventHandler<TotalBooksEventArgs> TotalBooksChanged;

    #endregion


    #region Private methods

    private async Task NavigateToPage(string? commandName)
    {
        Debug.WriteLine($"NavigateCommand triggered with commandName: {commandName}");

        if (string.IsNullOrWhiteSpace(commandName))
            return;

        // Prevent navigation to the same page - 'Shell.Current.GoToAsync(...'
        var currentRoute = Shell.Current.CurrentState.Location.OriginalString;
        if (currentRoute == $"//{nameof(BooksPage)}")
        {
            switch (commandName)
            {
                case nameof(LibraryPage):
                {
                    try
                    {
                        // Dynamically navigate using the provided commandName
                        // begins '//' added in the beginning to switch a Menu as well as Page. without '//' it switch only Page
                        await Shell.Current.GoToAsync($"//{commandName}").ConfigureAwait(false);
                    }
                    catch (Exception ex) // Handle any issues with navigation
                    {
                        Debug.WriteLine($"Navigation error: {ex.Message}");
                    }
                }
                    break;
                default:
                {
#if DEBUG
                    Debug.WriteLine($"Commands {commandName} on {nameof(BooksPage)} page.");
#endif
                    // TODO : performing actions at the BooksManager

                    await _bookManageable.RunCommand(commandName);

                    RunInMainThread(() =>
                        {
                            RaisePropertyChanged(nameof(Library));
                            RaisePropertyChanged(nameof(Library.BookList));
                        }
                    );


                    //  await AddBook(); //Test only
                }
                    break;
            }
        }
        else
        {
#if DEBUG
            Debug.WriteLine(
                $"Navigation error path '{commandName}' in class '{nameof(BooksViewModel)}' by method '{nameof(NavigateToPage)}'");
#endif
        }
    }

    /// <summary>
    /// Adds a new book to the <see cref="Library.BookList"/> in a UI-safe manner.
    /// </summary>
    /// <remarks>
    /// This method ensures the update to the book list is performed on the UI thread. 
    /// The added book has default values for its properties.
    /// </remarks>
    /// <returns>A completed task, indicating the operation is done.</returns>
    private Task AddBook()
    {
        RunInPageThread(() =>
            Library.BookList.Add(
                new Book { Id = 0, Title = "1984", Author = "George Orwell", Year = 1949, TotalPages = 1 }
            ));

        return Task.CompletedTask;
    }


    // Run code in the UI thread
    // Platform-agnostic, works anywhere in MAUI
    // Slightly slower due to abstraction (but negligible)
    private void RunInMainThread(Action a)
    {
        // Run code in the UI thread
        MainThread.BeginInvokeOnMainThread(a.Invoke);
    }

    // Run code in the UI thread
    // Best For -  Page- or Application-scoped logic 
    // Slightly faster in direct access situations 
    private void RunInPageThread(Action a)
    {
        Application.Current?.Dispatcher.Dispatch(a.Invoke);
    }

    #endregion


    #region Binding implementation

    public event PropertyChangedEventHandler PropertyChanged;

    private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        RaisePropertyChanged(propertyName);
        return true;
    }

    #endregion


    #region Private fields

    private readonly IBookManageable _bookManageable;
    private ILibrary _library;
    private IList<Book> _selectedBooks = new List<Book>();
    private bool _disposed; // Safeguard for multiple calls to Dispose.

    #endregion
}