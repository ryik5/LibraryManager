using LibraryManager.Models;
using LibraryManager.Views;

namespace LibraryManager;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for non-hierarchical pages
        Routing.RegisterRoute("CreateLibrary", typeof(LibraryPage));
        Routing.RegisterRoute("LoadLibrary", typeof(LibraryPage));
        Routing.RegisterRoute("SaveLibrary", typeof(LibraryPage));
        Routing.RegisterRoute("SaveAsLibrary", typeof(LibraryPage));
        Routing.RegisterRoute("CloseLibrary", typeof(LibraryPage));
        // to switch from BooksPage
        Routing.RegisterRoute(nameof(LibraryPage), typeof(LibraryPage));
       
        Routing.RegisterRoute("AddNewBook", typeof(BooksPage));
        Routing.RegisterRoute("EditBook", typeof(BooksPage));
        Routing.RegisterRoute("DeleteBooks", typeof(BooksPage));
        Routing.RegisterRoute("ImportBook", typeof(BooksPage));
        Routing.RegisterRoute("ExportBook", typeof(BooksPage));
        Routing.RegisterRoute("SortBooks", typeof(BooksPage));
        // to switch from LibraryPage
        Routing.RegisterRoute(nameof(BooksPage), typeof(BooksPage));

        // App Logo. Assign the custom drawable to the GraphicsView
        LogoGraphicsView.Drawable = new CustomDrawable();
    }
}