using LibraryManager.Models;
using LibraryManager.Views;

namespace LibraryManager;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for non-hierarchical pages.
        Routing.RegisterRoute(nameof(LibraryPage), typeof(LibraryPage));
        Routing.RegisterRoute(nameof(BooksPage), typeof(BooksPage));
        Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));
        Routing.RegisterRoute(nameof(FindBooksPage), typeof(FindBooksPage));
        Routing.RegisterRoute(nameof(ToolsPage), typeof(ToolsPage));

        // App Logo. Assign the custom drawable to the GraphicsView
        LogoGraphicsView.Drawable = new CustomDrawable();
        
        // App Time
        _cancellationTokenSource = new CancellationTokenSource();
        UpdateTimeOnUI(_cancellationTokenSource);
        
      //  AppButton.Command=new Command(async () =>AppInfo.Current.ShowSettingsUI());

    }

    protected override void OnDisappearing()
    {
        _cancellationTokenSource.Cancel();
        base.OnDisappearing();
    }

    /// <summary>
    /// Invokes the specified action on the UI thread.
    /// </summary>
     /// <remarks>
    /// This method is used to update the time on the UI thread.
    /// </remarks>
    private Task UpdateTimeOnUI(CancellationTokenSource cts)
    {
        // Run the task on a separate thread.
        return Task.Run(() =>
        {
            // Loop until the cancellation token is cancelled.
            while (!cts.IsCancellationRequested)
            {
                // Update the time label on the UI thread.
                MainThread.BeginInvokeOnMainThread(() => TimeLabel.Text = DateTime.Now.ToString("hh:mm:ss tt"));

                // Sleep for 1 second.
                Thread.Sleep(1000);
            }
        }, cts.Token);
    }

    private readonly CancellationTokenSource _cancellationTokenSource;
}