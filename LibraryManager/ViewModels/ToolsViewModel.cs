using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using LibraryManager.AbstractObjects;
using LibraryManager.Models;
using LibraryManager.Views;

namespace LibraryManager.ViewModels;

/// <summary>
/// View model for the tools page.
/// </summary>
/// <author>YR 2025-03-09</author>
public sealed partial class ToolsViewModel : AbstractViewModel
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
    }


    #region Public properties
    /// <summary>
    /// Gets or sets a value indicating whether the settings view is visible.
    /// </summary>
    [ObservableProperty] bool isSettingsViewVisible = true;

    /// <summary>
    /// Gets or sets a value indicating whether the debug view is visible.
    /// </summary>
    [ObservableProperty] private bool _isDebugViewVisible;

    /// <summary>
    /// Gets or sets the settings view model.
    /// </summary>
    [ObservableProperty] private SettingsViewModel _settings;

    /// <summary>
    /// Gets or sets the status bar.
    /// </summary>
    [ObservableProperty] private IStatusBar _statusBar;
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
        #if DEBUG
        await ShowNavigationCommandInDebug(commandParameter, nameof(ToolsPage));
        #endif
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
                    #if DEBUG
                    WeakReferenceMessenger.Default.Send(new StatusMessage()
                    {
                        InfoKind = EInfoKind.DebugInfo,
                        LogLevel = ELogLevel.Debug,
                        Message = $"Selected Settings view"
                    });
                    #endif
                    break;

                case Constants.DEBUG: //DebugView
                    IsDebugViewVisible = true;
                    IsSettingsViewVisible = false;
                    #if DEBUG
                    WeakReferenceMessenger.Default.Send(new StatusMessage()
                    {
                        InfoKind = EInfoKind.DebugInfo,
                        Message = $"Selected Debug view"
                    });
                    #endif
                    break;

                case Constants.TOOLS: //ToolsView
                    IsDebugViewVisible = false;
                    IsSettingsViewVisible = false;
                    break;

                case Constants.SAVE:
                case Constants.CANCEL:
                case Constants.RESET:
                    await Settings.PerformAction(commandParameter);
                    break;
                default:
                    #if DEBUG
                    await ShowNavigationErrorInDebug($"Unknown command '{commandParameter}'", nameof(ToolsViewModel));
                    #endif
                    break;
            }
        }
        else
        {
            #if DEBUG
            await ShowNavigationErrorInDebug(commandParameter, nameof(ToolsViewModel));
            #endif
        }
    }
    #endregion
}