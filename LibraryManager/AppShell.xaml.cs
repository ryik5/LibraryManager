using LibraryManager.Models;
using LibraryManager.Views;

namespace LibraryManager;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for non-hierarchical pages
        Routing.RegisterRoute("CreateLibrary", typeof(LibraryManagePage));
        Routing.RegisterRoute("LoadLibrary", typeof(LibraryManagePage));
        Routing.RegisterRoute("SaveLibrary", typeof(LibraryManagePage));
        Routing.RegisterRoute("SaveAsLibrary", typeof(LibraryManagePage));
        Routing.RegisterRoute("CloseLibrary", typeof(LibraryManagePage));
        
        Routing.RegisterRoute("AddNewBook", typeof(BooksManagePage));
        Routing.RegisterRoute("EditBook", typeof(BooksManagePage));
        Routing.RegisterRoute("DeleteBooks", typeof(BooksManagePage));
        Routing.RegisterRoute("ImportBook", typeof(BooksManagePage));
        Routing.RegisterRoute("ExportBook", typeof(BooksManagePage));
        Routing.RegisterRoute("SortBooks", typeof(BooksManagePage));

        // Assign the custom drawable to the GraphicsView
        LogoGraphicsView.Drawable = new CustomDrawable();
    }
}