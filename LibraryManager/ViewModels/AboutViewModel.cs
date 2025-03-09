using System.Diagnostics;
using System.Reflection;
using LibraryManager.Views;

namespace LibraryManager.ViewModels;

public class AboutViewModel : AbstractViewModel
{
    public AboutViewModel()
    {
        if (Assembly.GetEntryAssembly()?.Location is string fileName)
        {
            var versionInfo = FileVersionInfo.GetVersionInfo(fileName);

            Header =
                $"App.{versionInfo.ProductName}, Author:'{versionInfo.CompanyName}', build:'{versionInfo.FileMajorPart}.{versionInfo.FileMinorPart}.{versionInfo.FileBuildPart}.{versionInfo.FilePrivatePart}'";
            Footer = $"Developer: @YR{Environment.NewLine}Designer: @Ila Yavorska";
        }
    }


    #region Properties

    /// <summary>
    /// Gets the Header of the About Page.
    /// </summary>
    public string Header { get; }

    /// <summary>
    /// Gets the Footer of the About Page.
    /// </summary>
    public string Footer { get; }

    #endregion


    #region Public Methods

    protected override async Task PerformAction(string? commandParameter)
    {
        Debug.WriteLine($"NavigateCommand triggered with commandParameter: {commandParameter}");

        if (string.IsNullOrWhiteSpace(commandParameter))
            return;

        // Prevent navigation to the same page - 'Shell.Current.GoToAsync(...'
        var currentRoute = Shell.Current.CurrentState.Location.OriginalString;
        if (currentRoute == $"//{nameof(AboutPage)}")
        {
            switch (commandParameter)
            {
                case nameof(BooksPage):
                case nameof(LibraryPage):
                {
                    try
                    {
                        // Dynamically navigate using the provided commandParameter
                        // begins '//' added in the beginning to switch a Menu as well as Page. without '//' it switch only Page
                        Shell.Current.GoToAsync($"//{commandParameter}").ConfigureAwait(false);
                    }
                    catch (Exception ex) // Handle any issues with navigation
                    {
                        Debug.WriteLine($"Navigation error: {ex.Message}");
                    }
                }
                    break;
                default: //jobs perform without creating views
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(commandParameter))
                        {
                            // Try opening the URL using the platform default browser
                            await Browser.OpenAsync(commandParameter, BrowserLaunchMode.SystemPreferred);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                }
                    break;
            }
        }
        else
        {
#if DEBUG
            Debug.WriteLine(
                $"Navigation error path '{commandParameter}' in class '{nameof(AboutViewModel)}' by method '{nameof(PerformAction)}'");
#endif
        }
    }

    #endregion
}