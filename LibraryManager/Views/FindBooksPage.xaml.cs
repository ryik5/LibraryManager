using LibraryManager.ViewModels;

namespace LibraryManager.Views;

public partial class FindBooksPage : ContentPage
{
    public FindBooksPage()
    {
        InitializeComponent();
        
        // Fetch the singleton instance of FindBooksViewModel
        BindingContext ??= App.Services.GetService<FindBooksViewModel>();
    }
    
    
    /// <summary>
    /// To ensure the BindingContext is bound correctly
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // Ensure BindingContext is assigned only if not already set
        BindingContext ??= App.Services.GetService<FindBooksViewModel>();
    }

    /// <summary>
    /// To ensure the BindingContext is cleared correctly
    /// </summary>
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (BindingContext != null)
        {
            BindingContext = null;
        }
    }
}