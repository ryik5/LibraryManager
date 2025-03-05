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
            Children =
            {
                new Label
                {
                    Text = title,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center
                },
                new Label
                {
                    Text = message,
                    HorizontalOptions = LayoutOptions.Center
                },
                new Button
                {
                    Text = "OK",
                    Command = new Command(async () =>
                    {
                        await CloseDialogAsync(true);
                    })
                },
                new Button
                {
                    Text = "Cancel",
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
