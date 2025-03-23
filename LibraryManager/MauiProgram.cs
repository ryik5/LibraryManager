using LibraryManager.AbstractObjects;
using System.Collections.ObjectModel;
using LibraryManager.Models;
using LibraryManager.ViewModels;
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

        /*// Configuration
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("LibraryManager.appsettings.json");
        var config = new ConfigurationBuilder().AddJsonStream(stream).Build();
        builder.Configuration.AddConfiguration(config);*/
        
        var library = new Library
        {  
            Id = 0,
            Name = "",
            Description = "",
            BookList = new ObservableCollection<Book>()
        };
        var settings = new SettingsViewModel();
        
        builder.Services.AddTransient<IFolderPicker, Platforms.MacCatalyst.FolderPicker>();
        builder.Services.AddTransient<Book>();
        builder.Services.AddTransient<App>();
        
        // Register all shared ViewModels and objects
        builder.Services.AddSingleton<ILibrary>(library);
        
        builder.Services.AddSingleton(settings);
        builder.Services.AddSingleton<LibraryViewModel>();
        builder.Services.AddSingleton<BooksViewModel>();
        builder.Services.AddSingleton<FindBooksViewModel>();
        builder.Services.AddSingleton<AboutViewModel>();
        builder.Services.AddSingleton<ToolsViewModel>();
        #if DEBUG
        builder.Logging.AddDebug();
        #endif

        return builder.Build();
    }
}