namespace LibraryManager.Models;

/// <summary>
/// Enumerates the possible button configurations for a message box.
/// </summary>
/// <author>YR 2025-02-17</author>
public enum EMessageBoxButtonsViewSelector
{
    /// <summary>Displays a single "Yes" button.</summary>
    Yes,
    /// <summary>Displays "Yes" and "No" buttons.</summary>
    YesNo,
    /// <summary>Displays "Yes" and "Cancel" buttons.</summary>
    YesCancel,
    /// <summary>Displays "Yes", "No", and "Cancel" buttons.</summary>
    YesNoCancel,
    /// <summary>Displays a single "OK" button.</summary>
    Ok,
    /// <summary>Displays "OK" and "No" buttons.</summary>
    OkNo,
    /// <summary>Displays "OK" and "Cancel" buttons.</summary>
    OkCancel,
    /// <summary>Displays "OK", "No", and "Cancel" buttons.</summary>
    OkNoCancel
}
