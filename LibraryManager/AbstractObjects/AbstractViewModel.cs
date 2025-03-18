using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LibraryManager.AbstractObjects;
using LibraryManager.Views;

namespace LibraryManager.ViewModels;

public abstract class AbstractViewModel : AbstractBindableModel
{
    protected AbstractViewModel()
    {
        // Initialize the generic navigation command
        NavigateCommand = new AsyncRelayCommand<string>(PerformAction);
        ExtendedCommand = new AsyncRelayCommand<string>(PerformExtendedAction);
    }


    #region Public Methods
    /// <summary>
    /// Performs an action based on the provided command parameter.
    /// This is a method and should be overridden by derived classes to implement specific actions.
    /// </summary>
    /// <param name="commandParameter">The command parameter specifying the action to be performed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected abstract Task PerformAction(string? commandParameter);

    /// <summary>
    /// Performs an extended action based on the provided command parameter.
    /// This is a virtual empty method and can be overridden by derived classes to implement specific extended actions.
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

    // <summary>
    /// Displays a custom dialog page with a title and message, and returns a boolean indicating whether the user pressed OK or Cancel.
    /// </summary>
    /// <param name="title">The title of the dialog page.</param>
    /// <param name="message">The message to display on the dialog page.</param>
    /// <returns>A boolean indicating whether the user pressed OK or Cancel.</returns>
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

    /// <summary>
    /// Attempts to navigate to an application's page specified by a command parameter, handling any navigation errors that may occur.
    /// </summary>
    /// <param name="commandParameter">The command parameter specifying the page to navigate to.</param>
    protected async Task TryGoToPage(string commandParameter)
    {
        try
        {
            // Dynamically navigate using the provided commandParameter
            // begins '//' added in the beginning to switch a Menu as well as Page. without '//' it switch only Page
            await Shell.Current.GoToAsync($"//{commandParameter}").ConfigureAwait(false);
        }
        catch (Exception ex) // Handle any issues with navigation
        {
            #if DEBUG
            Debug.WriteLine($"Navigation error: {ex.Message}");
            #endif
        }
    }

    /// <summary>
    /// Attempts to open a URL in the platform's default web browser, handling any errors that may occur.
    /// </summary>
    /// <param name="url">The URL to open in the browser.</param>
    protected async Task TryOpenBrowser(string url)
    {
        try
        {
            // Try opening the URL using the platform default browser
            await Browser.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception e)
        {
            #if DEBUG
            Debug.WriteLine(e.Message);
            #endif
        }

        /*try
        {
            await Launcher.OpenAsync(url);
        }
        catch (Exception ex) // Handle any issues with navigation
        {
            #if DEBUG
            Debug.WriteLine($"Navigation error: {ex.Message}");
            #endif
        }*/
    }

    /// <summary>
    /// Checks if the current route is equal to the provided page.
    /// </summary>
    /// <param name="page">The page to compare the current route with.</param>
    /// <returns>true if the current route is equal to the provided page, false otherwise.</returns>
    protected bool IsCurrentRoute(string page) =>
        Shell.Current.CurrentState.Location.OriginalString == $"//{page}";

    protected Task ShowNavigationErrorInDebug(string commandParameter, string className)
    {
        #if DEBUG
        Debug.WriteLine(
            $"Navigation error path '{commandParameter}' in class '{className}' by method '{nameof(PerformAction)}'");
        #endif

        return Task.CompletedTask;
    }

    protected Task ShowNavigationCommandInDebug(string commandParameter, string className)
    {
        #if DEBUG
        Debug.WriteLine(
            $"NavigateCommand on {className} triggered with commandParameter: {commandParameter}");
        #endif

        return Task.CompletedTask;
    }
    #endregion


    #region Public Properties
    /// <summary>
    /// Command to perform an action, such as navigate to a different page or view or other actions.
    /// </summary>
    public ICommand NavigateCommand { get; set; }

    /// <summary>
    /// Command to perform an extra actions.
    /// </summary>
    public ICommand ExtendedCommand { get; }
    #endregion
}