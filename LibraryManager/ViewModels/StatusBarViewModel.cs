using LibraryManager.AbstractObjects;

namespace LibraryManager.ViewModels;

public class StatusBarViewModel : AbstractBindableModel, IStatusBar
{
    public StatusBarViewModel()
    {
        Task.Run(async () => await SetCommonInfo(""));
        Task.Run(async () => await SetCurrentInfo(""));
        Task.Run(async () => await SetTotalBooks(0));
    }

    #region Public Properties
    public string CommonInfo
    {
        get => _commonInfo;
        set => SetProperty(ref _commonInfo, value);
    }

    public string CurrentInfo
    {
        get => _currentInfo;
        set => SetProperty(ref _currentInfo, value);
    }

    public string StatusInfo
    {
        get => _statusInfo;
        set => SetProperty(ref _statusInfo, value);
    }
    #endregion


    #region Public Methods
    public async Task SetTotalBooks(int totalBooks)
    {
        StatusInfo = $"Total books: {totalBooks}";
    }

    public async Task SetCurrentInfo(string message)
    {
        CurrentInfo = message;
    }

    public async Task SetCommonInfo(string message)
    {
        CommonInfo = message;
    }
    #endregion


    #region Private fields
    private string _statusInfo;
    private string _commonInfo;
    private string _currentInfo;
    #endregion
}