using LibraryManager.Models;

namespace LibraryManager.Controls;

public partial class BookGridView : ContentView
{
    public BookGridView()
    {
        InitializeComponent();
        
        // Fetch the singleton instance of Book
        BindingContext ??= App.Services.GetService<Book>();
    }
}