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

        // Initialize the generic navigation command
        NavigateCommand = new AsyncRelayCommand<string>(NavigateToPage);
    }

    // Dispose method for external calls
    public void Dispose()
    {
        if (_disposed) return; // Safeguard against multiple Dispose calls.
        _disposed = true;

        // Perform cleanup: Unsubscribe from any events
        SelectedBooks = null;
        if (_library is INotifyPropertyChanged notifyLibrary)
        {
            notifyLibrary.PropertyChanged -= OnLibraryChanged;
        }

        Console.WriteLine("BooksViewModel disposed successfully.");
    }

    ~BooksViewModel()
    {
        Dispose(); // Safeguard cleanup in destructor (if proper disposal is skipped)
    }

    private void OnLibraryChanged(object sender, PropertyChangedEventArgs e)
    {
        Console.WriteLine($"Library property changed: {e.PropertyName}");
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

    #endregion


    #region Private methods

    private async Task NavigateToPage(string? route)
    {
        Debug.WriteLine($"NavigateCommand triggered with route: {route}");

        if (string.IsNullOrWhiteSpace(route))
            return;

        try
        {
            // Prevent navigation to the same page - 'Shell.Current.GoToAsync(...'
            var currentRoute = Shell.Current.CurrentState.Location.OriginalString;
            if (currentRoute == $"//{nameof(BooksPage)}" && route != $"{nameof(LibraryPage)}")
            {
#if DEBUG
                Debug.WriteLine($"You're already on the {route} page. Navigation skipped.");
#endif
                // TODO : Do management of the books
                // Manage Books
                await AddBook();
            } // Dynamically navigate using the provided route
            else if (route == $"{nameof(LibraryPage)}")
            {
                // begins '//' added in the beginning to switch a Menu as well as Page
                // without '//' it switch only Page
                await Shell.Current.GoToAsync($"//{route}").ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            // Handle any issues with navigation
#if DEBUG
            Debug.WriteLine($"Navigation error: {ex.Message}");
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

    private ILibrary _library;
    private IList<Book> _selectedBooks = new List<Book>();
    private bool _disposed; // Safeguard for multiple calls to Dispose.

    #endregion
}