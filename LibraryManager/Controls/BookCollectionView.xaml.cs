using LibraryManager.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LibraryManager.Controls;

public partial class BookCollectionView : ContentView
{
    public static readonly BindableProperty SelectionInBookCollectionChangedCommandProperty =
        BindableProperty.Create(nameof(SelectionInBookCollectionChangedCommand),
            typeof(ICommand), typeof(BookCollectionView),
            defaultBindingMode: BindingMode.TwoWay);

    public ICommand SelectionInBookCollectionChangedCommand
    {
        get => (ICommand)GetValue(SelectionInBookCollectionChangedCommandProperty);
        set => SetValue(SelectionInBookCollectionChangedCommandProperty, value);
    }


    public static readonly BindableProperty BookCollectionProperty =
        BindableProperty.Create(nameof(BookCollection), typeof(ObservableCollection<Book>), typeof(BookCollectionView),
            default,
            propertyChanged: OnBookChanged);

    private static void OnBookChanged(BindableObject bindable, object oldvalue, object newValue)
    {
        if (!(bindable is BookCollectionView bookCollectionView))
            return;
        if (!(newValue is ObservableCollection<Book> value))
            return;
        bookCollectionView.BookCollection = value;
    }

    public ObservableCollection<Book> BookCollection
    {
        get => (ObservableCollection<Book>)GetValue(BookCollectionProperty);
        set => SetValue(BookCollectionProperty, value);
    }


    // Parameterless constructor required by .NET MAUI Shell navigation
    public BookCollectionView()
    {
        InitializeComponent();
    }

    private void OnCollectionViewSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is CollectionView cv)
        { 
            // TODO : fix reselection
        }
    }
}