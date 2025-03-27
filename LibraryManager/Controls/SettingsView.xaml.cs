namespace LibraryManager.Controls;

public partial class SettingsView : ContentView
{
    public SettingsView()
    {
        InitializeComponent();
    }

    // to indicate invalid FontSize
    private void OnTextFontSizeChanged(object? sender, TextChangedEventArgs args)
    {
        bool isValid = int.TryParse(args.NewTextValue, out int value) && value is >= 6 and <= 40;

        if (sender is Entry entry)
            entry.Background = isValid ? Brush.Transparent : Brush.Pink;
    }
}