using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LibraryManager.Models;
using LibraryManager.Views;

namespace LibraryManager.ViewModels;

public class BooksViewModel : BindableObject, INotifyPropertyChanged, IDisposable
{
    public BooksViewModel(ILibrary library)
    {
        Library = library;

        // Initialize the generic navigation command
        NavigateCommand = new AsyncRelayCommand<string>(NavigateToPage);

     }
    public BooksViewModel()
    {
        Library  = App.Services.GetService<ILibrary>();

        // Initialize the generic navigation command
        NavigateCommand = new AsyncRelayCommand<string>(NavigateToPage);
    }
    
    // Cleanup method for external calls
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

    public IList<Book> SelectedBooks {
        get => _selectedBooks;
        set => SetProperty(ref _selectedBooks, value);
    }
    private IList<Book> _selectedBooks=new List<Book>();

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion


    #region Private methods

    private async Task NavigateToPage(string? route)
    {
        Console.WriteLine($"NavigateCommand triggered with route: {route}");

        if (string.IsNullOrWhiteSpace(route))
            return;

        try
        {
            // Prevent navigation to the same page
            var currentRoute = Shell.Current.CurrentState.Location.OriginalString;
            if (currentRoute == $"//{nameof(BooksManagePage)}")
            {
#if DEBUG
                Console.WriteLine($"You're already on the {route} page. Navigation skipped.");
                AddBook();
#endif
            }

            // Dynamically navigate using the provided route
         // await Shell.Current.GoToAsync(route).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // Handle any issues with navigation
#if DEBUG
            Console.WriteLine($"Navigation error: {ex.Message}");
#endif
        }
    }

    private void AddBook()
    {
        Library.BookList.Add(
            new Book { Id = 0, Title = "1984", Author = "George Orwell", Year = 1949, TotalPages = 1 }
            );
    }

    #region Binding implementation

    
    
    
    private void OnSelectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (var newItem in e.NewItems)
            {
                var selectedBook = newItem as Book;
#if DEBUG
                Console.WriteLine($"Selected: {selectedBook?.Title}");
#endif
            }
        }

        if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (var removedItem in e.OldItems)
            {
                var deselectedBook = removedItem as Book;
#if DEBUG
                Console.WriteLine($"Deselected: {deselectedBook?.Title}");
#endif
            }
        }
    }

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

    #endregion

    #region private fields

   // private readonly LibraryModel _library;
    private ILibrary _library;
    private bool _disposed; // Safeguard for multiple calls to Dispose.
    #endregion
}