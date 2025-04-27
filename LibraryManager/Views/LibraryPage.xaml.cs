using LibraryManager.AbstractObjects;
using LibraryManager.ViewModels;

namespace LibraryManager.Views;

/// <author>YR 2025-02-09</author>
public partial class LibraryPage : ContentPage
{
    // Parameterless constructor required by .NET MAUI Shell navigation
    public LibraryPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// To ensure the BindingContext is bound correctly
    /// </summary>
    protected async override void OnAppearing()
    {
        base.OnAppearing();

        // Ensure BindingContext is assigned only if not already set
        BindingContext ??= App.Services.GetService<LibraryViewModel>();
        if (BindingContext is IRefreshable refresher)
        {
            await refresher.RefreshControlsOnAppearing();
        }
    }

    /// <summary>
    /// To ensure the BindingContext is cleared correctly
    /// </summary>
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        BindingContext = null;
    }
}