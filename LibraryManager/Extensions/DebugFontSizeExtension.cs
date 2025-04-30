using LibraryManager.ViewModels;

namespace LibraryManager.Extensions;

/// <summary>
/// Provides a value for the debug text font size binding.
/// </summary>
/// <author>YR 2025-04-28</author>
public class DebugFontSizeExtension : IMarkupExtension
{
    /// <summary>
    /// Provides a value for the debug text font size binding.
    /// </summary>
    /// <returns>The debug text's font size.</returns>
    public object ProvideValue(IServiceProvider serviceProvider)
    {
        var settings = new SettingsViewModel();
        return settings.Debug_TextFontSize;
    }
}