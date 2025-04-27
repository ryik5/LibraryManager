using LibraryManager.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LibraryManager.Controls;

/// <author>YR 2025-02-09</author>
public partial class BookCollectionView : ContentView
{
    public static readonly BindableProperty NavigateCommandProperty = BindableProperty.Create(nameof(NavigateCommand),
        typeof(ICommand), typeof(ThreeButtonsPanel),
        defaultBindingMode: BindingMode.OneTime);

    public ICommand NavigateCommand
    {
        get => (ICommand)GetValue(NavigateCommandProperty);
        set => SetValue(NavigateCommandProperty, value);
    }

    public static readonly BindableProperty BookCollectionProperty =
        BindableProperty.Create(nameof(BookCollection), typeof(ObservableCollection<Book>), typeof(BookCollectionView),
            default,
            propertyChanged: OnBookCollectionChanged);

    private static void OnBookCollectionChanged(BindableObject bindable, object oldvalue, object newValue)
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


    public static readonly BindableProperty SelectedBooksProperty =
        BindableProperty.Create(nameof(SelectedBooks), typeof(ObservableCollection<object>), typeof(BookCollectionView),
            default,
            propertyChanged: OnSelectedBooksChanged);

    private static void OnSelectedBooksChanged(BindableObject bindable, object oldvalue, object newValue)
    {
        if (!(bindable is BookCollectionView bookCollectionView))
            return;
        if (!(newValue is ObservableCollection<object> value))
            return;
        bookCollectionView.SelectedBooks = value;
    }

    public ObservableCollection<object> SelectedBooks
    {
        get => (ObservableCollection<object>)GetValue(SelectedBooksProperty);
        set => SetValue(SelectedBooksProperty, value);
    }


    // Parameterless constructor required by .NET MAUI Shell navigation
    public BookCollectionView()
    {
        InitializeComponent();
    }


    // To handle Click and double-click
    private void Button_OnClicked(object? sender, EventArgs e)
    {
        if (_clickCount < 1)
        {
            var timeSpan = new TimeSpan(0, 0, 0, 0, 200); // time to double-click - 200ms

            Dispatcher.StartTimer(timeSpan, ClickHandle);
        }

        _clickCount++;
    }

    private bool ClickHandle()
    {
        NavigateCommand.Execute(_clickCount > 1 ? Constants.DOUBLECLICK : Constants.CLICK);

        _clickCount = 0;
        return false;
    }

    private static int _clickCount = 0;
}