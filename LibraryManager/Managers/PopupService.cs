using LibraryManager.AbstractObjects;
using LibraryManager.Controls;
using LibraryManager.ViewModels;
using RGPopup.Maui.Extensions;

namespace LibraryManager.Models;

/// <author>YR 2025-04-27</author>
public class PopupService : IPopupService
{
    public async Task ShowPopup(PopUpViewModel popup)
    {
        var popUpView = new PopUpView() { BindingContext = popup };

        await Application.Current?.MainPage?.Navigation.PushPopupAsync(popUpView)!;
    }
}