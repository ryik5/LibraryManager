using LibraryManager.AbstractObjects;

namespace LibraryManager.ViewModels;

/// <author>YR 2025-04-27</author>
public class PopUpViewModel : AbstractBindableModel
{
    public PopUpViewModel()
    {
        CancellationTokenSource = new CancellationTokenSource();
        UpdateTimeOnUI(CancellationTokenSource).GetAwaiter();
    }

    private Task UpdateTimeOnUI(CancellationTokenSource cts)
    {
        // Run the task on a separate thread.
        return Task.Run(async () =>
        {
            // Loop until the cancellation token is cancelled.
            while (!cts.IsCancellationRequested)
            {
                // Sleep for 1 second.
                await Task.Delay(1000);
            }
        }, cts.Token);
    }

    public string TextField
    {
        get => _textField;
        set => SetProperty(ref _textField, value);
    }

    public string InputTextField
    {
        get => _inputTextField;
        set => SetProperty(ref _inputTextField, value);
    }

    public CancellationTokenSource CancellationTokenSource
    {
        get => _cancellationTokenSource;
        set => SetProperty(ref _cancellationTokenSource, value);
    }


    private CancellationTokenSource _cancellationTokenSource;
    private string _textField;
    private string _inputTextField;
    private bool _isWaited;
}