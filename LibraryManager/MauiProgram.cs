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
            BookList = new ObservableCollection<Book>()
        };

        // Register all shared ViewModels and objects
        builder.Services.AddSingleton<ILibrary>(library);
        builder.Services.AddSingleton<LibraryViewModel>();
        builder.Services.AddSingleton<BooksViewModel>();
        builder.Services.AddSingleton<AboutViewModel>();
        builder.Services.AddSingleton<ToolsViewModel>();
        builder.Services.AddSingleton<FindBooksViewModel>();
        builder.Services.AddSingleton<Book>();

        #if DEBUG
        builder.Logging.AddDebug();
        #endif

        return builder.Build();
    }
}