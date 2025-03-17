using LibraryManager.Views;
using System.Diagnostics;

namespace LibraryManager.ViewModels;

public class ToolsViewModel : AbstractViewModel
{
    private SettingsViewModel _settings;

    public ToolsViewModel()
    {
        Settings = new SettingsViewModel();
    }


    #region Public properties
    public SettingsViewModel Settings
    {
        get => _settings;
        set => SetProperty(ref _settings, value);
    }

    public string Save => Constants.SAVE;
    public string Cancel => Constants.CANCEL;
    #endregion

    #region Public Methods
    protected override async Task PerformAction(string? commandParameter)
    {
        Debug.WriteLine($"NavigateCommand triggered with commandParameter: {commandParameter}");

        if (string.IsNullOrWhiteSpace(commandParameter))
            return;

        // Prevent navigation to the same page - 'Shell.Current.GoToAsync(...'
        if (IsCurrentRoute(nameof(ToolsPage)))

        {
            switch (commandParameter)
            {
                case nameof(LibraryPage):
                case nameof(BooksPage):
                case nameof(FindBooksPage):
                case nameof(AboutPage):
                {
                    await TryGoToPage(commandParameter);
                    break;
                }

                case Constants.SAVE:
                    Settings.SaveSettings();
                    break;
                case Constants.CANCEL:
                    Settings.LoadAllSettings();
                    break;
                default:
                {
                    await ShowCustomDialogPage(Constants.FOLDER_WAS_NOT_SELECTED, $"{commandParameter}"); //Ok, Cancel
                    break;
                }
            }
        }
        else
        {
            await ShowDebugNavigationError(commandParameter, nameof(ToolsViewModel));
        }
    }
    #endregion
}