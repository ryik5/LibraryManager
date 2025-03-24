using LibraryManager.ViewModels;

namespace LibraryManager.Controls;

public partial class StatusBarPanel : ContentView
{
    public StatusBarPanel()
    {
        InitializeComponent();
    }


    public static readonly BindableProperty StatusBarProperty =
        BindableProperty.Create(nameof(StatusBar), typeof(StatusBarViewModel), typeof(StatusBarPanel), default,
            propertyChanged: OnStatusBarChanged);

    public StatusBarViewModel StatusBar
    {
        get => (StatusBarViewModel)GetValue(StatusBarProperty);
        set => SetValue(StatusBarProperty, value);
    }

    private static void OnStatusBarChanged(BindableObject bindable, object oldvalue, object newValue)
    {
        if (!(bindable is StatusBarPanel statusBarPanel))
            return;
        if (!(newValue is StatusBarViewModel value))
            return;
        statusBarPanel.StatusBar = value;
    }
}