using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LibraryManager.Models;

namespace LibraryManager.ViewModels;

public class BooksViewModel: INotifyPropertyChanged
{
    public ICommand NavigateCommand { get; }
    public ObservableCollection<Book> Books { get; set; }
    public ObservableCollection<Book> SelectedBooks { get; set; }

    public BooksViewModel()
    {
        var libraryViewModel = (Application.Current as App)?.Library;

        // Initialize the generic navigation command
        NavigateCommand = new AsyncRelayCommand<string>(async (route) => await NavigateToPage(route));

        // Populate with sample data
        Books = new ObservableCollection<Book>
        {
            new Book { Id = 0, Title = "1984", Author = "George Orwell", Year = 1949, TotalPages = 1},
            new Book { Id = 1,Title = "Pride and Prejudice", Author = "Jane Austen", Year = 1813 , TotalPages = 1},
            new Book { Id = 2,Title = "The Catcher in the Rye", Author = "J.D. Salinger", Year = 1951, TotalPages = 1 }
        };

        SelectedBooks = new ObservableCollection<Book>();

        // Monitor selected items
        SelectedBooks.CollectionChanged += OnSelectionChanged;
    }

    private void OnSelectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (var newItem in e.NewItems)
            {
                var selectedBook = newItem as Book;
                Console.WriteLine($"Selected: {selectedBook.Title}");
            }
        }

        if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (var removedItem in e.OldItems)
            {
                var deselectedBook = removedItem as Book;
                Console.WriteLine($"Deselected: {deselectedBook.Title}");
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    private async Task NavigateToPage(string route)
    {
        if (string.IsNullOrWhiteSpace(route))
            return;

        try
        {
            // Dynamically navigate using the provided route
            await Shell.Current.GoToAsync(route);
        }
        catch (Exception ex)
        {
            // Handle any issues with navigation
            Console.WriteLine($"Navigation error: {ex.Message}");
        }
    }
}
