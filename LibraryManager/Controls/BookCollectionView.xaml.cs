using LibraryManager.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LibraryManager.Controls;

public partial class BookCollectionView : ContentView
{
    public static readonly BindableProperty SelectionChangedCommandProperty =
        BindableProperty.Create(nameof(SelectionChangedCommand),
            typeof(ICommand), typeof(BookCollectionView),
            defaultBindingMode: BindingMode.TwoWay);

    public ICommand SelectionChangedCommand
    {
        get => (ICommand)GetValue(SelectionChangedCommandProperty);
        set => SetValue(SelectionChangedCommandProperty, value);
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
}