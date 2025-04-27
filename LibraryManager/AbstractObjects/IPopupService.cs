using LibraryManager.ViewModels;

namespace LibraryManager.AbstractObjects;

/// <author>YR 2025-04-27</author>
public interface IPopupService
{
    Task ShowPopup(PopUpViewModel popup);
}