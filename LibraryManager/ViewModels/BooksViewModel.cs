using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LibraryManager.Models;

namespace LibraryManager.ViewModels;

public class BooksViewModel : INotifyPropertyChanged
{
    private readonly LibraryModel _library;
    
    
    public ICommand NavigateCommand { get; }
    public ObservableCollection<Book> Books     {
        get => _library.BookList;
        set => SetProperty(ref _library.BookList, value);
    }
    
    public ObservableCollection<Book> SelectedBooks { get; set; }

    public BooksViewModel(ILibrary library)
    {
        _library = new LibraryModel(library); // Constructor injection ensures proper dependency handling
        RaisePropertyChanged(nameof(Books));

        // Initialize the generic navigation command
        NavigateCommand = new AsyncRelayCommand<string>(async (route) => await NavigateToPage(route));
        
        SelectedBooks = new ObservableCollection<Book>();

        // Monitor selected items
        SelectedBooks.CollectionChanged += OnSelectionChanged;
    }

    /*public BooksViewModel()
    {
        _library = new LibraryModel(App.Services.GetService<Library>()); //(Application.Current as App)?.Library
        RaisePropertyChanged(nameof(Books));


        // Initialize the generic navigation command
        NavigateCommand = new AsyncRelayCommand<string>(async (route) => await NavigateToPage(route));

        SelectedBooks = new ObservableCollection<Book>();

        // Monitor selected items
        SelectedBooks.CollectionChanged += OnSelectionChanged;
    }*/

    private  Task NavigateToPage(string route)
    {
        Console.WriteLine($"NavigateCommand triggered with route: {route}");

        if (string.IsNullOrWhiteSpace(route))
            return Task.CompletedTask;;

        try
        {
            // Prevent navigation to the same page
            var currentRoute = Shell.Current.CurrentState.Location.OriginalString;
            if (currentRoute == $"//BooksManagePage")
            {
                #if DEBUG
                Console.WriteLine($"You're already on the {route} page. Navigation skipped.");
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
}