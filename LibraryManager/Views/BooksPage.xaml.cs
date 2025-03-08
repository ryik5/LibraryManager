using LibraryManager.Extensions;
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
        // prevention of hung up switching between pages after it has been done selection of the row 
        BooksCollectionView.SelectedItems = null;

        base.OnDisappearing();

        if (BooksCollectionView != null)
            BooksCollectionView.SelectionChanged -= SelectableItemsView_OnSelectionChanged;

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
            bvs.SelectedBooks.ResetAndAddRange(e.CurrentSelection.Select((b => b as Book)));
        }
    }
}