using LibraryManager.Models;
using LibraryManager.ViewModels;

namespace LibraryManager.Views;

public partial class BooksPage : ContentPage
{
    // Parameterless constructor required by .NET MAUI Shell navigation
    public BooksPage()
    {
        InitializeComponent();
        
        // Fetch the singleton instance of BooksViewModel
        BindingContext ??= App.Services.GetService<BooksViewModel>();
    }
    
    /// <summary>
    /// To ensure the BindingContext is bound correctly
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // Avoid assigning a new instance unnecessarily
        BindingContext ??= App.Services.GetService<BooksViewModel>();

        // Add event handlers
        BooksCollectionView.SelectionChanged += SelectableItemsView_OnSelectionChanged;
    }

    /// <summary>
    /// To ensure the BindingContext is cleared correctly
    /// </summary>
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if(BooksCollectionView !=null)
            BooksCollectionView.SelectionChanged-= SelectableItemsView_OnSelectionChanged;
        
        if (BindingContext != null)
            BindingContext = null;
    }

    
   /// <summary>
   /// Event handler for SelectionChanged event
   /// </summary>
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