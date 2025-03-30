using LibraryManager.Models;
using System.Windows.Input;

namespace LibraryManager.Controls;

public partial class EditBookView : ContentView
{
    public static readonly BindableProperty BookProperty =
        BindableProperty.Create(nameof(Book), typeof(Book), typeof(EditBookView), default,
            propertyChanged: OnBookChanged);

    public Book Book
    {
        get => (Book)GetValue(BookProperty);
        set => SetValue(BookProperty, value);
    }


    public static readonly BindableProperty NavigateCommandProperty = BindableProperty.Create(nameof(NavigateCommand),
        typeof(ICommand), typeof(EditBookView),
        defaultBindingMode: BindingMode.OneTime);

    public ICommand NavigateCommand
    {
        get => (ICommand)GetValue(NavigateCommandProperty);
        set => SetValue(NavigateCommandProperty, value);
    }


    public EditBookView()
    {
        InitializeComponent();
    }


    private static void OnBookChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (!(bindable is EditBookView editBookView))
            return;
        if (!(newValue is Book value))
            return;
        editBookView.Book = value;
    }

    // to indicate invalid Input
    private void OnInputIntChanged(object? sender, TextChangedEventArgs args)
    {
        bool isValid = int.TryParse(args.NewTextValue, out int value) && value is > 0 and <= 1000000000;

        if (sender is Entry entry)
            entry.Background = isValid ? Brush.Transparent : Brush.Pink;
    }

    private void OnInputYearChanged(object? sender, TextChangedEventArgs args)
    {
        bool isValid = int.TryParse(args.NewTextValue, out int value) && 600 < value && value <= DateTime.Now.Year;

        if (sender is Entry entry)
            entry.Background = isValid ? Brush.Transparent : Brush.Pink;
    }

    // to indicate invalid Input
    private void OnInputTextChanged(object? sender, TextChangedEventArgs args)
    {
        bool isValid = !string.IsNullOrEmpty(args.NewTextValue);

        if (sender is Entry entry)
            entry.Background = isValid ? Brush.Transparent : Brush.Pink;
    }
}