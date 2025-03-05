using LibraryManager.ViewModels;

namespace LibraryManager.Views;

public partial class LibraryManagePage : ContentPage
{
    public LibraryManagePage()
    {
        InitializeComponent();
        
        // Manually resolve or assign the ViewModel when using DI
        
        BindingContext = App.Services.GetService<LibraryViewModel>();
       /*BindingContext = App.Services.GetService<LibraryViewModel>();*/
    }

    /*public LibraryManagePage(LibraryViewModel viewModel)
    {
        InitializeComponent();
        
        BindingContext = App.Services.GetService<LibraryViewModel>();
        // Set the ViewModel as the BindingContext (via DI)
      //  BindingContext = viewModel;
    }*/
}