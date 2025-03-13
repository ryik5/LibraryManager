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
        ExtendedCommand = new AsyncRelayCommand<string>(PerformExtendedAction);
    }

    /// <summary>
    /// Performs an action based on the provided command parameter.
    /// This is a method and should be overridden by derived classes to implement specific actions.
    /// </summary>
    /// <param name="commandParameter">The command parameter specifying the action to be performed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected abstract Task PerformAction(string? commandParameter);

    /// <summary>
    /// Performs an extended action based on the provided command parameter.
    /// This is a virtual method and can be overridden by derived classes to implement specific extended actions.
    /// </summary>
    /// <param name="commandParameter">The command parameter specifying the action to be performed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task PerformExtendedAction(string? commandParameter)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Shows a display alert to the user, with the option to either press "OK" or "Cancel".
    /// </summary>
    /// <param name="title">The title of the alert.</param>
    /// <param name="message">The message to be displayed in the alert.</param>
    /// <returns>A task representing the asynchronous operation. The result of the task
    /// is true if the user pressed "OK", false if the user pressed "Cancel".</returns>
    protected async Task<bool> ShowDisplayAlertAsync(string title, string message, string ok = "OK",
        string cancel = "No")
    {
        bool result = await Application.Current.MainPage.DisplayAlert(
            title,
            message,
            ok,
            cancel
        );

        if (result)
        {
            #if DEBUG
            Debug.WriteLine("User pressed OK");
            #endif
        }
        else
        {
            #if DEBUG
            Debug.WriteLine("User pressed Cancel");
            #endif
        }

        return result;
    }

    protected async Task<bool> ShowCustomDialogPage(string title, string message)
    {
        var dialogPage = new CustomDialogPage(title, message);
        await Application.Current?.MainPage?.Navigation.PushModalAsync(dialogPage)!;

        var result = await dialogPage.DialogResultTask.Task; // Await the user's response

        #if DEBUG
        Debug.WriteLine(result ? "User pressed OK." : "User pressed Cancel.");
        #endif

        return result;
    }


    #region Public Properties
    public ICommand NavigateCommand { get; set; }

    public ICommand ExtendedCommand { get; }


    /// <summary>
    /// Returns Shell.Current.CurrentState.Location.OriginalString
    /// </summary>
    protected string CurrentRoute => Shell.Current.CurrentState.Location.OriginalString;
    #endregion
}