using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;

namespace LibraryManager.AbstractObjects;

public abstract class AbstractViewModel : AbstractBindableModel
{
    protected AbstractViewModel()
    {
        // Initialize the generic navigation command
        NavigateCommand = new AsyncRelayCommand<string>(PerformAction);
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
    /// Refreshes the controls on appearing by executing the tasks in the following order:
    /// <list type="number">
    /// <item>HandleBeforeRefreshControlsOnAppearingTask</item>
    /// <item>Task.Delay(20)</item>
    /// <item>HandlePostRefreshControlsOnAppearingTask</item>
    /// </list>
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task RefreshControlsOnAppearing()
    {
        await HandleBeforeRefreshControlsOnAppearingTask();
        await Task.Delay(20);
        await HandlePostRefreshControlsOnAppearingTask();
    }

    /// <summary>
    /// Handles operations that should occur before refreshing the controls when appearing.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual async Task HandleBeforeRefreshControlsOnAppearingTask()
    {
    }

    /// <summary>
    /// Handles operations that should occur after refreshing the controls when appearing.
    /// This method can be overridden by derived classes to implement specific post-refresh actions.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual async Task HandlePostRefreshControlsOnAppearingTask()
    {
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


    /// <summary>
    /// Attempts to navigate to an application's page specified by a command parameter, handling any navigation errors that may occur.
    /// </summary>
    /// <param name="commandParameter">The command parameter specifying the page to navigate to.</param>
    protected async Task TryGoToPage(string commandParameter)
    {
        var gotoPageTask = GoToPage(commandParameter);
        await Task.Delay(20);
        await gotoPageTask;
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
    protected static bool IsCurrentRoute(string page) =>
        Shell.Current.CurrentState.Location.OriginalString == $"//{page}";


    protected Task ShowNavigationErrorInDebug(string commandParameter, string className)
    {
        #if DEBUG
        Debug.WriteLine(
            $"Navigation error path '{commandParameter}' in class '{className}' by method '{nameof(PerformAction)}'");
        #endif

        return Task.CompletedTask;
    }


    /// <summary>
    /// Displays the navigation command parameter in debug mode.
    /// </summary>
    /// <remarks>
    /// This method is used to debug the navigation commands. It shows the command parameter and the class name
    /// of the view model that triggered the navigation command.
    /// </remarks>
    /// <param name="commandParameter">The command parameter.</param>
    /// <param name="className">The name of the view model class.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected Task ShowNavigationCommandInDebug(string commandParameter, string className)
    {
        #if DEBUG
        Debug.WriteLine($"NavigateCommand on {className} triggered with commandParameter: {commandParameter}");
        #endif

        return Task.CompletedTask;
    }


    /// <summary>
    /// Determines whether the specified number is not null and not zero.
    /// </summary>
    /// <param name="number">The number to check.</param>
    /// <returns>true if the number is not null and not zero; otherwise, false.</returns>
    protected static bool IsNotZero(int? number)
    {
        if (number is not int || number == 0)
            return false;

        return true;
    }
    #endregion


    /// <summary>
    /// Attempts to navigate to an application's page specified by a command parameter, handling any navigation errors that may occur.
    /// </summary>
    /// <param name="commandParameter">The command parameter specifying the page to navigate to.</param>
    private async Task GoToPage(string commandParameter)
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
}