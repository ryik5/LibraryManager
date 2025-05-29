namespace LibraryManager.AbstractObjects;

/// <summary>
/// Enumerates the possible button configurations for a message box.
/// </summary>
/// <author>YR 2025-02-17</author>
public enum EMessageBoxButtons
{
    /// <summary>Displays "Yes" button.</summary>
    Yes,
    /// <summary>Displays "OK" button.</summary>
    Ok,
    /// <summary>Displays "No" button.</summary>
    No,
    /// <summary>Displays "Cancel" button.</summary>
    Cancel,
    /// <summary>Displays "Yes" and "No" buttons.</summary>
    YesNo,
    /// <summary>Displays "Yes" and "Cancel" buttons.</summary>
    YesCancel,
    /// <summary>Displays "Yes", "No", and "Cancel" buttons.</summary>
    YesNoCancel,
    /// <summary>Displays "OK" and "No" buttons.</summary>
    OkNo,
    /// <summary>Displays "OK" and "Cancel" buttons.</summary>
    OkCancel,
    /// <summary>Displays "OK", "No", and "Cancel" buttons.</summary>
    OkNoCancel,
}
