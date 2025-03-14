using LibraryManager.Views;
using System.Diagnostics;

namespace LibraryManager.ViewModels;

public class FindBooksViewModel: AbstractViewModel
{
    public FindBooksViewModel()
    {
        
    }
    
    #region Public Methods
    protected override async Task PerformAction(string? commandParameter)
    {
        Debug.WriteLine($"NavigateCommand triggered with commandParameter: {commandParameter}");

        if (string.IsNullOrWhiteSpace(commandParameter))
            return;

        // Prevent navigation to the same page - 'Shell.Current.GoToAsync(...'
        if (IsCurrentRoute(nameof(FindBooksPage)))
        {
            switch (commandParameter)
            {
                case nameof(LibraryPage):
                case nameof(BooksPage):
                case nameof(AboutPage):
                case nameof(ToolsPage):
                {
                    try
                    {
                        // Dynamically navigate using the provided commandParameter
                        // begins '//' added in the beginning to switch a Menu as well as Page. without '//' it switch only Page
                        Shell.Current.GoToAsync($"//{commandParameter}").ConfigureAwait(false);
                    }
                    catch (Exception ex) // Handle any issues with navigation
                    {
                        #if DEBUG
                        Debug.WriteLine($"Navigation error: {ex.Message}");
                        #endif
                    }
                }
                    break;
                default: //jobs perform without creating views
                {
                    try
                    {
                        // Try opening the URL using the platform default browser
                        await Browser.OpenAsync(commandParameter, BrowserLaunchMode.SystemPreferred);
                    }
                    catch (Exception e)
                    {
                        #if DEBUG
                        Debug.WriteLine(e.Message);
                        #endif
                    }

                    break;
                }
            }
        }
        else
        {
            #if DEBUG
            Debug.WriteLine(
                $"Navigation error path '{commandParameter}' in class '{nameof(FindBooksViewModel)}' by method '{nameof(PerformAction)}'");
            #endif
        }
    }
    #endregion
    
}