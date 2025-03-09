using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LibraryManager.AbstractObjects;
using LibraryManager.Views;

namespace LibraryManager.ViewModels;

public abstract class AbstractViewModel : AbstractBindableUiManager
{
    protected AbstractViewModel()
    {
        // Initialize the generic navigation command
        NavigateCommand = new AsyncRelayCommand<string>(PerformAction);
    }

    protected abstract Task PerformAction(string? commandParameter);

    protected async Task ShowDisplayAlertAsync(string title, string message)
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

    protected async Task ShowCustomDialogPage(string title, string message)
    {
        var dialogPage = new CustomDialogPage(title, message);
        await Application.Current?.MainPage?.Navigation.PushModalAsync(dialogPage)!;

        var result = await dialogPage.DialogResultTask.Task; // Await the user's response

#if DEBUG
        Debug.WriteLine(result ? "User pressed OK." : "User pressed Cancel.");
#endif
    }


    #region Public Properties

    public ICommand NavigateCommand { get; set; }

    /// <summary>
    /// Returns Shell.Current.CurrentState.Location.OriginalString
    /// </summary>
    protected string CurrentRoute => Shell.Current.CurrentState.Location.OriginalString;
    #endregion
}