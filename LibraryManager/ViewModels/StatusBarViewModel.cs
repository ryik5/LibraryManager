using CommunityToolkit.Mvvm.Messaging;
using LibraryManager.AbstractObjects;
using System.Collections.Concurrent;

namespace LibraryManager.ViewModels;

/// <author>YR 2025-03-09</author>
public class StatusBarViewModel : AbstractBindableModel, IStatusBar
{
    public StatusBarViewModel()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        WeakReferenceMessenger.Default.Register<StatusMessage>(this,
            (r, m) => { Task.Run(() => _messages.Enqueue(m), _cancellationTokenSource.Token); });
        ReadStatusMessageFromQueueTask(_cancellationTokenSource).GetAwaiter();
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

    public List<IndexedString> DebugInfo
    {
        get => _debugInfo;
        private set => SetProperty(ref _debugInfo, value);
    }
    #endregion

    #region Private Methods
    private async Task SetStatusMessageTask(EInfoKind infoKind, string message)
    {
        switch (infoKind)
        {
            case EInfoKind.CommonInfo:
                await SetCommonInfo(message);
                await SetDebugInfo(message);
                break;
            case EInfoKind.CurrentInfo:
                await SetCurrentInfo(message);
                await SetDebugInfo(message);
                break;
            case EInfoKind.TotalBooks:
                var msg = $"Total book(s): {message}";
                await SetTotalBooks(msg);
                await SetDebugInfo(msg);
                break;
        }
    }
 
    private Task ReadStatusMessageFromQueueTask(CancellationTokenSource cts)
    {
        // Run the task on a separate thread.
        return Task.Run(async () =>
        {
            // Loop until the cancellation token is cancelled.
            while (!cts.IsCancellationRequested)
            {
                if (_messages.TryDequeue(out var msg))
                {
                    await SetStatusMessageTask(msg.InfoKind, msg.Message);
                }

                // Sleep for 200 milliseconds.
                await Task.Delay(200);
            }
        }, cts.Token);
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


    private readonly CancellationTokenSource _cancellationTokenSource;
    private static readonly ConcurrentQueue<StatusMessage> _messages = new();

    #region Private fields
    private string _statusInfo = String.Empty;
    private string _commonInfo = String.Empty;
    private string _currentInfo = String.Empty;
    private List<IndexedString> _debugInfo = new();
    private bool _isExisted;
    #endregion
}

public class StatusMessage
{
    public EInfoKind InfoKind { get; set; }
    public string Message { get; set; }
}

public class IndexedString
{
    public string TimeStamp { get; set; }
    public string Message { get; set; }
}