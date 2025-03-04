using LibraryManager.Views;

namespace LibraryManager;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for non-hierarchical pages
        Routing.RegisterRoute("LibraryNew", typeof(LibraryNewPage));
        Routing.RegisterRoute("BooksPage", typeof(BooksPage));
    }
}