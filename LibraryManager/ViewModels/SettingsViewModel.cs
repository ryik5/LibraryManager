using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LibraryManager.AbstractObjects;
using LibraryManager.Extensions;
using LibraryManager.Models;
using LibraryManager.Platforms.MacCatalyst;

namespace LibraryManager.ViewModels;

/// <summary>
/// View model for managing application settings.
/// </summary>
/// <author>YR 2025-02-14</author>
public sealed partial class SettingsViewModel : AbstractBindableModel
{
    /// <summary>
    /// Initializes a new instance of <see cref="SettingsViewModel"/>.
    /// </summary>
    public SettingsViewModel()
    {
        NavigateCommand = new AsyncRelayCommand<string>(PerformAction);

        SearchFields = Enum.GetValues<EBibliographicKindInformation>().ToArray();
        Booleans = new[] { true, false };
        SortingDirections = new[] { Constants.SORTING_ASCENDING, Constants.SORTING_DESCENDING };
        BookProperties = new Library().GetBookProperties();
        LoadAllSettings().ConfigureAwait(false);
        _folderPicker = new FolderPicker();
    }


    #region Dictionaries
    /// <summary>
    /// Gets an array of search fields available for the FindBooks page.
    /// </summary>
    [ObservableProperty] private EBibliographicKindInformation[] _searchFields;

    /// <summary>
    /// Gets an array of boolean values representing the state of various settings.
    /// </summary>
    [ObservableProperty] private bool[] _booleans;

    /// <summary>
    /// Sort directions - ASCENDING, DESCENDING
    /// </summary>
    [ObservableProperty] private string[] _sortingDirections;

    /// <summary>
    /// Gets an array of boolean values representing the state of various settings.
    /// </summary>
    [ObservableProperty] private string[] _bookProperties;
    #endregion

    #region Public Properties
    /// <summary>
    /// Gets or sets the font size of the message box.
    /// </summary>
    [ObservableProperty] private double _messageBox_FontSize;

    /// <summary>
    /// Gets or sets the search field type.
    /// </summary>
    [ObservableProperty] private EBibliographicKindInformation _searchField = EBibliographicKindInformation.All;

    /// <summary>
    /// Gets or sets a value indicating whether to search on the fly.
    /// </summary>
    [ObservableProperty] private bool _searchOnFly;

    /// <summary>
    /// Gets or sets the font size of the debug text.
    /// </summary>
    [ObservableProperty] private double _debug_TextFontSize;

    /// <summary>
    /// Gets or sets the first sort book property.
    /// </summary>
    /// <remarks>
    /// If set to the same value as SecondSortBookProperty or ThirdSortBookProperty, 
    /// the other property will be reset to Book.None.
    /// </remarks>
    public string FirstSortBookProperty
    {
        get => _firstSortBookProperty;
        set
        {
            if (SetProperty(ref _firstSortBookProperty, value) && value != nameof(Book.None))
            {
                if (value == SecondSortBookProperty)
                {
                    SecondSortBookProperty = nameof(Book.None);
                }
                else if (value == ThirdSortBookProperty)
                {
                    ThirdSortBookProperty = nameof(Book.None);
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the first sort property is sorted in descending order.
    /// </summary>
    public bool FirstSortProperty_ByDescend
    {
        get => _firstSortPropertyByDescend;
        set
        {
            SetProperty(ref _firstSortPropertyByDescend, value);
            SetStringProperty(value, ref _firstSortProperty_SortingDirection, FirstSortProperty_SortingDirection);
        }
    }

    /// <summary>
    /// Gets or sets the sorting direction of the first sort property.
    /// </summary>
    public string FirstSortProperty_SortingDirection
    {
        get => _firstSortProperty_SortingDirection;
        set
        {
            SetProperty(ref _firstSortProperty_SortingDirection, value);
            SetBooleanProperty(value, ref _firstSortPropertyByDescend, FirstSortProperty_ByDescend);
        }
    }

    /// <summary>
    /// Gets or sets the second sort book property.
    /// </summary>
    /// <remarks>
    /// If set to the same value as FirstSortBookProperty or ThirdSortBookProperty, 
    /// the other property will be reset to Book.None.
    /// </remarks>
    public string SecondSortBookProperty
    {
        get => _secondSortBookProperty;
        set
        {
            if (SetProperty(ref _secondSortBookProperty, value) && value != nameof(Book.None))
            {
                if (value == FirstSortBookProperty)
                    FirstSortBookProperty = nameof(Book.None);
                else if (value == ThirdSortBookProperty)
                    ThirdSortBookProperty = nameof(Book.None);
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the second sort property is sorted in descending order.
    /// </summary>
    public bool SecondSortProperty_ByDescend
    {
        get => _secondSortPropertyByDescend;
        set
        {
            SetProperty(ref _secondSortPropertyByDescend, value);
            SetStringProperty(value, ref _secondSortProperty_SortingDirection, SecondSortProperty_SortingDirection);
        }
    }

    /// <summary>
    /// Gets or sets the sorting direction of the second sort property.
    /// </summary>
    public string SecondSortProperty_SortingDirection
    {
        get => _secondSortProperty_SortingDirection;
        set
        {
            SetProperty(ref _secondSortProperty_SortingDirection, value);
            SetBooleanProperty(value, ref _secondSortPropertyByDescend, SecondSortProperty_ByDescend);
        }
    }

    /// <summary>
    /// Gets or sets the third sort book property.
    /// </summary>
    /// <remarks>
    /// If set to the same value as FirstSortBookProperty or SecondSortBookProperty, 
    /// the other property will be reset to Book.None.
    /// </remarks>
    public string ThirdSortBookProperty
    {
        get => _thirdSortBookProperty;
        set
        {
            if (SetProperty(ref _thirdSortBookProperty, value) && value != nameof(Book.None))
            {
                if (value == FirstSortBookProperty)
                    FirstSortBookProperty = nameof(Book.None);
                else if (value == SecondSortBookProperty)
                    SecondSortBookProperty = nameof(Book.None);
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the third sort property is sorted in descending order.
    /// </summary>
    public bool ThirdSortProperty_ByDescend
    {
        get => _thirdSortPropertyByDescend;
        set
        {
            SetProperty(ref _thirdSortPropertyByDescend, value);
            SetStringProperty(value, ref _thirdSortProperty_SortingDirection, ThirdSortProperty_SortingDirection);
        }
    }

    /// <summary>
    /// Gets or sets the sorting direction of the third sort property.
    /// </summary>
    public string ThirdSortProperty_SortingDirection
    {
        get => _thirdSortProperty_SortingDirection;
        set
        {
            SetProperty(ref _thirdSortProperty_SortingDirection, value);
            SetBooleanProperty(value, ref _thirdSortPropertyByDescend, ThirdSortProperty_ByDescend);
        }
    }

    /// <summary>
    /// Gets or sets the maximum content length for a book.
    /// </summary>
    public long Book_MaxContentLength
    {
        get => _bookMaxContentLength;
        set
        {
            Book_MaxContentLength_ToolTip = string.Empty;
            if (value < 0)
            {
                Book_MaxContentLength = 0;
                Book_MaxContentLength_ToolTip = "0";
            }
            else if (SetProperty(ref _bookMaxContentLength, value))
            {
                Book_MaxContentLength_ToolTip = GetFileSizeTooltip(value);
            }
        }
    }

    /// <summary>
    /// Gets or sets the tooltip for the maximum content length for a book.
    /// </summary>
    [ObservableProperty] private string _book_MaxContentLength_ToolTip;

    /// <summary>
    /// Property to get/set the library home folder path
    /// </summary>
    [ObservableProperty] private string _libraryHomeFolder;

    /// <summary>
    /// Read-only property to get the label for the library home folder
    /// </summary>
    public string LabelLibraryHomeFolder => Constants.LIBRARY_HOME_FOLDER;
    #endregion

    #region Public methods
    public async Task PerformAction(string? commandParameter)
    {
        switch (commandParameter)
        {
            case Constants.LIBRARY_HOME_FOLDER:
                await Task.Delay(10);
                LibraryHomeFolder = await PickFolderUpTask();
                break;
            case Constants.SAVE:
                await SaveSettings();
                WeakReferenceMessenger.Default.Send(new StatusMessage()
                {
                    InfoKind = EInfoKind.CurrentInfo,
                    Message = $"Saved current setting values"
                });
                break;
            case Constants.CANCEL:
                await LoadAllSettings();
                WeakReferenceMessenger.Default.Send(new StatusMessage()
                {
                    InfoKind = EInfoKind.CurrentInfo,
                    Message = $"Restored previous setting values"
                });
                break;
            case Constants.RESET:
                await ResetAllSettings();
                WeakReferenceMessenger.Default.Send(new StatusMessage()
                {
                    InfoKind = EInfoKind.CurrentInfo,
                    Message = $"Restored default setting values"
                });
                break;
        }
    }

    /// <summary>
    /// Loads all settings into the view model.
    /// </summary>
    public Task LoadAllSettings()
    {
        MessageBox_FontSize = Preferences.Get(nameof(MessageBox_FontSize),
            (double)DefaultSettings[nameof(MessageBox_FontSize)]);

        SearchField =
            (EBibliographicKindInformation)Preferences.Get(nameof(SearchField),
                (int)DefaultSettings[nameof(SearchField)]);

        SearchOnFly = Preferences.Get(nameof(SearchOnFly), (bool)DefaultSettings[nameof(SearchOnFly)]);
        Debug_TextFontSize =
            Preferences.Get(nameof(Debug_TextFontSize), (double)DefaultSettings[nameof(Debug_TextFontSize)]);

        FirstSortBookProperty = Preferences.Get(nameof(FirstSortBookProperty),
            (string)DefaultSettings[nameof(FirstSortBookProperty)]);
        FirstSortProperty_ByDescend = Preferences.Get(nameof(FirstSortProperty_ByDescend),
            (bool)DefaultSettings[nameof(FirstSortProperty_ByDescend)]);
        SecondSortBookProperty = Preferences.Get(nameof(SecondSortBookProperty),
            (string)DefaultSettings[nameof(SecondSortBookProperty)]);
        SecondSortProperty_ByDescend = Preferences.Get(nameof(SecondSortProperty_ByDescend),
            (bool)DefaultSettings[nameof(SecondSortProperty_ByDescend)]);
        ThirdSortBookProperty = Preferences.Get(nameof(ThirdSortBookProperty),
            (string)DefaultSettings[nameof(ThirdSortBookProperty)]);
        ThirdSortProperty_ByDescend = Preferences.Get(nameof(ThirdSortProperty_ByDescend),
            (bool)DefaultSettings[nameof(ThirdSortProperty_ByDescend)]);

        Book_MaxContentLength = Preferences.Get(nameof(Book_MaxContentLength),
            (long)DefaultSettings[nameof(Book_MaxContentLength)]);
        Book_MaxContentLength_ToolTip = Preferences.Get(nameof(Book_MaxContentLength_ToolTip),
            (string)DefaultSettings[nameof(Book_MaxContentLength_ToolTip)]);

        LibraryHomeFolder = Preferences.Get(nameof(LibraryHomeFolder),
            (string)DefaultSettings[nameof(LibraryHomeFolder)]);

        return Task.CompletedTask;
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Resets all application settings to their default values.
    /// </summary>
    private async Task ResetAllSettings()
    {
        await ResetPreferencesExtensions();
        await LoadAllSettings(); // Refresh view model properties
    }

    /// <summary>
    /// Saves all view model values to preferences.
    /// </summary>
    private Task SaveSettings()
    {
        Preferences.Set(nameof(MessageBox_FontSize), MessageBox_FontSize);
        Preferences.Set(nameof(SearchField), (int)SearchField);
        Preferences.Set(nameof(SearchOnFly), SearchOnFly);
        Preferences.Set(nameof(Debug_TextFontSize), Debug_TextFontSize);

        Preferences.Set(nameof(FirstSortBookProperty), FirstSortBookProperty);
        Preferences.Set(nameof(FirstSortProperty_ByDescend), FirstSortProperty_ByDescend);
        Preferences.Set(nameof(SecondSortBookProperty), SecondSortBookProperty);
        Preferences.Set(nameof(SecondSortProperty_ByDescend), SecondSortProperty_ByDescend);
        Preferences.Set(nameof(ThirdSortBookProperty), ThirdSortBookProperty);
        Preferences.Set(nameof(ThirdSortProperty_ByDescend), ThirdSortProperty_ByDescend);

        Preferences.Set(nameof(Book_MaxContentLength), Book_MaxContentLength);
        Preferences.Set(nameof(Book_MaxContentLength_ToolTip), Book_MaxContentLength_ToolTip);
        Preferences.Set(nameof(LibraryHomeFolder), LibraryHomeFolder);

        return Task.CompletedTask;
    }

    private void SetStringProperty(bool key, ref string _propertyValue, string PropertyName)
    {
        switch (key)
        {
            case true:
                _propertyValue = Constants.SORTING_DESCENDING;
                break;
            default:
                _propertyValue = Constants.SORTING_ASCENDING;
                break;
        }
    }

    private void SetBooleanProperty(string key, ref bool _propertyValue, bool PropertyName)
    {
        switch (key)
        {
            case Constants.SORTING_DESCENDING:
                _propertyValue = true;
                break;
            default:
                _propertyValue = false;
                break;
        }
    }

    private Task ResetPreferencesExtensions()
    {
        foreach (var setting in DefaultSettings)
        {
            PreferencesExtensions.SetPreference(setting.Key, setting.Value); // Use static method
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Returns a tooltip text describing the file size.
    /// </summary>
    /// <param name="fileSize">The size of the file in bytes.</param>
    /// <returns>A tooltip text describing the file size.</returns>
    private string GetFileSizeTooltip(long fileSize)
    {
        var tooltip = string.Empty;

        if (1_000_000_000 < fileSize)
        {
            var fileSizeInGigabytes = fileSize / 1_000_000_000;
            tooltip = $"The set file size enormous ({fileSizeInGigabytes} GB). This may cause storage issues.";
        }
        else
        {
            // Convert the file size to a human-readable format
            ConvertToHumanReadableFileSize(fileSize, EFileLengthUnit.Byte, out var fileSizeInUnits, out var unit);

            tooltip = $"The loaded file size can be maximum as {fileSizeInUnits} {unit}";
        }

        return tooltip;
    }

    /// <summary>
    /// Converts a file size in bytes to a human-readable format.
    /// </summary>
    /// <param name="number">The file size in bytes.</param>
    /// <param name="startLength">The starting unit of measurement (e.g. Byte, KB, MB, GB).</param>
    /// <param name="result">The converted file size.</param>
    /// <param name="length">The unit of measurement for the converted file size.</param>
    private void ConvertToHumanReadableFileSize(long number, EFileLengthUnit startLength, out long result,
        out EFileLengthUnit length)
    {
        while (true)
        {
            result = number;
            length = startLength;
            if (1024 < number && (int)startLength < (int)EFileLengthUnit.GB)
            {
                length = startLength.Next();
                result = number / 1024;
                number = result;
                startLength = length;
                continue;
            }

            break;
        }
    }


    /// <summary>
    /// Gets the path to the current user Document Directory on the device.
    /// </summary
    private async Task<string> PickFolderUpTask()
    {
        return await _folderPicker.PickFolder();
    }
    #endregion

    #region Private fields
    private long _bookMaxContentLength;
    private double _messageBoxFontSize;
    private string _firstSortBookProperty;
    private bool _firstSortPropertyByDescend;
    private string _firstSortProperty_SortingDirection;
    private string _secondSortBookProperty;
    private bool _secondSortPropertyByDescend;
    private string _secondSortProperty_SortingDirection;
    private string _thirdSortBookProperty;
    private bool _thirdSortPropertyByDescend;
    private string _thirdSortProperty_SortingDirection;
    private readonly IFolderPicker _folderPicker;

    private readonly Dictionary<string, object> DefaultSettings = new() // Default values for preferences
    {
        { nameof(MessageBox_FontSize), 18.0 },
        { nameof(SearchField), EBibliographicKindInformation.All },
        { nameof(SearchOnFly), false },
        { nameof(Debug_TextFontSize), 16.0 },

        { nameof(FirstSortBookProperty), nameof(Book.Year) },
        { nameof(FirstSortProperty_ByDescend), true },
        { nameof(SecondSortBookProperty), nameof(Book.Author) },
        { nameof(SecondSortProperty_ByDescend), false },
        { nameof(ThirdSortBookProperty), nameof(Book.Title) },
        { nameof(ThirdSortProperty_ByDescend), false },
        { nameof(Book_MaxContentLength), 0L },
        { nameof(Book_MaxContentLength_ToolTip), string.Empty },
        { nameof(LibraryHomeFolder), string.Empty },
    };
    #endregion
}