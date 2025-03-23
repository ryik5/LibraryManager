namespace LibraryManager.Controls;

public partial class StatusBarPanel : ContentView
{
    public StatusBarPanel()
    {
        InitializeComponent();
    }
    
    public static readonly BindableProperty CurrentInfoProperty =
        BindableProperty.Create(nameof(CurrentInfo), typeof(string), typeof(StatusBarPanel), default,
            propertyChanged: OnCurrentInfoChanged);
    
    public string CurrentInfo
    {
        get => (string)GetValue(CurrentInfoProperty);
        set => SetValue(CurrentInfoProperty, value);
    }
    
    private static void OnCurrentInfoChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (!(bindable is StatusBarPanel statusBarPanel))
            return;
        if (!(newValue is string value))
            return;
        statusBarPanel.CurrentInfo = value;
    }

}