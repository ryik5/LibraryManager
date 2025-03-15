using LibraryManager.ViewModels;

namespace LibraryManager.Views;

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
    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // Ensure BindingContext is assigned only if not already set
        BindingContext ??= App.Services.GetService<LibraryViewModel>();
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