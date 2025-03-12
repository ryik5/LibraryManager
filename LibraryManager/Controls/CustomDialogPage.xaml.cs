namespace LibraryManager.Views;

public partial class CustomDialogPage : ContentPage
{
    public CustomDialogPage(string title, string message)
    {
        var layout = new VerticalStackLayout
        {
            Padding = new Thickness(20),
            Spacing = 15,
            BackgroundColor = Colors.White,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 350,
            Children =
            {
                new Label
                {
                    Text = title,
                    FontSize = 20,
                    FontFamily = "ManropeExtraLight",
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center
                },
                new Label
                {
                    Text = message,
                    FontSize = 18,
                    FontFamily = "ManropeExtraLight",
                    HorizontalOptions = LayoutOptions.Center
                },
                new Button
                {
                    Text = "OK",
                    FontSize = 18,
                    Command = new Command(async () =>
                    {
                        await CloseDialogAsync(true);
                    })
                },
                new Button
                {
                    Text = "Cancel",
                    FontSize = 18,
                    Command = new Command(async () =>
                    {
                        await CloseDialogAsync(false);
                    })
                }
            }
        };

        Content = layout;
    }

    public TaskCompletionSource<bool> DialogResultTask { get; set; } = new TaskCompletionSource<bool>();

    private async Task CloseDialogAsync(bool isConfirmed)
    {
        DialogResultTask.TrySetResult(isConfirmed);
        await Navigation.PopModalAsync(); // Close the modal dialog
    }
}
