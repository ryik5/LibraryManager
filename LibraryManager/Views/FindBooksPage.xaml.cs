using LibraryManager.AbstractObjects;
using LibraryManager.ViewModels;

namespace LibraryManager.Views;

/// <author>YR 2025-02-09</author>
public partial class FindBooksPage : ContentPage
{
    public FindBooksPage()
    {
        InitializeComponent();
    }


    /// <summary>
    /// To ensure the BindingContext is bound correctly
    /// </summary>
    protected async override void OnAppearing()
    {
        base.OnAppearing();

        // Avoid assigning a new instance unnecessarily
        BindingContext ??= App.Services.GetService<FindBooksViewModel>();
        BooksCollectionView.BindingContext ??= App.Services.GetService<FindBooksViewModel>();
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