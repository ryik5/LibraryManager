namespace LibraryManager.AbstractObjects;

public interface IFolderPicker
{
    Task<string> PickFolder();
}