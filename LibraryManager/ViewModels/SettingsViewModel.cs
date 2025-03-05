using System.ComponentModel;
using System.Runtime.CompilerServices;
using LibraryManager.Extensions;
using LibraryManager.Models;

namespace LibraryManager.ViewModels;

/// <summary>
/// View model for managing application settings.
/// </summary>
/// <author>YR 2025-02-14</author>
public class SettingsViewModel: INotifyPropertyChanged
{
    private readonly Dictionary<string, object> DefaultSettings = new() // Default values for preferences
    {
        { nameof(MessageBox_FontSize), 12.0 },
       // { nameof(FindBooks_LastSearchField), string.Empty },
        { nameof(SearchOnFly), false },
        { nameof(Debug_TextFontSize), 14.0 },
        
        { nameof(FirstSortBookProperty), string.Empty },
        { nameof(FirstSortProperty_ByDescend), false },
        { nameof(SecondSortBookProperty), string.Empty },
        { nameof(SecondSortProperty_ByDescend), false },
        { nameof(ThirdSortBookProperty), string.Empty },
        { nameof(ThirdSortProperty_ByDescend), false },
        
        { nameof(Book_MaxContentLength), 0L }
    };

    /// <summary>
    /// Initializes a new instance of <see cref="SettingsViewModel"/>.
    /// </summary>
    public SettingsViewModel()
    {
        LoadAllSettings();
    }

    #region Reset, Load, and Save Settings

    /// <summary>
    /// Resets all application settings to their default values.
    /// </summary>
    public void ResetAllSettings()
    {
        foreach (var setting in DefaultSettings)
        {
            PreferencesExtensions.SetPreference(setting.Key, setting.Value); // Use static method
        }
        LoadAllSettings(); // Refresh view model properties
    }

    /// <summary>
    /// Loads all settings into the view model.
    /// </summary>
    public void LoadAllSettings()
    {
        MessageBox_FontSize = Preferences.Get(nameof(MessageBox_FontSize), (double)DefaultSettings[nameof(MessageBox_FontSize)]);
       // SearchField = Preferences.Get(nameof(FindBooks_LastSearchField), (string)DefaultSettings[nameof(FindBooks_LastSearchField)]);
        SearchOnFly = Preferences.Get(nameof(SearchOnFly), (bool)DefaultSettings[nameof(SearchOnFly)]);
        Debug_TextFontSize = Preferences.Get(nameof(Debug_TextFontSize), (double)DefaultSettings[nameof(Debug_TextFontSize)]);

        FirstSortBookProperty = Preferences.Get(nameof(FirstSortBookProperty), (string)DefaultSettings[nameof(FirstSortBookProperty)]);
        FirstSortProperty_ByDescend = Preferences.Get(nameof(FirstSortProperty_ByDescend), (bool)DefaultSettings[nameof(FirstSortProperty_ByDescend)]);
        SecondSortBookProperty = Preferences.Get(nameof(SecondSortBookProperty), (string)DefaultSettings[nameof(SecondSortBookProperty)]);
        SecondSortProperty_ByDescend = Preferences.Get(nameof(SecondSortProperty_ByDescend), (bool)DefaultSettings[nameof(SecondSortProperty_ByDescend)]);
        ThirdSortBookProperty = Preferences.Get(nameof(ThirdSortBookProperty), (string)DefaultSettings[nameof(ThirdSortBookProperty)]);
        ThirdSortProperty_ByDescend = Preferences.Get(nameof(ThirdSortProperty_ByDescend), (bool)DefaultSettings[nameof(ThirdSortProperty_ByDescend)]);

        Book_MaxContentLength = Preferences.Get(nameof(Book_MaxContentLength), (long)DefaultSettings[nameof(Book_MaxContentLength)]);
    }

    /// <summary>
    /// Saves all view model values to preferences.
    /// </summary>
    public void SaveSettings()
    {
        Preferences.Set(nameof(MessageBox_FontSize), MessageBox_FontSize);
       // Preferences.Set(nameof(FindBooks_LastSearchField), SearchField);
        Preferences.Set(nameof(SearchOnFly), SearchOnFly);
        Preferences.Set(nameof(Debug_TextFontSize), Debug_TextFontSize);

        Preferences.Set(nameof(FirstSortBookProperty), FirstSortBookProperty);
        Preferences.Set(nameof(FirstSortProperty_ByDescend), FirstSortProperty_ByDescend);
        Preferences.Set(nameof(SecondSortBookProperty), SecondSortBookProperty);
        Preferences.Set(nameof(SecondSortProperty_ByDescend), SecondSortProperty_ByDescend);
        Preferences.Set(nameof(ThirdSortBookProperty), ThirdSortBookProperty);
        Preferences.Set(nameof(ThirdSortProperty_ByDescend), ThirdSortProperty_ByDescend);

        Preferences.Set(nameof(Book_MaxContentLength), Book_MaxContentLength);
    }

    #endregion

    #region Properties for Binding

    // Example ViewModel Properties
    private double _messageBoxFontSize;
    public double MessageBox_FontSize
    {
        get => _messageBoxFontSize;
        set => SetProperty(ref _messageBoxFontSize, value);
    }

    private string _searchField;
    public string SearchField
    {
        get => _searchField;
        set => SetProperty(ref _searchField, value);
    }

    private bool _searchOnFly;
    public bool SearchOnFly
    {
        get => _searchOnFly;
        set => SetProperty(ref _searchOnFly, value);
    }

    private double _debugTextFontSize;
    public double Debug_TextFontSize
    {
        get => _debugTextFontSize;
        set => SetProperty(ref _debugTextFontSize, value);
    }

    private string _firstSortBookProperty;
    public string FirstSortBookProperty
    {
        get => _firstSortBookProperty;
        set => SetProperty(ref _firstSortBookProperty, value);
    }

    private bool _firstSortPropertyByDescend;
    public bool FirstSortProperty_ByDescend
    {
        get => _firstSortPropertyByDescend;
        set => SetProperty(ref _firstSortPropertyByDescend, value);
    }

    private string _secondSortBookProperty;
    public string SecondSortBookProperty
    {
        get => _secondSortBookProperty;
        set => SetProperty(ref _secondSortBookProperty, value);
    }

    private bool _secondSortPropertyByDescend;
    public bool SecondSortProperty_ByDescend
    {
        get => _secondSortPropertyByDescend;
        set => SetProperty(ref _secondSortPropertyByDescend, value);
    }

    private string _thirdSortBookProperty;
    public string ThirdSortBookProperty
    {
        get => _thirdSortBookProperty;
        set => SetProperty(ref _thirdSortBookProperty, value);
    }

    private bool _thirdSortPropertyByDescend;
    public bool ThirdSortProperty_ByDescend
    {
        get => _thirdSortPropertyByDescend;
        set => SetProperty(ref _thirdSortPropertyByDescend, value);
    }

    private long _bookMaxContentLength;
    public long Book_MaxContentLength
    {
        get => _bookMaxContentLength;
        set => SetProperty(ref _bookMaxContentLength, value);
    }

    #endregion

    #region Private Helper Methods

    private bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return false;

        backingStore = value;
        RaisePropertyChanged(propertyName);
        return true;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}
