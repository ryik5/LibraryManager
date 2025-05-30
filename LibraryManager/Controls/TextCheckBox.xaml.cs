namespace LibraryManager.Controls;

/// <author>YR 2025-02-09</author>
public partial class TextCheckBox : ContentView
{
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(TextCheckBox), default,
            propertyChanged: OnTextChanged);

    public static readonly BindableProperty IsCheckedProperty =
        BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(TextCheckBox), default,
            propertyChanged: OnIsCheckedChanged);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public bool IsChecked
    {
        get => (bool)GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    public TextCheckBox()
    {
        InitializeComponent();
    }

    private static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (!(bindable is TextCheckBox textCheckBox))
            return;
        if (!(newValue is string value))
            return;
        textCheckBox.Text = value;
    }

    private static void OnIsCheckedChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (!(bindable is TextCheckBox textCheckBox))
            return;
        if (!(newValue is bool value))
            return;
        textCheckBox.IsChecked = value;
    }

    private void OnTextTapped(object sender, TappedEventArgs e) => IsChecked = !IsChecked;

    private void OnCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox checkBox)
            IsChecked = checkBox.IsChecked;
    }
}