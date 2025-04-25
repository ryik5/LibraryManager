using LibraryManager.AbstractObjects;
using LibraryManager.Views;

namespace LibraryManager.ViewModels;

/// <summary>
/// View model for the tools page.
/// </summary>
/// <author>YR 2025-03-09</author>
public class ToolsViewModel : AbstractViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToolsViewModel"/> class.
    /// </summary>
    /// <param name="settings">The settings view model.</param>
    /// <param name="statusBar">The status bar.</param>
    public ToolsViewModel(SettingsViewModel settings, IStatusBar statusBar)
    {
        StatusBar = statusBar;
        Settings = settings;
        Settings.LoadAllSettings().ConfigureAwait(false);
        IsSettingsViewVisible = true;
    }


    #region Public properties
    /// <summary>
    /// Gets or sets a value indicating whether the settings view is visible.
    /// </summary>
    public bool IsSettingsViewVisible
    {
        get => _isSettingsVisible;
        set => SetProperty(ref _isSettingsVisible, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the debug view is visible.
    /// </summary>
    public bool IsDebugViewVisible
    {
        get => _isDebugViewVisible;
        set => SetProperty(ref _isDebugViewVisible, value);
    }

    /// <summary>
    /// Gets or sets the settings view model.
    /// </summary>
    public SettingsViewModel Settings
    {
        get => _settings;
        set => SetProperty(ref _settings, value);
    }

    /// <summary>
    /// Gets or sets the status bar.
    /// </summary>
    public IStatusBar StatusBar
    {
        get => _statusBar;
        set => SetProperty(ref _statusBar, value);
    }
    #endregion

    #region CommandParameters
    public string SettingsView => Constants.SETTINGS;
    public string DebugView => Constants.DEBUG;
    public string ToolsView => Constants.TOOLS;
    public string Save => Constants.SAVE;
    public string Cancel => Constants.CANCEL;
    public string Reset => Constants.RESET;
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
                    await Settings.SaveSettings();
                    break;

                case Constants.CANCEL:
                    await Settings.LoadAllSettings();
                    break;

                case Constants.RESET:
                    await Settings.ResetAllSettings();
                    break;

                default:
                    await ShowNavigationErrorInDebug($"Unknown command '{commandParameter}'", nameof(ToolsViewModel));
                    break;
            }

            RaisePropertyChanged(nameof(Settings));
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