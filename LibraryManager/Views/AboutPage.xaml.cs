using LibraryManager.ViewModels;

namespace LibraryManager.Views;

/// <author>YR 2025-02-09</author>
public partial class AboutPage : ContentPage
{
    public AboutPage()
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
        BindingContext ??= App.Services.GetService<AboutViewModel>();
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