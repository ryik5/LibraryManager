using LibraryManager.Extensions;
using LibraryManager.Models;
using LibraryManager.ViewModels;

namespace LibraryManager.Views;

public partial class BooksManagePage : ContentPage
{
    // Parameterless constructor required by .NET MAUI Shell navigation
    public BooksManagePage()
    {
        InitializeComponent();
        
        // Fetch the singleton instance of BooksViewModel
        BindingContext ??= App.Services.GetService<BooksViewModel>();
    }
    

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // Avoid assigning a new instance unnecessarily
        if (BindingContext == null)
        {
            BindingContext = App.Services.GetService<BooksViewModel>();
        }

        // Add event handlers
        BooksCollectionView.SelectionChanged += SelectableItemsView_OnSelectionChanged;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        BooksCollectionView.SelectionChanged-= SelectableItemsView_OnSelectionChanged;
        
        if (BindingContext != null)
        {
            BindingContext = null;
        }
    }

    private void SelectableItemsView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (BindingContext is BooksViewModel bvs)
        {
            bvs.SelectedBooks.Clear();
            foreach (Book item in e.CurrentSelection)
            {
                #if DEBUG
                Console.WriteLine($"Selected: {item?.Title}");
                #endif
                bvs.SelectedBooks.Add(item);
            }
        }
    }
}