using LibraryManager.Extensions;
using LibraryManager.Models;
using LibraryManager.ViewModels;

namespace LibraryManager.Controls;

public partial class FoundCollectionView : ContentView
{
    public FoundCollectionView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// To ensure the BindingContext is bound correctly
    /// </summary>
    public virtual void OnAppearing()
    {
        // Avoid assigning a new instance unnecessarily
        BindingContext ??= App.Services.GetService<FindBooksViewModel>();

        // Add event handlers
        BooksCollectionView.SelectionChanged += SelectableItemsView_OnSelectionChanged;
    }

    /// <summary>
    /// To ensure the BindingContext is cleared correctly
    /// </summary>
    public virtual void OnDisappearing()
    {
        // prevention of hung up switching between pages after it has been done selection of the row 
        BooksCollectionView.SelectedItems = null;

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
        if (BindingContext is FindBooksViewModel findBooksViewModel)
        {
            findBooksViewModel.SelectedBooks.ResetAndAddRange(e.CurrentSelection.Select(b => b as Book));
        }
    }
}