namespace LibraryManager;

public partial class App : Application
{
    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        Services = serviceProvider;

        MainPage = new AppShell();
    }

    /// <summary>
    /// DI for singletons // TODO : Check necessary
    /// </summary>
    public static IServiceProvider Services { get; private set; }
}