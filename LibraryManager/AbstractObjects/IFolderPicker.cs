namespace LibraryManager.AbstractObjects;

/// <author>YR 2025-05-09</author>
public interface IFolderPicker
{
    Task<string> PickFolder();
}