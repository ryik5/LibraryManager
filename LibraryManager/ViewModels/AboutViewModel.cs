using LibraryManager.AbstractObjects;
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
        await   ShowNavigationCommandInDebug(commandParameter,nameof(AboutPage));

        if (string.IsNullOrWhiteSpace(commandParameter))
            return;

        // Prevent navigation to the same page - 'Shell.Current.GoToAsync(...'
        //IsCurrentRoute == $"//{nameof(AboutPage)}"
        if (IsCurrentRoute(nameof(AboutPage)))
        {
            switch (commandParameter)
            {
                case nameof(LibraryPage):
                case nameof(BooksPage):
                case nameof(FindBooksPage):
                case nameof(ToolsPage):
                    await TryGoToPage(commandParameter);
                    break;

                default: //jobs perform without creating views
                    await TryOpenBrowser(commandParameter);
                    break;
            }
        }
        else
        {
            await ShowNavigationErrorInDebug(commandParameter, nameof(AboutViewModel));
        }
    }
    #endregion
}