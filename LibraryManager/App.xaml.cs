using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Messaging;
using LibraryManager.Models;
using LibraryManager.ViewModels;

namespace LibraryManager;

public partial class App : Application
{
    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        Services = serviceProvider;
        
        MainPage = new AppShell();
    }

    public static IServiceProvider Services { get; private set; }
    
}