using System.Windows.Input;

namespace LibraryManager.Controls;

/// <author>YR 2025-02-09</author>
public partial class ThreeButtonsPanel : ContentView
{
    public ThreeButtonsPanel()
    {
        InitializeComponent();
    }


    public static readonly BindableProperty NavigateCommandProperty = BindableProperty.Create(nameof(NavigateCommand),
        typeof(ICommand), typeof(ThreeButtonsPanel),
        defaultBindingMode: BindingMode.OneTime);

    public ICommand NavigateCommand
    {
        get => (ICommand)GetValue(NavigateCommandProperty);
        set => SetValue(NavigateCommandProperty, value);
    }


    public static readonly BindableProperty OkTextProperty = BindableProperty.Create(nameof(OkText), typeof(string),
        typeof(ThreeButtonsPanel), defaultBindingMode: BindingMode.OneTime);

    public string OkText
    {
        get => (string)GetValue(OkTextProperty);
        set => SetValue(OkTextProperty, value);
    }

    public static readonly BindableProperty IsOkVisibileProperty = BindableProperty.Create(nameof(IsOkVisibile),
        typeof(bool), typeof(ThreeButtonsPanel),
        defaultBindingMode: BindingMode.OneTime);

    public bool IsOkVisibile
    {
        get => (bool)GetValue(IsOkVisibileProperty);
        set => SetValue(IsOkVisibileProperty, value);
    }


    public static readonly BindableProperty NoTextProperty = BindableProperty.Create(nameof(NoText), typeof(string),
        typeof(ThreeButtonsPanel),
        defaultBindingMode: BindingMode.OneTime);

    public string NoText
    {
        get => (string)GetValue(NoTextProperty);
        set => SetValue(NoTextProperty, value);
    }

    public static readonly BindableProperty IsNoVisibileProperty = BindableProperty.Create(nameof(IsNoVisibile),
        typeof(bool), typeof(ThreeButtonsPanel),
        defaultBindingMode: BindingMode.OneTime);

    public bool IsNoVisibile
    {
        get => (bool)GetValue(IsNoVisibileProperty);
        set => SetValue(IsNoVisibileProperty, value);
    }


    public static readonly BindableProperty CancelTextProperty = BindableProperty.Create(nameof(CancelText),
        typeof(string), typeof(ThreeButtonsPanel),
        defaultBindingMode: BindingMode.OneWay);

    public string CancelText
    {
        get => (string)GetValue(CancelTextProperty);
        set => SetValue(CancelTextProperty, value);
    }

    public static readonly BindableProperty IsCancelVisibileProperty = BindableProperty.Create(nameof(IsCancelVisibile),
        typeof(bool), typeof(ThreeButtonsPanel),
        defaultBindingMode: BindingMode.OneWay); /*, defaultValueCreator: GetDefaultCancelVisibility*/

    public bool IsCancelVisibile
    {
        get => (bool)GetValue(IsCancelVisibileProperty);
        set => SetValue(IsCancelVisibileProperty, value);
    }

    /*private static object GetDefaultCancelVisibility(BindableObject bindable) =>
        ((ThreeButtonsPanel)bindable).IsCancelVisibile = true;*/
}