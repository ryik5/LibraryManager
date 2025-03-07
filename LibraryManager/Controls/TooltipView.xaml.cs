using Microsoft.Maui.Controls;

namespace LibraryManager.Views;

public partial class TooltipView : ContentView
{
    public TooltipView()
    {
        InitializeComponent();
    }

    // Bindable property for Tooltip content
    public static readonly BindableProperty FieldNameProperty = BindableProperty.Create(
        nameof(FieldName), typeof(string), typeof(TooltipView), string.Empty,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (TooltipView)bindable;
            control.FieldNameLabel.Text = (string)newValue; // Update field name
        });
    
    public static readonly BindableProperty FieldValueProperty = BindableProperty.Create(
        nameof(FieldValue), typeof(string), typeof(TooltipView), string.Empty,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (TooltipView)bindable;
            control.FieldValueLabel.Text = (string)newValue; // Update field value
        });

    public string FieldName
    {
        get => (string)GetValue(FieldNameProperty);
        set => SetValue(FieldNameProperty, value);
    }

    public string FieldValue
    {
        get => (string)GetValue(FieldValueProperty);
        set => SetValue(FieldValueProperty, value);
    }
}
