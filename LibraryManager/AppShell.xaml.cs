using LibraryManager.Models;
using LibraryManager.Views;

namespace LibraryManager;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for non-hierarchical pages.
        // to switch from BooksPage
        Routing.RegisterRoute(nameof(LibraryPage), typeof(LibraryPage));
        // to switch from LibraryPage
        Routing.RegisterRoute(nameof(BooksPage), typeof(BooksPage));
        
        Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));

        // App Logo. Assign the custom drawable to the GraphicsView
        LogoGraphicsView.Drawable = new CustomDrawable();
    }
}