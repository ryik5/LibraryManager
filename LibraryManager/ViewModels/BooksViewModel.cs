using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LibraryManager.Models;

namespace LibraryManager.ViewModels;

public class BooksViewModel : BindableObject, INotifyPropertyChanged
{
    public BooksViewModel(ILibrary library)
    {
         
        Library = library;
      //  _library = new LibraryModel(library); // Constructor injection ensures proper dependency handling
       // RaisePropertyChanged(nameof(Books));
       
       //SelectedBooks = new ObservableCollection<Book>();
        
       // Monitor selected items
       // SelectedBooks.CollectionChanged += OnSelectionChanged;

        // Initialize the generic navigation command
        NavigateCommand = new AsyncRelayCommand<string>(async (route) => await NavigateToPage(route));
     }

    #region Public properties

    public ICommand NavigateCommand { get; }

    /*public ObservableCollection<Book> Books
    {
        get => _library.BookList;
        set => SetProperty(ref _library.BookList, value);
    }*/
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

    private Task NavigateToPage(string route)
    {
        Console.WriteLine($"NavigateCommand triggered with route: {route}");

        if (string.IsNullOrWhiteSpace(route))
            return Task.CompletedTask;
        ;

        try
        {
            // Prevent navigation to the same page
            var currentRoute = Shell.Current.CurrentState.Location.OriginalString;
            if (currentRoute == $"//BooksManagePage")
            {
#if DEBUG
                Console.WriteLine($"You're already on the {route} page. Navigation skipped.");
                AddBook();
#endif

                return Task.CompletedTask;
            }

            // Dynamically navigate using the provided route
            Shell.Current.GoToAsync(route);
        }
        catch (Exception ex)
        {
            // Handle any issues with navigation
#if DEBUG
            Console.WriteLine($"Navigation error: {ex.Message}");
#endif
        }

        return Task.CompletedTask;
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

    #endregion
}