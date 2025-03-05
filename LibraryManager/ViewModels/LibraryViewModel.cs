using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LibraryManager.Models;
using LibraryManager.Views;

namespace LibraryManager.ViewModels;

public class LibraryViewModel
{
    private readonly ILibrary _library;

    public LibraryViewModel(ILibrary library)
    {
        _library = library; // Constructor injection ensures proper dependency handling
        // Initialize the generic navigation command
        NavigateCommand = new AsyncRelayCommand<string>(async (route) => await NavigateToPage(route));

        MessagingCenter.Subscribe<BooksViewModel>(this, "Navigate", async (sender) =>
        {
            Console.WriteLine("Received navigation request from BooksViewModel.");
            await NavigateToPage("CreateLibrary");
        });
    }

    /*public LibraryViewModel()
    {
        _library = App.Services.GetService<ILibrary>();

        // Initialize the generic navigation command
        NavigateCommand = new AsyncRelayCommand<string>(async (route) => await NavigateToPage(route));

        MessagingCenter.Subscribe<BooksViewModel>(this, "Navigate", async (sender) =>
        {
            Console.WriteLine("Received navigation request from BooksViewModel.");
            await NavigateToPage("CreateLibrary");
        });
    }*/

    public ICommand NavigateCommand { get; }

    /* LibraryLoadCommand = new AsyncRelayCommand(ShowCustomDialogPage);
      public ICommand LibraryLoadCommand { get; }*/

    private Task NavigateToPage(string route)
    {
        Console.WriteLine($"NavigateCommand triggered with route: {route}");

        if (string.IsNullOrWhiteSpace(route))
            return Task.CompletedTask;

        try
        {
            // Prevent navigation to the same page
            var currentRoute = Shell.Current.CurrentState.Location.OriginalString;
            if (currentRoute == $"//LibraryManagePage")
            {
#if DEBUG
                Console.WriteLine($"You're already on the {route} page. Navigation skipped.");
#endif

                return Task.CompletedTask;
            }

            // Dynamically navigate using the provided route
            Shell.Current.GoToAsync(route);
        }
        catch (Exception ex)
        {
            // Handle any issues with navigation
#if DEBUG
            Console.WriteLine($"Navigation error: {ex.Message}");
#endif
        }

        return Task.CompletedTask;
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