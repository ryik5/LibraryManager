namespace LibraryManager.AbstractObjects;

public interface IRefreshable
{
    void RefreshControlsOnAppearing();
    Task RefreshControlsOnDisappearing();
}