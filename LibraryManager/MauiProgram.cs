using System.Collections.ObjectModel;
using LibraryManager.Models;
using LibraryManager.ViewModels;
using LibraryManager.Views;
using Microsoft.Extensions.Logging;

namespace LibraryManager;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Manrope.ttf", "ManropeExtraLight");
            });


        var library = new Library
        {
            // Populate with sample data       
            Id = new Random().Next(),
            Name = "",
            Description = "",
            BookList = new ObservableCollection<Book>
            {
                // TODO : DEMO data. Will clean
                new Book { Id = 0, Title = "1984", Author = "George Orwell", Year = 1949, TotalPages = 1 },
                new Book { Id = 1, Title = "Pride and Prejudice", Author = "Jane Austen", Year = 1813, TotalPages = 1 },
                new Book { Id = 2, Title = "The Catcher in the Rye", Author = "J.D. Salinger", Year = 1951, TotalPages = 1 }
            }
        };

        // Register all shared ViewModels and objects
        builder.Services.AddSingleton<ILibrary>(library);
        builder.Services.AddSingleton<LibraryViewModel>();
        builder.Services.AddSingleton<BooksViewModel>();


#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}