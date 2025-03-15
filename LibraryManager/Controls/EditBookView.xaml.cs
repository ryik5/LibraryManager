using LibraryManager.Models;

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
}