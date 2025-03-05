using LibraryManager.ViewModels;

namespace LibraryManager.Views;

public partial class LibraryManagePage : ContentPage
{
    public LibraryManagePage()
    {
        InitializeComponent();
        
        // Fetch the singleton instance of LibraryViewModel
        BindingContext ??= App.Services.GetService<LibraryViewModel>();
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // Ensure BindingContext is assigned only if not already set
        if (BindingContext == null)
        {
            BindingContext = App.Services.GetService<LibraryViewModel>();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (BindingContext != null)
        {
            BindingContext = null;
        }
    }
}