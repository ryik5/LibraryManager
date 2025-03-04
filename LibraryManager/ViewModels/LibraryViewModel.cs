using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibraryManager.Views;

namespace LibraryManager.ViewModels;

public class LibraryViewModel : ObservableObject
{
    public LibraryViewModel()
    {
        // Initialize the generic navigation command
        NavigateCommand = new AsyncRelayCommand<string>(async (route) => await NavigateToPage(route));
        
    }

    public ICommand NavigateCommand { get; }

  /* LibraryLoadCommand = new AsyncRelayCommand(ShowCustomDialogPage);
    public ICommand LibraryLoadCommand { get; }*/
    
    private async Task NavigateToPage(string route)
    {
        if (string.IsNullOrWhiteSpace(route))
            return;

        try
        {
            // Dynamically navigate using the provided route
            await Shell.Current.GoToAsync(route);
        }
        catch (Exception ex)
        {
            // Handle any issues with navigation
            Console.WriteLine($"Navigation error: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine("User pressed OK");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("User pressed Cancel");
        }
    }
    
    private async Task ShowCustomDialogPage()
    {
        var dialogPage = new CustomDialogPage("Custom Modal Dialog", "Would you like to continue?");
        await Application.Current.MainPage.Navigation.PushModalAsync(dialogPage);

        var result = await dialogPage.DialogResultTask.Task; // Await the user's response

        System.Diagnostics.Debug.WriteLine(result ? "User pressed OK." : "User pressed Cancel.");
    }
    
    private async Task NavigateToNewLibrary()
    {
        // Navigate to NewLibrary page
        await Shell.Current.GoToAsync("NewLibrary");
    }

}


