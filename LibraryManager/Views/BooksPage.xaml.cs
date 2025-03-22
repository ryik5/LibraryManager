using LibraryManager.AbstractObjects;
using LibraryManager.ViewModels;

namespace LibraryManager.Views;

public partial class BooksPage : ContentPage
{
    // Parameterless constructor required by .NET MAUI Shell navigation
    public BooksPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// To ensure the BindingContext is bound correctly
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Avoid assigning a new instance unnecessarily
        BindingContext ??= App.Services.GetService<BooksViewModel>();
        BooksCollectionView.BindingContext ??= App.Services.GetService<BooksViewModel>();

        if (BindingContext is IRefreshable refresher)
        {
            refresher.RefreshControlsOnAppearing();
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