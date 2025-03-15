namespace LibraryManager.ViewModels;

public partial class EditBookPage : ContentPage
{
    public EditBookPage()
    {
        InitializeComponent();

        SaveButton.Command = new Command(async () =>
        {
            await CloseDialogAsync(true);
        }) ;
        CancelButton.Command =  new Command(async () =>
        {
            await CloseDialogAsync(false);
        });
    }
    
    public TaskCompletionSource<bool> DialogResultTask { get; set; } = new TaskCompletionSource<bool>();

    private async Task CloseDialogAsync(bool isConfirmed)
    {
        DialogResultTask.TrySetResult(isConfirmed);
        await Navigation.PopModalAsync(); // Close the modal Page
    }
}