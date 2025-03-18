using LibraryManager.ViewModels;

namespace LibraryManager.Views;

public partial class FindBooksPage : ContentPage
{
    public FindBooksPage()
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
        BindingContext ??= App.Services.GetService<FindBooksViewModel>();
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