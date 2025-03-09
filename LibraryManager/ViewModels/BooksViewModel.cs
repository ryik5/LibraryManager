using System.ComponentModel;
using System.Diagnostics;
using LibraryManager.Models;
using LibraryManager.Views;

namespace LibraryManager.ViewModels;

public class BooksViewModel : AbstractViewModel, IDisposable
{
    public BooksViewModel(ILibrary library)
    {
        Library = library;
        Library.BookList.CollectionChanged += BookList_CollectionChanged;

        _bookManageable = new BookManagerModel(Library);
     }

    
    #region Public properties
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

    
    #region Public Methods
    protected override async Task PerformAction(string? commandParameter)
    {
        Debug.WriteLine($"NavigateCommand triggered with commandParameter: {commandParameter}");

        if (string.IsNullOrWhiteSpace(commandParameter))
            return;

        // Prevent navigation to the same page - 'Shell.Current.GoToAsync(...'
        if (CurrentRoute == $"//{nameof(BooksPage)}")
        {
            switch (commandParameter)
            {
                case nameof(AboutPage):
                case nameof(LibraryPage):
                {
                    try
                    {
                        // Dynamically navigate using the provided commandParameter
                        // begins '//' added in the beginning to switch a Menu as well as Page. without '//' it switch only Page
                        await Shell.Current.GoToAsync($"//{commandParameter}").ConfigureAwait(false);
                    }
                    catch (Exception ex) // Handle any issues with navigation
                    {
                        Debug.WriteLine($"Navigation error: {ex.Message}");
                    }
                }
                    break;
                case Constants.EDIT_BOOK:
                    break;
                default: //jobs perform without creating views
                {
#if DEBUG
                    Debug.WriteLine($"Commands {commandParameter} on {nameof(BooksPage)} page.");
#endif
                    
                    // Performing actions at the BooksManager
                    await _bookManageable.RunCommand(commandParameter, SelectedBooks);

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
                $"Navigation error path '{commandParameter}' in class '{nameof(BooksViewModel)}' by method '{nameof(PerformAction)}'");
#endif
        }
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
   #endregion


   #region Private methods

     private void OnLibraryChanged(object sender, PropertyChangedEventArgs e)
     {
         Debug.WriteLine($"Library property changed: {e.PropertyName}");
     }
 
     /// <summary>
     /// Handles the CollectionChanged event of the BookList.
     /// </summary>
     /// <param name="sender">The source of the event.</param>
     /// <param name="e">The NotifyCollectionChangedEventArgs instance containing the event data.</param>
     private void BookList_CollectionChanged(object? sender,
         System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
     {
         TotalBooksChanged?.Invoke(this, new TotalBooksEventArgs { TotalBooks = Library.BookList.Count });
         Library.TotalBooks= Library.BookList.Count;
     }
     
    

    private Task AddBook()
    {
        RunInPageThread(() =>
            Library.BookList.Add(
                new Book { Id = 0, Title = "1984", Author = "George Orwell", Year = 1949, TotalPages = 1 }
            ));

        return Task.CompletedTask;
    }

    #endregion

    
    #region Private fields

    private readonly IBookManageable _bookManageable;
    private ILibrary _library;
    private IList<Book> _selectedBooks = new List<Book>();
    private bool _disposed; // Safeguard for multiple calls to Dispose.

    #endregion
}