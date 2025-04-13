using LibraryManager.AbstractObjects;
using LibraryManager.Views;

namespace LibraryManager.ViewModels;

public class ToolsViewModel : AbstractViewModel
{
    public ToolsViewModel(SettingsViewModel settings, IStatusBar statusBar)
    {
        StatusBar = statusBar;
        Settings = settings;
        Settings.LoadAllSettings().ConfigureAwait(false);
        IsSettingsViewVisible = true;
    }


    #region Public properties
    public bool IsSettingsViewVisible
    {
        get => _isSettingsVisible;
        set => SetProperty(ref _isSettingsVisible, value);
    }

    public bool IsDebugViewVisible
    {
        get => _isDebugViewVisible;
        set => SetProperty(ref _isDebugViewVisible, value);
    }

    public SettingsViewModel Settings
    {
        get => _settings;
        set => SetProperty(ref _settings, value);
    }

    public IStatusBar StatusBar
    {
        get => _statusBar;
        set => SetProperty(ref _statusBar, value);
    }

    #region CommandParameters
    public string SettingsView => Constants.SETTINGS;
    public string DebugView => Constants.DEBUG;
    public string ToolsView => Constants.TOOLS;
    public string Save => Constants.SAVE;
    public string Cancel => Constants.CANCEL;
    public string Reset => Constants.RESET;
    #endregion
    #endregion


    #region Public Methods
    protected override async Task PerformAction(string? commandParameter)
    {
        await ShowNavigationCommandInDebug(commandParameter, nameof(ToolsPage));

        if (string.IsNullOrWhiteSpace(commandParameter))
            return;

        if (IsCurrentRoute(nameof(ToolsPage))) // Prevent navigation to the same page - 'Shell.Current.GoToAsync(...'
        {
            switch (commandParameter)
            {
                case nameof(LibraryPage):
                case nameof(BooksPage):
                case nameof(FindBooksPage):
                case nameof(AboutPage):
                    await TryGoToPage(commandParameter);
                    break;

                case Constants.SETTINGS: //SettingsView
                    IsDebugViewVisible = false;
                    IsSettingsViewVisible = true;
                    break;

                case Constants.DEBUG: //DebugView
                    IsDebugViewVisible = true;
                    IsSettingsViewVisible = false;
                    break;

                case Constants.TOOLS: //ToolsView
                    IsDebugViewVisible = false;
                    IsSettingsViewVisible = false;
                    break;

                case Constants.SAVE:
                    await
                        Settings.SaveSettings();
                    break;

                case Constants.CANCEL:
                    await Settings.LoadAllSettings();
                    break;

                case Constants.RESET:
                    await Settings.ResetAllSettings();
                    break;

                default:
                    await ShowCustomDialogPage(Constants.UNKNOWN_COMMAND,
                        $"Unknown command '{commandParameter}'on {nameof(ToolsViewModel)}");
                    break;
            }
        }
        else
        {
            await ShowNavigationErrorInDebug(commandParameter, nameof(ToolsViewModel));
        }
    }
    #endregion


    #region Private fields
    private SettingsViewModel _settings;
    private bool _isSettingsVisible;
    private bool _isDebugViewVisible;
    private IStatusBar _statusBar;
    #endregion
}