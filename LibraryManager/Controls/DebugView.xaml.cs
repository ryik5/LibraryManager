using LibraryManager.Models;

namespace LibraryManager.Controls;

/// <author>YR 2025-02-09</author>
public partial class DebugView : ContentView
{
    public DebugView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty DebugTextViewProperty =
        BindableProperty.Create(nameof(DebugTextView), typeof(List<IndexedString>), typeof(DebugView), default,
            propertyChanged: OnDebugTextViewChanged);


    public List<IndexedString> DebugTextView
    {
        get => (List<IndexedString>)GetValue(DebugTextViewProperty);
        set => SetValue(DebugTextViewProperty, value);
    }


    private static void OnDebugTextViewChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (!(bindable is DebugView debugView))
            return;
        if (!(newValue is List<IndexedString> value))
            return;
        debugView.DebugTextView = value;
    }
}