using CommunityToolkit.Mvvm.Messaging;
using LibraryManager.AbstractObjects;
using LibraryManager.Controls;
using System.Collections.ObjectModel;
using LibraryManager.Models;
using LibraryManager.ViewModels;
using Microsoft.Extensions.Logging;

namespace LibraryManager;

/// <author>YR 2025-02-09</author>
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
            Id = new Random().Next(),
            Name = "Empty Library",
            Description = "",
            BookList = new ObservableCollection<Book>()
        };
        builder.Services.AddTransient<IFolderPicker, Platforms.MacCatalyst.FolderPicker>();
        var settings = new SettingsViewModel();
        var statusBar = new StatusBarViewModel();
        var package = AppInfo.Current.PackageName;
        WeakReferenceMessenger.Default.Send(new StatusMessage(){ InfoKind = EInfoKind.DebugInfo, Message = $"{AppInfo.Current.Name} started with package '{package}'."});
        
       // statusBar.SetStatusMessage(EInfoKind.DebugInfo, $"{AppInfo.Current.Name} started with package '{package}'.");
       WeakReferenceMessenger.Default.Send(new StatusMessage(){ InfoKind = EInfoKind.CommonInfo, Message = $"v.{AppInfo.Current.VersionString}, b.{AppInfo.Current.BuildString}"});
        //statusBar.SetStatusMessage(EInfoKind.CommonInfo,  $"v.{AppInfo.Current.VersionString}, b.{AppInfo.Current.BuildString}");
       // statusBar.SetStatusMessage(EInfoKind.CurrentInfo,$"Current Library ID: '{library.Id}'");
       WeakReferenceMessenger.Default.Send(new StatusMessage(){ InfoKind = EInfoKind.CurrentInfo, Message = $"Current Library ID: '{library.Id}'"});
       // statusBar.SetStatusMessage(EInfoKind.TotalBooks,0);
       WeakReferenceMessenger.Default.Send(new StatusMessage(){ InfoKind = EInfoKind.TotalBooks, Message = "0"});

        builder.Services.AddTransient<Book>();
        builder.Services.AddTransient<App>();

        // Register all shared ViewModels and objects
        builder.Services.AddSingleton<ILibrary>(library);

        builder.Services.AddSingleton<IStatusBar>(statusBar);
        builder.Services.AddTransient<StatusBarPanel>();

        builder.Services.AddSingleton(settings);
        builder.Services.AddSingleton<LibraryViewModel>();
        builder.Services.AddSingleton<BooksViewModel>();
        builder.Services.AddSingleton<FindBooksViewModel>();

        builder.Services.AddTransient<AboutViewModel>();
        builder.Services.AddTransient<ToolsViewModel>();
        #if DEBUG
        builder.Logging.AddDebug();
        #endif

        return builder.Build();
    }
}