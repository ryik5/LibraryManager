namespace LibraryManager;

/// <author>YR 2025-02-09</author>
public partial class App : Application
{
    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        Services = serviceProvider;

        MainPage = new AppShell();
    }

    /// <summary>
    /// DI for singletons
    /// </summary>
    public static IServiceProvider Services { get; private set; }
}