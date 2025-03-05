namespace LibraryManager.Extensions;

public static class PreferencesExtensions
{
    /// <summary>
    /// Safely sets a preference value, determining the correct type at runtime.
    /// </summary>
    /// <param name="key">The key of the preference.</param>
    /// <param name="value">The value to store in preferences.</param>
    public static void SetPreference(string key, object value)
    {
        switch (value)
        {
            case string strValue:
                Preferences.Set(key, strValue);
                break;
            case int intValue:
                Preferences.Set(key, intValue);
                break;
            case double doubleValue:
                Preferences.Set(key, doubleValue);
                break;
            case bool boolValue:
                Preferences.Set(key, boolValue);
                break;
            case long longValue:
                Preferences.Set(key, longValue);
                break;
            default:
                throw new InvalidOperationException($"Unsupported preference type for key '{key}'. Value must be a primitive type.");
        }
    }
}
