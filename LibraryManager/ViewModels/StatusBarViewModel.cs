using LibraryManager.AbstractObjects;

namespace LibraryManager.ViewModels;

public class StatusBarViewModel : AbstractBindableModel, IStatusBar
{
    public StatusBarViewModel()
    {
        Task.Run(async () => await SetTotalBooks(0));

        /*MessagingCenter.Subscribe<AbstractViewModel, string>(this, "Navigate", async (sender, args) =>
        {
            Console.WriteLine($"Received {args} from AbstractViewModel.");
            // await PerformAction("CreateLibrary").ConfigureAwait(false);
        });*/
    }

    #region Public Properties
    public string CommonInfo
    {
        get => _commonInfo;
        private set => SetProperty(ref _commonInfo, value);
    }

    public string CurrentInfo
    {
        get => _currentInfo;
        private set => SetProperty(ref _currentInfo, value);
    }

    public string StatusInfo
    {
        get => _statusInfo;
        private set => SetProperty(ref _statusInfo, value);
    }
    #endregion


    #region Public Methods
    public async Task SetStatusMessage(EInfoKind infoKind, string message)
    {
        switch (infoKind)
        {
            case EInfoKind.CommonMessage:
                await SetCommonInfo(message);
                break;
            case EInfoKind.DebugMessage:
                await SetCurrentInfo(message);
                break;
        }
    }

    public async Task SetStatusMessage(EInfoKind infoKind, int totalBooks)
    {
        switch (infoKind)
        {
            case EInfoKind.TotalBooks:
                await SetTotalBooks(totalBooks);
                break;
        }
    }

    private Task SetTotalBooks(int totalBooks)
    {
        StatusInfo = $"Total books: {totalBooks}";
        return Task.CompletedTask;
    }

    private Task SetCurrentInfo(string message)
    {
        CurrentInfo = message;
        return Task.CompletedTask;
    }

    private Task SetCommonInfo(string message)
    {
        CommonInfo = message;
        return Task.CompletedTask;
    }
    #endregion


    #region Private fields
    private string _statusInfo = String.Empty;
    private string _commonInfo = String.Empty;
    private string _currentInfo = String.Empty;
    #endregion
}