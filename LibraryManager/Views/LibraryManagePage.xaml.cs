using LibraryManager.ViewModels;

namespace LibraryManager.Views;

public partial class LibraryManagePage : ContentPage
{
    public LibraryManagePage()
    {
        InitializeComponent();
        
        // Manually resolve or assign the ViewModel when using DI
        BindingContext = App.Services.GetService<LibraryViewModel>();
    }
}