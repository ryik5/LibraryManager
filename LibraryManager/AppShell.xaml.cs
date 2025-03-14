using LibraryManager.Models;
using LibraryManager.Views;

namespace LibraryManager;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for non-hierarchical pages.
        Routing.RegisterRoute(nameof(LibraryPage), typeof(LibraryPage));
        Routing.RegisterRoute(nameof(BooksPage), typeof(BooksPage));
        Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));
        Routing.RegisterRoute(nameof(FindBooksPage), typeof(FindBooksPage));
        Routing.RegisterRoute(nameof(ToolsPage), typeof(ToolsPage));

        // App Logo. Assign the custom drawable to the GraphicsView
        LogoGraphicsView.Drawable = new CustomDrawable();
    }
}