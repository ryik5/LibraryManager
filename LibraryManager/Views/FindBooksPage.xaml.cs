using LibraryManager.AbstractObjects;
using LibraryManager.ViewModels;

namespace LibraryManager.Views;

/// <author>YR 2025-02-09</author>
public partial class FindBooksPage : ContentPage
{
    public FindBooksPage()
    {
        InitializeComponent();

        BindingContext = App.Services.GetService<FindBooksViewModel>();
    }


    /// <summary>
    /// To ensure the BindingContext is bound correctly
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    /// <summary>
    /// To ensure the BindingContext is cleared correctly
    /// </summary>
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        /*BindingContext = null;*/
    }
}