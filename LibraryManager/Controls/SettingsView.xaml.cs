namespace LibraryManager.Controls;

public partial class SettingsView : ContentView
{
    public SettingsView()
    {
        InitializeComponent();
    }

    // to indicate invalid FontSize
    // TODO: from settings(add attached properties)
    private void OnTextFontSizeChanged(object? sender, TextChangedEventArgs args)
    {
        if (sender is Entry entry)
        {
            var isValid = int.TryParse(args.NewTextValue, out int value) && value is >= 6 and <= 40;
            entry.Background = isValid ? Brush.Transparent : Brush.Pink;
        }
    }
}