using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LibraryManager.ViewModels;

public class LibraryViewModel : ObservableObject
{
    public LibraryViewModel()
    {
        LibraryCreateNewCommand = new AsyncRelayCommand(CreateNewLibraryAsync);
    }

    public ICommand LibraryCreateNewCommand { get; }

    private async Task CreateNewLibraryAsync()
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
}