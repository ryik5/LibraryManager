using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LibraryManager.Models;
using LibraryManager.Views;

namespace LibraryManager.ViewModels;

public class LibraryViewModel
{
    private readonly ILibrary? _library;

    public LibraryViewModel(ILibrary? library)
    {
        Console.WriteLine("LibraryViewModel instantiated with library.");
        _library = library; // Constructor injection ensures proper dependency handling
        // Initialize the generic navigation command
        NavigateCommand = new AsyncRelayCommand<string>(NavigateToPage);

        /*MessagingCenter.Subscribe<BooksViewModel>(this, "Navigate", async (sender) =>
        {
            Console.WriteLine("Received navigation request from BooksViewModel.");
           // await NavigateToPage("CreateLibrary").ConfigureAwait(false);
        });*/
    }

    public LibraryViewModel()
    {
        Console.WriteLine("LibraryViewModel instantiated with library.");

        _library = App.Services.GetService<ILibrary>();

        // Initialize the generic navigation command
        NavigateCommand = new AsyncRelayCommand<string>(NavigateToPage);

        /*MessagingCenter.Subscribe<BooksViewModel>(this, "Navigate", async (sender) =>
        {
            Console.WriteLine("Received navigation request from BooksViewModel.");
           // await NavigateToPage("CreateLibrary").ConfigureAwait(false);
        });*/
    }

    // Add cleanup method to unsubscribe
    public void Cleanup()
    {
        // MessagingCenter cleanup
        Console.WriteLine("Cleaning up MessagingCenter resources in LibraryViewModel.");
        MessagingCenter.Unsubscribe<BooksViewModel>(this, "Navigate");
    }
    ~LibraryViewModel()
    {
        Cleanup(); // Safeguard cleanup in destructor (if proper disposal is skipped)
    }


    public ICommand NavigateCommand { get; }
    

    private async Task NavigateToPage(string? route)
    {
        Console.WriteLine($"NavigateCommand triggered with route: {route}");

        if (string.IsNullOrWhiteSpace(route))
            return;

        try
        {
            // Prevent navigation to the same page
            var currentRoute = Shell.Current.CurrentState.Location.OriginalString;
            if (currentRoute == $"//{nameof(LibraryManagePage)}")
            {
#if DEBUG
                Console.WriteLine($"You're already on the {route} page. Navigation skipped.");
#endif
            }

            // Dynamically navigate using the provided route
          // await Shell.Current.GoToAsync(route).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // Handle any issues with navigation
#if DEBUG
            Console.WriteLine($"Navigation error: {ex.Message}");
#endif
        }
    }


    private async Task LibraryCreateNewAsync()
    {
        await ShowMessageAsync("Message", "Pressed - CreateNewLibrary");
    }

    private async Task ShowMessageAsync(string title, string message)
    {
        bool result = await Application.Current.MainPage.DisplayAlert(
            title,
            message,
            "OK",
            "Cancel"
        );

        if (result)
        {
            Debug.WriteLine("User pressed OK");
        }
        else
        {
            Debug.WriteLine("User pressed Cancel");
        }
    }

    private async Task ShowCustomDialogPage()
    {
        var dialogPage = new CustomDialogPage("Custom Modal Dialog", "Would you like to continue?");
        await Application.Current.MainPage.Navigation.PushModalAsync(dialogPage);

        var result = await dialogPage.DialogResultTask.Task; // Await the user's response

#if DEBUG
        Debug.WriteLine(result ? "User pressed OK." : "User pressed Cancel.");
#endif
    }

    private async Task NavigateToNewLibrary()
    {
        // Navigate to NewLibrary page
        await Shell.Current.GoToAsync("NewLibrary");
    }
}