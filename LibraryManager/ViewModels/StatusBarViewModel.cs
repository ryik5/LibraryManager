using LibraryManager.AbstractObjects;

namespace LibraryManager.ViewModels;

public class StatusBarViewModel : AbstractBindableModel, IStatusBar
{
    public StatusBarViewModel()
    {
        /*MessagingCenter.Subscribe<AbstractViewModel, string>(this, "Navigate", async (sender, args) =>
        {
            Console.WriteLine($"Received {args} from AbstractViewModel.");
            // await PerformAction("CreateLibrary").ConfigureAwait(false);
        });*/
    }


    #region Public Methods
    public async Task SetStatusMessage(EInfoKind infoKind, string message)
    {
        switch (infoKind)
        {
            case EInfoKind.CommonInfo:
                await SetCommonInfo(message);
                break;
            case EInfoKind.CurrentInfo:
                await SetCurrentInfo(message);
                break;
        }

        await SetDebugInfo(message);
    }

    public async Task SetStatusMessage(EInfoKind infoKind, int totalBooks)
    {
        switch (infoKind)
        {
            case EInfoKind.TotalBooks:
                var message = $"Total book(s): {totalBooks}";
                await SetTotalBooks(message);
                await SetDebugInfo(message);
                break;
        }
    }

    private Task SetTotalBooks(string message)
    {
        StatusInfo = message;
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

    private Task SetDebugInfo(string message)
    {
        DebugInfo.Add(new IndexedString { TimeStamp = $"{DateTime.Now:HH:mm:ss:fff}", Message = message });
        return Task.CompletedTask;
    }
    #endregion


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

    public List<IndexedString> DebugInfo
    {
        get => _debugInfo;
        private set => SetProperty(ref _debugInfo, value);
    }
    #endregion


    #region Private fields
    private string _statusInfo = String.Empty;
    private string _commonInfo = String.Empty;
    private string _currentInfo = String.Empty;
    private List<IndexedString> _debugInfo = new();
    #endregion
}

public class IndexedString
{
    public string TimeStamp { get; set; }
    public string Message { get; set; }
}