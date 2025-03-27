namespace LibraryManager.AbstractObjects;

public interface IRefreshable
{
    Task RefreshControlsOnAppearing();
    Task RefreshControlsOnDisappearing();
}