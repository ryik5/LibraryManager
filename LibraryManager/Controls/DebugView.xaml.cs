namespace LibraryManager.Controls;

public partial class DebugView : ContentView
{
    public DebugView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty DebugTextViewProperty =
        BindableProperty.Create(nameof(DebugTextView), typeof(List<string>), typeof(DebugView), default,
            propertyChanged: OnDebugTextViewChanged);


    public List<string> DebugTextView
    {
        get => (List<string>)GetValue(DebugTextViewProperty);
        set => SetValue(DebugTextViewProperty, value);
    }


    private static void OnDebugTextViewChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (!(bindable is DebugView debugView))
            return;
        if (!(newValue is List<string> value))
            return;
        debugView.DebugTextView = value;
    }
}